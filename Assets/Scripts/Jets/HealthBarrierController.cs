using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class HealthBarrierController : MonoBehaviour
    {
        [SerializeField]
        float maxHealth = 50;
        public float MaxHealth => maxHealth;

        HealthController healthController;

        float health;
        public float Health => health;

        public Action<float> OnDamaged;
        public Action OnDie;

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            health = maxHealth;

            healthController = GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.OnDepleteBarrier += DepleteHealth;

                var playerBrain = GetComponent<PlayerBrain>();
                if (playerBrain != null)
                    playerBrain.ConnectToHealthBarrierBar(this);
            }
        }


        void OnDisable()
        {
            if (healthController != null)
            {
                healthController.OnDepleteBarrier -= DepleteHealth;
            }
        }

        float DepleteHealth(float damage)
        {
            health -= damage;
            OnDamaged?.Invoke(damage);

            if (health <= 0)
            {
                Die();
                return -health;
            }

            return 0;
        }

        void Die()
        {
            OnDie?.Invoke();
            Destroy(this);
        }
    }
}
