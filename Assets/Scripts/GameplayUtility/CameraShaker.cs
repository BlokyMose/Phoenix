using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

namespace Phoenix
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField]
        Vector2 velocityMinimal = new(0.1f, 0.133f);

        [SerializeField]
        Vector2 velocityRange = new(0.33f, 0.366f);

        [SerializeField]
        float force = 0.25f;

        [SerializeField]
        float forceRange = 0.1f;

        CinemachineImpulseSource impulse;

        private void Awake()
        {
            impulse = GetComponent<CinemachineImpulseSource>();
        }

        public void Init(HealthController healthController)
        {
            healthController.OnDamaged += (damage) => Shake();
        }

        public void Exit(HealthController healthController)
        {
            healthController.OnDamaged -= (damage) => Shake();
        }

        public void Shake()
        {
            bool isPositiveX = Random.Range(0,2) == 0;
            bool isPositiveY = Random.Range(0,2) == 0;

            impulse.m_DefaultVelocity = new(
                Random.Range(velocityMinimal.x, velocityMinimal.x + velocityRange.x) * (isPositiveX ? 1 : -1),
                Random.Range(velocityMinimal.y, velocityMinimal.y + velocityRange.y) * (isPositiveY ? 1 : -1)
                );

            impulse.GenerateImpulse(Random.Range(force - forceRange, force + forceRange));
        }
    }
}
