using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class HealthController : MonoBehaviour, IElemental
    {
        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        [ShowInInspector, ReadOnly]
        float health;
        public float Health => health;

        [SerializeField]
        Element element;

        public Element Element => element;

        public Action<float> OnDamaged;
        public Action OnDie;


        private void Awake()
        {
            health = maxHealth;
        }

        public void ReceiveDamage(float damage)
        {
            health -= damage;
            OnDamaged?.Invoke(damage);

            if (health < 0)
                Die();
        }

        public void Die()
        {
            OnDie?.Invoke();
        }

    }
}
