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

        [SerializeField]
        HealthBarUI healthBarrierUI;

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

            if (TryGetComponent<HealthController>(out healthController))
            {
                healthController.OnDepleteBarrier += DepleteHealth;
            }

            if (TryGetComponent<CameraFXController>(out var cameraFX))
                cameraFX.Init(this);

            if (healthBarrierUI != null)
            {
                healthBarrierUI.Init(this); 
            }
        }

        private void OnDestroy()
        {
            Exit();
        }

        void Exit()
        {
            if (healthController != null)
            {
                healthController.OnDepleteBarrier -= DepleteHealth;
            }

            if (TryGetComponent<CameraFXController>(out var cameraFX))
                cameraFX.Exit(this);

            if (healthBarrierUI != null)
            {
                healthBarrierUI.Exit(this);
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
