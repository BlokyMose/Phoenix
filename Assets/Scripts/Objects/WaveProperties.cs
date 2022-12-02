using Sirenix.OdinInspector;
using System;
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
        public string SOName
        {
            get => soName;
            set => soName = value;
        }

        [SerializeField, LabelWidth(0.1f)]
        GameObject prefab;
        public GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        [SerializeField]
        float delay = 1f;
        public float Delay
        {
            get => delay;
            set => delay = value;
        }

        [SerializeField]
        float period = 0.5f;
        public float Period
        {
            get => period;
            set => period = value;
        }

        [SerializeField]
        int count = 5;
        public int Count
        {
            get => count;
            set => count = value;
        }
        public float Duration => delay + period * count;


        public bool TryInstantiatePrefab(Transform transform, int currentCount, SpawnerData.WaveData cache)
        {
            if (cache.ToInstantiateIndex > currentCount || cache.ToInstantiateIndex > count)
                return false;

            if (prefab != null)
            {
                var go = Instantiate(prefab);
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
