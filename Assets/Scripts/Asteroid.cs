using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class Asteroid : MonoBehaviour, IHealth
    {
        [SerializeField]
        float maxHealth = 100;

        [ShowInInspector, ReadOnly]
        float health;

        private void Awake()
        {
            health = maxHealth;
        }

        public float GetHealth()
        {
            return health;
        }

        public void ReceiveDamage(float damage)
        {
            health -= damage;
        }
    }
}
