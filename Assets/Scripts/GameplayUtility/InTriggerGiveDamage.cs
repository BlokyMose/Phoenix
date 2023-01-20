using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Collider2D))]
    public class InTriggerGiveDamage : MonoBehaviour
    {
        public enum DamageMode { Static, MinMax }

        [System.Serializable]
        public struct MinMax
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

            public Collider2D Col { get => col; }
            public HealthController HealthController { get => healthController; }
            public Coroutine CorDamaging { get => corDamaging; }

            public EnteredHC(Collider2D col, HealthController healthController)
            {
                this.col = col;
                this.healthController = healthController;
            }

            public void Init(MonoBehaviour context, float damage, float period, DamageCanvas damageCanvasPrefab = null, Color? damageColor = null)
            {
                corDamaging = context.RestartCoroutine(Damaging());

                IEnumerator Damaging()
                {
                    var timer = period;

                    while (true)
                    {
                        if (timer < 0)
                        {
                            timer = period;
                            healthController.ReceiveDamage(damage);
                            if (context.TryInstantiate(damageCanvasPrefab, out var damageCanvas))
                            {
                                damageCanvas.transform.position = healthController.transform.position;
                                damageCanvas.Init(damage, damageColor);
                            }
                        }

                        timer -= Time.deltaTime * Time.timeScale;
                        yield return null;
                    }
                }
            }

            public void Init(MonoBehaviour context, float damage, float maxDamage, MinMax maxDamageDistance, float period, DamageCanvas damageCanvasPrefab = null, Color? damageColor = null)
            {
                corDamaging = context.RestartCoroutine(Damaging());

                IEnumerator Damaging()
                {
                    var timer = period;
                    int maxDamageMargin = Mathf.RoundToInt(maxDamage - damage);
                    int extraDamage;
                    float distance;

                    while (true)
                    {
                        if (timer < 0)
                        {
                            timer = period;

                            distance = Vector2.Distance(healthController.transform.position, context.transform.position);
                            if (distance > maxDamageDistance.min)
                                extraDamage = 0;
                            else if (distance < maxDamageDistance.max)
                                extraDamage = maxDamageMargin;
                            else
                                extraDamage = Mathf.RoundToInt((1-(distance - maxDamageDistance.max) / (maxDamageDistance.min - maxDamageDistance.max)) * maxDamageMargin);

                            healthController.ReceiveDamage(damage + extraDamage);
                            if (context.TryInstantiate(damageCanvasPrefab, out var damageCanvas))
                            {
                                damageCanvas.transform.position = healthController.transform.position;
                                damageCanvas.Init(damage+extraDamage, damageColor);
                            }
                        }

                        timer -= Time.deltaTime * Time.timeScale;
                        yield return null;
                    }
                }
            }

            public void Exit(MonoBehaviour monoBehaviour)
            {
                monoBehaviour.StopCoroutine(corDamaging);
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
        MinMax maxDamageDistance = new(2, 1);

        [SerializeField, SuffixLabel("sec",true)]
        float period = 0.5f;

        [SerializeField]
        DamageCanvas damageCanvasPrefab;

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
                        enteredHC.Init(this, damage, period, damageCanvasPrefab);
                    else if (damageMode == DamageMode.MinMax)
                        enteredHC.Init(this, damage, maxDamage, maxDamageDistance, period, damageCanvasPrefab);
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
