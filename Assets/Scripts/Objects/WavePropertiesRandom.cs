using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class WavePropertiesRandom : WavePropertiesStatic
    {
        [SerializeField]
        List<GameObject> alternatePrefabs = new List<GameObject>();
        public List<GameObject> AlternatePrefabs
        {
            get => alternatePrefabs; 
            set => alternatePrefabs = value;
        }
        public override GameObject GetPrefab()
        {
            var allPrefabs = new List<GameObject>() { prefab };
            allPrefabs.AddRange(alternatePrefabs);
            var random = Random.Range(0, allPrefabs.Count);
            return allPrefabs[random];
        }

        [SerializeField]
        float delayRange = 1f;
        public float DelayRange
        {
            get => delayRange;
            set => delayRange = value;
        }
        public override float GetDelay()
        {
            var min = delay - delayRange / 2;
            if (min < 0) min = 0;
            var max = delay + delayRange / 2;
            return Random.Range(min, max);
        }

        [SerializeField]
        float periodRange = 1f;
        public float PeriodRange
        {
            get => periodRange;
            set => periodRange = value;
        }
        public override float GetPeriod()
        {
            var min = period - periodRange / 2;
            if (min < 0) min = 0;
            var max = period + periodRange / 2;
            return Random.Range(min, max);
        }

        [SerializeField]
        int countRange = 2;
        public int CountRange
        {
            get => countRange;
            set => countRange = value;
        }
        public override int GetCount()
        {
            var min = count - countRange / 2;
            if (min < 0) min = 0;
            var max = count + countRange / 2;
            return Random.Range(min, max);
        }

        public void Setup(string SOName, GameObject prefab, float delay, float period, int count,
    List<GameObject> alternatePrefabs, float delayRange, float periodRange, int countRange)
        {
            base.Setup(SOName, prefab, delay, period, count);
            this.AlternatePrefabs = alternatePrefabs;
            this.DelayRange = delayRange;
            this.PeriodRange = periodRange;
            this.CountRange = countRange;
        }
    }
}
