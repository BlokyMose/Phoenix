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

        public float Health => health;

        public float MaxHealth => maxHealth;

        private void Awake()
        {
            health = maxHealth;
        }



        public void ReceiveDamage(float damage)
        {
            health -= damage;
        }

        public void Die()
        {
        }
    }
}
