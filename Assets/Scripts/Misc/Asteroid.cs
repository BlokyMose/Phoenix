using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class Asteroid : MonoBehaviour
    {
        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        [ShowInInspector, ReadOnly]
        float health;
        public float Health => health;



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
