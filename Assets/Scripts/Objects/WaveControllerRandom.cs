using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    public class WaveControllerRandom : WaveController
    {
        #region [Classes]

        [Serializable]
        public class WaveProbabilty
        {
            [SerializeField, Range(0,100), LabelText("Probability")]
            int chosenProbability = 25;
            public int ChosenProbability => chosenProbability;

            public WaveProbabilty(int chosenProbability)
            {
                this.chosenProbability = chosenProbability;
            }
        }

        class SpawnerDataRandom : SpawnerData
        {
            public float time;
            public float maxTime;

            public SpawnerDataRandom(Spawner spawner) : base(spawner)
            {
            }
        }

        struct SpawnerAndDataRandom
        {
            public Spawner spawner;
            public SpawnerDataRandom spawnerData;

            public SpawnerAndDataRandom(Spawner spawner, SpawnerDataRandom spawnerData)
            {
                this.spawner = spawner;
                this.spawnerData = spawnerData;
            }
        }


        #endregion

        [SerializeField, LabelText("Chances for wave by index")]
        List<WaveProbabilty> probabilities = new();


        protected override void StartSpawning()
        {
            corSpawning = this.RestartCoroutine(Update());
            IEnumerator Update()
            {
                List<SpawnerAndDataRandom> spawnerAndDataList = new();
                foreach (var spawner in spawners)
                    spawnerAndDataList.Add(new SpawnerAndDataRandom(spawner, new SpawnerDataRandom(spawner)));

                InitRandomWave(spawnerAndDataList);


                while (true)
                {
                    int finishedSpawnersCount = 0;
                    foreach (var spawnerAndData in spawnerAndDataList)
                    {
                        if (spawnerAndData.spawnerData.time < spawnerAndData.spawnerData.maxTime)
                        {
                            spawnerAndData.spawnerData.time += Time.fixedDeltaTime;
                            spawnerAndData.spawner.Evaluate(spawnerAndData.spawnerData.time, spawnerAndData.spawnerData);
                        }
                        else
                        {
                            finishedSpawnersCount++;
                        }
                    }

                    if (finishedSpawnersCount >= spawnerAndDataList.Count)
                        InitRandomWave(spawnerAndDataList);

                    yield return null;
                }

                void InitRandomWave(List<SpawnerAndDataRandom> spawnerAndDataList)
                {
                    int index = GetRandomWaveIndex();
                    foreach (var spawnerAndData in spawnerAndDataList)
                    {
                        spawnerAndData.spawnerData.Reset();
                        var startTimeAndMaxTime = GetStartAndEndTimeOfWaveIndex(index, spawnerAndData.spawner.Waves);
                        spawnerAndData.spawnerData.time = startTimeAndMaxTime.Item1;
                        spawnerAndData.spawnerData.maxTime = startTimeAndMaxTime.Item2;
                    }
                }

                int GetRandomWaveIndex()
                {
                    int chosenWaveIndex = 0;
                    var totalProbabilities = 0;
                    foreach (var probability in probabilities)
                        totalProbabilities += probability.ChosenProbability;

                    var random = Random.Range(0, totalProbabilities);
                    int currentProbabilitiesSum = 0;
                    int waveIndex = 0;
                    foreach (var probability in probabilities)
                    {
                        if (probability.ChosenProbability > 0)
                        {
                            currentProbabilitiesSum += probability.ChosenProbability;
                            if (random <= currentProbabilitiesSum)
                            {
                                chosenWaveIndex = waveIndex;
                                break;
                            }
                        }
                        waveIndex++;
                    }

                    return chosenWaveIndex;
                }

                Tuple<float,float> GetStartAndEndTimeOfWaveIndex(int index, List<WaveProperties> waves)
                {
                    if (index > waves.Count - 1)
                        return new Tuple<float, float>(-1, -1);

                    var startTime = 0f;
                    for (int i = 0; i < index; i++)
                        startTime += waves[i].Duration;
                    var endTime = startTime + waves[index].Duration;

                    return new Tuple<float, float>(startTime, endTime);
                }
            }
        }

        [Button]
        void SyncProbabilitiesWithSpawners()
        {
            probabilities = new();
            var maxWavesCount = 0;
            foreach (var spawner in spawners)
                if (spawner.Waves.Count > maxWavesCount)
                    maxWavesCount = spawner.Waves.Count;

            var equalProbability = 100 / maxWavesCount;
            for (int i = 0; i < maxWavesCount; i++)
                probabilities.Add(new WaveProbabilty(equalProbability));

        }
    }
}
