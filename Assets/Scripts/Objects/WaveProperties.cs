using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.WaveController;

namespace Phoenix
{
    [InlineEditor]
    public class WaveProperties : ScriptableObject
    {
        [SerializeField, InlineButton(nameof(SetSOName), "Set Name"), LabelWidth(0.1f)]
        string soName = "Wave Name";
        public string SOName => soName;

        [SerializeField, LabelWidth(0.1f)]
        GameObject enemyPrefab;
        public GameObject EnemyPrefab => enemyPrefab;

        [SerializeField]
        float delay = 1f;
        public float Delay => delay;

        [SerializeField]
        float period = 0.5f;
        public float Period => period;

        [SerializeField]
        int count = 5;
        public int Count => count;
        public float Duration => delay + period * count;


        public bool TryInstantiatePrefab(Transform transform, int currentCount, SpawnerData.WaveData cache)
        {
            if (cache.ToInstantiateIndex > currentCount || cache.ToInstantiateIndex > count)
                return false;

            if (enemyPrefab != null)
            {
                var go = Instantiate(enemyPrefab);
                go.SetActive(true);
                go.transform.SetParent(null);
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                cache.ToInstantiateIndex++;
            }

            return true;
        }

        void SetSOName()
        {
            name = soName;
        }
    }
}
