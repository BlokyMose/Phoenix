using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.WaveController;

namespace Phoenix
{
    [InlineEditor]
    public class WavePropertiesStatic : WaveProperties
    {
        [SerializeField, InlineButton(nameof(SetSOName), "Set Name"), LabelWidth(0.1f)]
        protected string soName = "Wave Name";
        public override string SOName
        {
            get => soName;
            set => soName = value;
        }

        [SerializeField, LabelWidth(0.1f)]
        protected GameObject prefab;
        public override GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }
        public override GameObject GetPrefab() => prefab;

        [SerializeField]
        protected float delay = 1f;
        public override float Delay
        {
            get => delay;
            set => delay = value;
        }

        public override float GetDelay() => delay;

        [SerializeField]
        protected float period = 0.5f;
        public override float Period
        {
            get => period;
            set => period = value;
        }
        public override float GetPeriod() => count;

        [SerializeField]
        protected int count = 5;
        public override int Count
        {
            get => count;
            set => count = value;
        }
        public override int GetCount() => count;

        public override float Duration => delay + period * count;


    }
}
