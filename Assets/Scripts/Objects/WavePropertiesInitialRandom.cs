using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Phoenix
{
    public class WavePropertiesInitialRandom : WavePropertiesRandom
    {
        public GameObject LastPrefab { get; private set; }
        public float? LastDelay { get; private set; }
        public float? LastPeriod { get; private set; }
        public int? LastCount { get; private set; }

        public override GameObject GetPrefab()
        {
            LastPrefab = LastPrefab == null ? base.GetPrefab() : LastPrefab;
            return LastPrefab;
        }

        public override float GetDelay()
        {
            LastDelay ??= base.GetDelay();
            return (float)LastDelay;
        }

        public override float GetPeriod()
        {
            LastPeriod ??= base.GetPeriod();
            return (float)LastPeriod;
        }

        public override int GetCount()
        {
            LastCount ??= base.GetCount();
            return (int)LastCount;
        }

        public void ResetCache()
        {
            LastPrefab = null;
            LastDelay = null;
            LastPeriod = null;
            LastCount = null;
        }
    }
}
