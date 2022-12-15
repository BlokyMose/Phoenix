using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static Phoenix.WaveController;
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

            public void Reinit(Spawner spawner, SpawnerDataRandom spawnerData)
            {
                this.spawner = spawner;
                this.data = spawnerData;
            }
        }

        [Serializable]
        public class SpawnerSequence
        {
            [SerializeField]
            List<Spawner> spawners = new List<Spawner>();
            public List<Spawner> Spawners => spawners;
        }

        #endregion

        [SerializeField, LabelText("Chances for wave by index")]
        List<WaveProbabilty> waveProbabilities = new();

        [SerializeField]
        List<SpawnerSequence> spawnerSequences = new();
        public List<SpawnerSequence> SpawnerSequences => spawnerSequences;    

        [SerializeField, Range(0,100)]
        int chanceToInstantiate = 100;

        int currentWaveIndex = 0;
        int currentSequence = 0;

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
                            var instantiableWave = spawnerAndData.spawner.GetInstantiableWaveAt(spawnerAndData.data.time, spawnerAndData.data);
                            if (instantiableWave != null)
                                InstantiateWavePrefab(instantiableWave, spawnerAndData.spawner.Position);
                        }
                        else finishedSpawnersCount++;
                    }

                    if (finishedSpawnersCount >= spawnerAndDataList.Count)
                        InitRandomWave(spawnerAndDataList);
                    
                    yield return null;
                }

                // = = =

                void InitRandomWave(List<SpawnerAndDataRandom> spawnerAndDataList)
                {
                    spawnerAndDataList.Clear();

                    if (currentSequence >= SpawnerSequences.Count)
                    {
                        currentWaveIndex = GetRandomWaveIndex();
                        foreach (var spawner in spawners)
                            spawnerAndDataList.Add(NewSpawnerAndData(spawner, currentWaveIndex));

                        currentSequence = 0;
                    }
                    else
                    {
                        foreach (var spawner in spawnerSequences[currentSequence].Spawners)
                            spawnerAndDataList.Add(NewSpawnerAndData(spawner, currentWaveIndex));

                        currentSequence++;
                    }

                    SpawnerAndDataRandom NewSpawnerAndData(Spawner spawner, int startWaveIndex)
                    {
                        var spawnerAndData = new SpawnerAndDataRandom(spawner, new SpawnerDataRandom(spawner));
                        var startTimeAndMaxTime = GetStartAndEndTimeOfWaveIndex(startWaveIndex, spawnerAndData.spawner.Waves);
                        spawnerAndData.data.time = startTimeAndMaxTime.Item1;
                        spawnerAndData.data.maxTime = startTimeAndMaxTime.Item2;


                        // TODO: move this do DataRandom
                        foreach (var wave in spawnerAndData.spawner.Waves)
                        {
                            if (wave is WavePropertiesInitialRandom)
                                (wave as WavePropertiesInitialRandom).ResetCache();
                        }

                        return spawnerAndData;
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
            if (Random.Range(0, 100) > chanceToInstantiate) return;
            base.InstantiateWavePrefab(wave, transform);
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
