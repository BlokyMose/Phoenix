using Cinemachine;
using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Phoenix
{
    public class CameraFXController : MonoBehaviour
    {
        [SerializeField]
        CameraShakeProperties camShakeDamaged;

        [SerializeField]
        VolumeProperties volumeDamaged;

        [SerializeField]
        VolumeProperties volumeDamagedBarrier;

        [SerializeField]
        LayerMask layer = 7;

        [SerializeField, Tooltip("damage value to match the volume's max weight")]
        float maxDamage = 30f;

        GameObject go;
        CinemachineImpulseSource impulse;
        Volume volume;
        Coroutine corVolume;

        void Awake()
        {
            Init();
        }

        public void Init(HealthController healthController)
        {
            healthController.OnDamaged += DamageFX;
        }

        public void Exit(HealthController healthController)
        {
            healthController.OnDamaged -= DamageFX;
        }

        public void Init(HealthBarrierController healthBarrier)
        {
            healthBarrier.OnDamaged += DamageBarrierFX;
        }

        public void Exit(HealthBarrierController healthBarrier)
        {
            healthBarrier.OnDamaged -= DamageBarrierFX;
        }

        public void Init()
        {
            go = new GameObject("Post");
            go.transform.parent = transform;
            go.layer = (int)Mathf.Log(layer.value, 2);

            impulse = go.AddComponent<CinemachineImpulseSource>();
            volume = go.AddComponent<Volume>();
            volume.weight = 0f;
            volume.isGlobal = true;
        }

        public void DamageFX(float damage)
        {
            var weight = damage / maxDamage;
            Shake(camShakeDamaged, weight); ;
            corVolume ??= StartCoroutine(ApplyVolume(volumeDamaged, weight));
        }

        public void DamageBarrierFX(float damage)
        {
            var weight = damage / maxDamage;
            Shake(camShakeDamaged, weight); ;
            corVolume ??= StartCoroutine(ApplyVolume(volumeDamagedBarrier, weight));
        }

        public void Shake(CameraShakeProperties properties, float strength = 0)
        {
            if (strength < 0) strength = 0;
            else if (strength > 1) strength = 1;

            impulse.m_ImpulseDefinition.m_ImpulseType = properties.ImpulseType;
            impulse.m_ImpulseDefinition.m_ImpulseShape = properties.ImpulseShape;

            float duration = properties.DurationMin + (properties.DurationMax - properties.DurationMin) * strength;
            impulse.m_ImpulseDefinition.m_ImpulseDuration = duration;

            bool isPositiveX = Random.Range(0, 2) == 0;
            bool isPositiveY = Random.Range(0, 2) == 0;

            var velocity = new Vector2(
                properties.VelocityMin.x + (properties.VelocityMax.x - properties.VelocityMin.x) * strength,
                properties.VelocityMin.y + (properties.VelocityMax.y - properties.VelocityMin.y) * strength
                );

            impulse.m_DefaultVelocity = new(
                Random.Range(velocity.x, velocity.x + properties.VelocityRange.x) * (isPositiveX ? 1 : -1),
                Random.Range(velocity.y, velocity.y + properties.VelocityRange.y) * (isPositiveY ? 1 : -1)
                );

            var force = properties.ForceMin + (properties.ForceMax - properties.ForceMin) * strength;
            var forceRandom = Random.Range(force - properties.ForceRange, force + properties.ForceRange);

            impulse.GenerateImpulse(forceRandom);
        }

        IEnumerator ApplyVolume(VolumeProperties properties, float weight)
        {
            yield return ApplyVolume(properties.Volume, properties.InDuration, properties.OutDuration, weight);
        }

        IEnumerator ApplyVolume(VolumeProfile profile, float inDuration, float outDuration, float weight)
        {
            volume.profile = profile;
            volume.weight = 0f;

            if (weight > 1f) weight = 1f;
            else if (weight < 0f) weight = 0f;

            var time = 0f;
            while (time < inDuration)
            {
                volume.weight = weight * (time / inDuration);
                time += Time.deltaTime;
                yield return null;
            }

            time = outDuration;
            while (time > 0f)
            {
                volume.weight = weight * (time / outDuration);
                time -= Time.deltaTime;
                yield return null;
            }

            volume.weight = 0;

            corVolume = null;
        }
    }
}
