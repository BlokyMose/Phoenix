using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.WaveController;
using static Phoenix.WaveController.Spawner;

namespace Phoenix
{
    public class WaveController : MonoBehaviour
    {
        #region [Classes]

        [Serializable]
        public class Spawner
        {
            [SerializeField]
            bool isActive = true;
            public bool IsActive
            {
                get => isActive;
                set => isActive = value;
            }

            [SerializeField]
            Transform position;
            public Transform Position
            {
                get => position;
                set => position = value;
            }

            [SerializeField]
            bool isLoop = false;
            public bool IsLoop
            {
                get => isLoop;
                set => isLoop = value;
            }

            [SerializeField]
            List<WaveProperties> waves = new List<WaveProperties>();
            public List<WaveProperties> Waves => waves;

            /// <summary>
            /// Get which wave should instantiate its prefab based on time and spawnerData;<br></br> 
            /// Alters spawnerData.toInstantiateNextIndex based on time
            /// </summary>
            public WaveProperties GetInstantiableWave(float time, SpawnerData spawnerCache)
            {
                if (!isActive) return null;

                // Convert total time to time substracted by all previous loops
                var timeInLoop = time % TotalWavesDuration();
                if (time > TotalWavesDuration() && isLoop)
                    spawnerCache.ResetDataForLoop((int)Mathf.Floor(time / TotalWavesDuration()));

                float currentWaveDurationSum = 0f;
                foreach (var wave in waves)
                {
                    // Check which wave timeInLoop is in
                    currentWaveDurationSum += wave.Duration;
                    if (timeInLoop < currentWaveDurationSum)
                    {
                        var totalDurationOfPreviousWaves = currentWaveDurationSum - wave.Duration;
                        var timeInWave = timeInLoop - totalDurationOfPreviousWaves;
                        if (timeInWave >= wave.GetDelay())
                        {
                            var timeAfterDelay = timeInWave - wave.GetDelay();
                            for (int subWaveIndex = 0; subWaveIndex < wave.GetCount(); subWaveIndex++)
                            {
                                // Check which period of the wave timeAfterDelay is in
                                if (timeAfterDelay > wave.GetPeriod() * subWaveIndex)
                                {
                                    var waveCache = spawnerCache.GetWaveData(wave);
                                    if (waveCache.ToInstantiateNextIndex <= subWaveIndex)
                                    {
                                        waveCache.ToInstantiateNextIndex++;
                                        return wave;
                                    }
                                }
                            }
                        }

                        break;
                    }
                }

                return null;



            }

            public float TotalWavesDuration()
            {
                var totalDuration = 0f;
                foreach (var wave in waves)
                    totalDuration += wave.Duration;

                return totalDuration;
            }

            [HorizontalGroup(0.5f), Button]
            void AddWave()
            {
                waves.Add(ScriptableObject.CreateInstance<WavePropertiesStatic>());
            }

            [HorizontalGroup(0.5f), Button]
            void AddWaveRandom()
            {
                waves.Add(ScriptableObject.CreateInstance<WavePropertiesInitialRandom>());
            }
        }

        public class SpawnerData
        {
            public class WaveData
            {
                WaveProperties wave;
                public WaveProperties Wave => wave;

                int toInstantiateIndex;
                /// <summary>Cache data of from which index instantiation is possible</summary>
                public int ToInstantiateNextIndex
                {
                    get => toInstantiateIndex;
                    set { toInstantiateIndex = value; }
                }

                public WaveData(WaveProperties wave, int toInstantiateIndex)
                {
                    this.wave = wave;
                    this.toInstantiateIndex = toInstantiateIndex;
                }

                public void Reset()
                {
                    toInstantiateIndex = 0;
                }
            }

            List<WaveData> wavesData = new List<WaveData>();
            public List<WaveData> WavesData => wavesData;

            int loopCount = 1;

            public SpawnerData(Spawner spawner)
            {
                wavesData = new();
                foreach (var wave in spawner.Waves)
                    wavesData.Add(new WaveData(wave, 0));
            }

            public WaveData GetWaveData(WaveProperties wave)
            {
                foreach (var waveData in wavesData)
                    if (waveData.Wave == wave)
                        return waveData;

                return null;
            }

            public void ResetDataForLoop(int currentLoopCount)
            {
                if (currentLoopCount < loopCount)
                    return;

                foreach (var waveData in wavesData)
                    waveData.ToInstantiateNextIndex = 0;

                loopCount++;
            }

            public void Reset()
            {
                foreach (var waveData in wavesData)
                    waveData.Reset();

                loopCount = 1;
            }
        }

        struct SpawnerAndData
        {
            public Spawner spawner;
            public SpawnerData data;

            public SpawnerAndData(Spawner spawner, SpawnerData spawnerData)
            {
                this.spawner = spawner;
                this.data = spawnerData;
            }
        }

        #endregion

        [SerializeField]
        protected List<Spawner> spawners = new List<Spawner>();
        public List<Spawner> Spawners => spawners;

        protected Coroutine corSpawning;

        void OnEnable()
        {
            Init();
        }

        void Init()
        {
            StartSpawning();
        }

        protected virtual void StartSpawning()
        {
            corSpawning = this.RestartCoroutine(Update());
            IEnumerator Update()
            {
                List<SpawnerAndData> spawnerAndDataList = new();
                foreach (var spawner in spawners)
                    spawnerAndDataList.Add(new SpawnerAndData(spawner, new SpawnerData(spawner)));

                var time = 0f;

                while (true)
                {
                    foreach (var spawnerAndData in spawnerAndDataList)
                    {
                        var instantiableWave = spawnerAndData.spawner.GetInstantiableWave(time, spawnerAndData.data);
                        InstantiateWavePrefab(instantiableWave, spawnerAndData.spawner.Position);
                    }

                    time += Time.fixedDeltaTime;
                    yield return null;
                }

            }
        }

        protected virtual void InstantiateWavePrefab(WaveProperties wave, Transform transform)
        {
            if (wave == null) return;

            if (wave.GetPrefab() != null)
            {
                var go = Instantiate(wave.GetPrefab());
                go.SetActive(true);
                go.transform.SetParent(null);
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
            }
        }

    }
}
