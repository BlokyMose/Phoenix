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
            public SpawnerDataRandom data;

            public SpawnerAndDataRandom(Spawner spawner, SpawnerDataRandom spawnerData)
            {
                this.spawner = spawner;
                this.data = spawnerData;
            }
        }


        #endregion

        [SerializeField, LabelText("Chances for wave by index")]
        List<WaveProbabilty> waveProbabilities = new();

        [SerializeField]
        List<List<Spawner>> alternativeSpawnerLists = new List<List<Spawner>>();

        [SerializeField, Range(0,100)]
        int chanceToInstantiate = 100;

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
                        if (spawnerAndData.data.time < spawnerAndData.data.maxTime)
                        {
                            spawnerAndData.data.time += Time.fixedDeltaTime;
                            var instantiableWave = spawnerAndData.spawner.GetInstantiableWave(spawnerAndData.data.time, spawnerAndData.data);

                            InstantiateWavePrefab(instantiableWave, spawnerAndData.spawner.Position);
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
                        spawnerAndData.data.Reset();
                        var startTimeAndMaxTime = GetStartAndEndTimeOfWaveIndex(index, spawnerAndData.spawner.Waves);
                        spawnerAndData.data.time = startTimeAndMaxTime.Item1;
                        spawnerAndData.data.maxTime = startTimeAndMaxTime.Item2;

                        foreach (var wave in spawnerAndData.spawner.Waves)
                        {
                            if (wave is WavePropertiesInitialRandom)
                                (wave as WavePropertiesInitialRandom).ResetCache();
                        }
                    }
                }

                int GetRandomWaveIndex()
                {
                    int chosenWaveIndex = 0;
                    var totalProbabilities = 0;
                    foreach (var probability in waveProbabilities)
                        totalProbabilities += probability.ChosenProbability;

                    var random = Random.Range(0, totalProbabilities);
                    int currentProbabilitiesSum = 0;
                    int waveIndex = 0;
                    foreach (var probability in waveProbabilities)
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

        protected override void InstantiateWavePrefab(WaveProperties wave, Transform transform)
        {
            if (wave == null) return;
            if (Random.Range(0, 100) > chanceToInstantiate) return;

            if (wave.GetPrefab() != null)
            {
                var go = Instantiate(wave.GetPrefab());
                go.SetActive(true);
                go.transform.SetParent(null);
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
            }
        }

        [Button]
        void SyncWaveProbabilitiesWithSpawners()
        {
            waveProbabilities = new();
            var maxWavesCount = 0;
            foreach (var spawner in spawners)
                if (spawner.Waves.Count > maxWavesCount)
                    maxWavesCount = spawner.Waves.Count;

            var equalProbability = 100 / maxWavesCount;
            for (int i = 0; i < maxWavesCount; i++)
                waveProbabilities.Add(new WaveProbabilty(equalProbability));

        }
    }
}
