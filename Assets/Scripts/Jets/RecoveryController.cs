using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class RecoveryController : MonoBehaviour
    {
        [SerializeField]
        float recoveryCooldown = 15f;

        [SerializeField]
        float recoverHealth = 10;

        Coroutine corDelayingRecovery;
        HealthController healthController;
        public Action OnStartRecovering;
        /// <summary>(time, cooldown)</summary>
        public Action<float, float, float> OnRecovering;
        public Action OnRecovered;

        void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            Exit();
        }

        void Init()
        {
            if (TryGetComponent(out healthController))
            {
                healthController.OnDamaged += OnDamaged;

                if (healthController.HealthBarUI!=null)
                    healthController.HealthBarUI.Init(this);
            }
        }

        void Exit()
        {
            if (TryGetComponent(out healthController))
            {
                healthController.OnDamaged -= OnDamaged;

                if (healthController.HealthBarUI != null)
                    healthController.HealthBarUI.Exit(this);
            }
        }

        void OnDamaged(float damage)
        {
            if (damage > 0)
                StartRecovering();
        }


        void StartRecovering()
        {
            if (corDelayingRecovery != null) 
                return;

            OnStartRecovering?.Invoke();

            corDelayingRecovery = StartCoroutine(Recovering());
            IEnumerator Recovering()
            {
                var _recoverHealth = healthController.Health + recoverHealth > healthController.MaxHealth
                    ? healthController.Health + recoverHealth - healthController.MaxHealth
                    : recoverHealth;

                var time = 0f;
                while (time < recoveryCooldown)
                {
                    time += Time.deltaTime;
                    OnRecovering?.Invoke(_recoverHealth, time,recoveryCooldown);
                    yield return null;
                }

                Recover(_recoverHealth);
            }
        }

        void Recover(float recoverHealth)
        {
            if (corDelayingRecovery != null) 
                StopCoroutine(corDelayingRecovery);
            corDelayingRecovery = null;
            healthController.ReceiveRecovery(recoverHealth);
            OnRecovered?.Invoke();
            TryStartRecovering();
        }

        void TryStartRecovering()
        {
            if(healthController.Health < healthController.MaxHealth)
                StartRecovering();
        }
    }
}
