using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Phoenix
{
    [RequireComponent(typeof(Collider2D))]
    public class InTriggerGiveDamage : MonoBehaviour
    {
        public enum DamageMode { Static, MinMax }

        [System.Serializable]
        public class MinMax
        {
            public float min;
            public float max;

            public MinMax(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public class EnteredHC
        {
            Collider2D col;
            HealthController healthController;
            Coroutine corDamaging;
            Volume volume;
            VolumeProperties volumeProperties;

            public Collider2D Col { get => col; }
            public HealthController HealthController { get => healthController; }
            public Coroutine CorDamaging { get => corDamaging; }

            public EnteredHC(Collider2D col, HealthController healthController)
            {
                this.col = col;
                this.healthController = healthController;
            }

            public void Init(MonoBehaviour context, float damage, float period, DamageCanvas damageCanvasPrefab = null, Color? damageColor = null, VolumeProperties volumeProperties = null, MinMax maxWeightDistance = null)
            {
                this.volumeProperties = volumeProperties;
                if (volumeProperties != null && healthController.TryGetComponentInFamily<CameraFXController>(out var cameraFX))
                    volume = cameraFX.AddVolume(volumeProperties);

                corDamaging = context.RestartCoroutine(Damaging(context, period, ()=>(int)damage, damageCanvasPrefab, damageColor, maxWeightDistance));
            }

            public void Init(MonoBehaviour context, float damage, float maxDamage, MinMax maxDamageDistance, float period, DamageCanvas damageCanvasPrefab = null, Color? damageColor = null, VolumeProperties volumeProperties = null, MinMax maxWeightDistance = null)
            {
                this.volumeProperties = volumeProperties;
                if (volumeProperties != null && healthController.TryGetComponentInFamily<CameraFXController>(out var cameraFX))
                    volume = cameraFX.AddVolume(volumeProperties);

                corDamaging = context.RestartCoroutine(Damaging(context, period, () => (int)damage + GetExtraDamage(), damageCanvasPrefab, damageColor, maxWeightDistance));

                int GetExtraDamage()
                {
                    var maxDamageMargin = Mathf.RoundToInt(maxDamage - damage);
                    var extraDamage = 0;
                    var distance = Vector2.Distance(healthController.transform.position, context.transform.position);
                    if (distance > maxDamageDistance.min)
                        extraDamage = 0;
                    else if (distance < maxDamageDistance.max)
                        extraDamage = maxDamageMargin;
                    else
                        extraDamage = Mathf.RoundToInt((1 - (distance - maxDamageDistance.max) / (maxDamageDistance.min - maxDamageDistance.max)) * maxDamageMargin);

                    return extraDamage;
                }
            }

            IEnumerator Damaging(MonoBehaviour context, float period, Func<int> GetDamage, DamageCanvas damageCanvasPrefab = null, Color? damageColor = null, MinMax maxWeightDistance = null)
            {
                var timer = period;

                while (true)
                {
                    if (timer < 0)
                    {
                        var damage = GetDamage();
                        timer = period;
                        healthController.ReceiveDamage(damage);
                        if (context.TryInstantiate(damageCanvasPrefab, out var damageCanvas))
                        {
                            damageCanvas.transform.position = healthController.transform.position;
                            damageCanvas.Init(damage, damageColor);
                        }

                    }

                    if (volume != null)
                    {
                        var distance = Vector2.Distance(healthController.transform.position, context.transform.position);
                        if (distance > maxWeightDistance.min)
                            volume.weight = 0;
                        else if (distance < maxWeightDistance.max)
                            volume.weight = 1f;
                        else
                            volume.weight = 1f - (distance - maxWeightDistance.max) / (maxWeightDistance.min - maxWeightDistance.max);

                    }

                    timer -= Time.deltaTime * Time.timeScale;
                    yield return null;
                }
            }

            public void Exit(MonoBehaviour context)
            {
                if (volume != null)
                {
                    context.StopCoroutine(corDamaging);

                    context.StartCoroutine(Delay(volumeProperties.OutDuration));
                    IEnumerator Delay(float delay)
                    {
                        var time = delay;
                        var initialWeight = volume.weight;
                        while (time > 0f)
                        {
                            volume.weight = time / delay * initialWeight;
                            time -= Time.deltaTime;
                            yield return null;
                        }

                        Destroy(volume.gameObject);
                    }
                }
            }

        }

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        int damage = 1;

        [SerializeField]
        DamageMode damageMode;

        [SerializeField, ShowIf("@damageMode==DamageMode.MinMax")]
        int maxDamage = 2;

        [SerializeField, ShowIf("@damageMode==DamageMode.MinMax")]
        MinMax maxDamageDistance = new(4,.5f);

        [SerializeField, SuffixLabel("sec",true)]
        float period = 0.5f;

        [SerializeField]
        DamageCanvas damageCanvasPrefab;

        [SerializeField]
        VolumeProperties volumeProperties;

        [SerializeField]
        MinMax maxWeightDistance = new(4,.5f);

        Collider2D col;
        List<int> layerMaskNumbers = new();
        List<EnteredHC> enteredHCs = new();


        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.isTrigger = true;
            layerMaskNumbers = layerMask.GetMemberLayerNumbers();
            
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (layerMaskNumbers.Contains(collision.gameObject.layer) && !DoesEnteredHCsHave(collision, out var hc))
                if (collision.TryGetComponentInFamily<HealthController>(out var otherHC))
                {
                    var enteredHC = new EnteredHC(collision, otherHC);
                    if (damageMode == DamageMode.Static)
                        enteredHC.Init(this, damage, period, damageCanvasPrefab, volumeProperties: volumeProperties, maxWeightDistance: maxWeightDistance);
                    else if (damageMode == DamageMode.MinMax)
                        enteredHC.Init(this, damage, maxDamage, maxDamageDistance, period, damageCanvasPrefab, volumeProperties: volumeProperties, maxWeightDistance: maxWeightDistance);
                    enteredHCs.Add(enteredHC);
                }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (DoesEnteredHCsHave(collision,out var hc))
            {
                hc.Exit(this);
                enteredHCs.Remove(hc);
            }
        }



        bool DoesEnteredHCsHave(Collider2D collision, out EnteredHC foundHC)
        {
            foreach (var hc in enteredHCs)
                if (hc.Col == collision)
                {
                    foundHC = hc;
                    return true;
                }

            foundHC = null;
            return false;
        }

    }
}
