using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public abstract class WaveProperties : ScriptableObject
    {
        public virtual string SOName { get; set; }

        /// <summary> For game use </summary>
        public abstract GameObject GetPrefab();
        public virtual GameObject Prefab { get; set; }

        /// <summary> For game use </summary>
        public abstract float GetDelay();
        public virtual float Delay { get; set; }

        /// <summary> For game use </summary>
        public abstract float GetPeriod();
        public virtual float Period { get; set; }

        /// <summary> For game use </summary>
        public abstract int GetCount();
        public virtual int Count { get; set; }

        public virtual float Duration { get; }

        protected void SetSOName()
        {
            name = SOName;
        }
    }
}
