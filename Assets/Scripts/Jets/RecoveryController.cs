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
        public Action<float, float> OnRecovering;
        public Action OnRecovered;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            healthController = GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.OnDamaged += (damage) =>
                {
                    if (damage > 0)
                    {
                        StartRecovering();
                    }
                };

                var playerBrain = GetComponent<PlayerBrain>();
                if (playerBrain != null)
                    playerBrain.ConnectToHealthBar(this);
            }
        }

        void StartRecovering()
        {
            if (corDelayingRecovery != null) 
                return;

            OnStartRecovering?.Invoke();

            corDelayingRecovery = StartCoroutine(Recovering());
            IEnumerator Recovering()
            {
                var time = 0f;
                while (time < recoveryCooldown)
                {
                    time += Time.deltaTime;
                    OnRecovering?.Invoke(time,recoveryCooldown);
                    yield return null;
                }

                Recover(recoverHealth);
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
            {
                StartRecovering();
            }
        }
    }
}
