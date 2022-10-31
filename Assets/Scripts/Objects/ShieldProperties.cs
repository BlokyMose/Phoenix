using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Properties/Shield Properties", fileName = "SP_")]
    [InlineEditor]
    public class ShieldProperties : ScriptableObject
    {
        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        [SerializeField]
        Element element;
        public Element Element => element;
    }
}
