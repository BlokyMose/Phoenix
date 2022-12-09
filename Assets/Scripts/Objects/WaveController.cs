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

            public void Evaluate(float time, SpawnerData cache)
            {
                if (!isActive) return;

                // Convert total time to time substracted by all previous loops
                var timeInLoop = time % TotalWavesDuration();
                if (time > TotalWavesDuration() && isLoop)
                    cache.ResetDataForLoop((int)Mathf.Floor(time / TotalWavesDuration()));

                float currentWaveDurationSum = 0f;
                foreach (var wave in waves)
                {
                    // Check which wave timeInLoop is in
                    currentWaveDurationSum += wave.Duration;
                    if (timeInLoop < currentWaveDurationSum)
                    {
                        var totalDurationOfPreviousWaves = currentWaveDurationSum - wave.Duration;
                        var timeInWave = timeInLoop - totalDurationOfPreviousWaves;
                        if (timeInWave >= wave.Delay)
                        {
                            var timeAfterDelay = timeInWave - wave.Delay;
                            for (int i = 0; i < wave.Count; i++)
                            {
                                // Check which period of the wave timeAfterDelay is in
                                if (timeAfterDelay > wave.Period * i)
                                {
                                    if (wave.TryInstantiatePrefab(position, i, cache.GetWaveData(wave)))
                                        break;
                                }
                            }
                        }

                        break;
                    }
                }

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
                waves.Add(ScriptableObject.CreateInstance<WaveProperties>());
            }
        }

        public class SpawnerData
        {
            public class WaveData
            {
                WaveProperties wave;
                public WaveProperties Wave => wave;

                int toInstantiateIndex;
                public int ToInstantiateIndex
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
                    waveData.ToInstantiateIndex = 0;

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
            public SpawnerData spawnerData;

            public SpawnerAndData(Spawner spawner, SpawnerData spawnerData)
            {
                this.spawner = spawner;
                this.spawnerData = spawnerData;
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
                        spawnerAndData.spawner.Evaluate(time, spawnerAndData.spawnerData);

                    time += Time.fixedDeltaTime;
                    yield return null;
                }

            }
        }

        
    }
}
