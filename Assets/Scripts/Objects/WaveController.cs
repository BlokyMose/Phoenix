using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.WaveController.Spawner;

namespace Phoenix
{
    public class WaveController : MonoBehaviour
    {
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

                var timeInLoop = time % TotalWavesDuration();
                if (time > TotalWavesDuration() && isLoop)
                    cache.ResetDataForLoop((int)Mathf.Floor(time / TotalWavesDuration()));

                float currentWaveDurationSum = 0f;
                foreach (var wave in waves)
                {
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
            }

            List<WaveData> wavesData = new List<WaveData>();
            public List<WaveData> WavesData => wavesData;

            int loopCount = 1;

            public SpawnerData(Spawner spawner)
            {
                wavesData = new List<WaveData>();
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
        }


        [SerializeField]
        List<Spawner> spawners = new List<Spawner>();
        public List<Spawner> Spawners
        {
            get => spawners;
        } 

        void Awake()
        {
            Init();
        }

        void Init()
        {
            StartSpawning();
        }

        void StartSpawning()
        {
            StartCoroutine(Update());
            IEnumerator Update()
            {
                List<Tuple<Spawner,SpawnerData>> spawnersAndData = new();
                foreach (var spawner in spawners)
                    spawnersAndData.Add(Tuple.Create(spawner, new SpawnerData(spawner)));

                var time = 0f;

                while (true)
                {
                    foreach (var spawnerAndData in spawnersAndData)
                        spawnerAndData.Item1.Evaluate(time, spawnerAndData.Item2);

                    time += Time.fixedDeltaTime;
                    yield return null;
                }

            }
        }

        
    }
}
