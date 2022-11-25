using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    /// <summary>
    /// 
    /// [GENERAL NOTES]<br></br>
    /// - When using this after a collision, Objects that are to be damaged may move away so fast before a damage is applicable.<br></br>
    /// - The faster the expansionSpeed, the higher the chance to apply damage to an object that is moving away.<br></br>
    /// 
    /// </summary>

    [RequireComponent(typeof(Collider2D))]
    public class BombController : MonoBehaviour
    {
        [SerializeField, Range(1, 1000), SuffixLabel("/ 10,000")]
        int expansionSpeed = 500;

        [SerializeField]
        float damage = 10;

        Collider2D col;
        Dictionary<Collider2D,HealthController> allGOsToDamage = new Dictionary<Collider2D,HealthController>();
        Dictionary<Collider2D,HealthController> allGOsDamaged = new Dictionary<Collider2D,HealthController>();
        float currentExpansion = 0;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        void Start()
        {
            StartCoroutine(ExpandingAndDamaging());
            IEnumerator ExpandingAndDamaging()
            {
                yield return null;

                var damageDistanceMax = col.bounds.size.x > col.bounds.size.y
                    ? col.bounds.size.x / 2f
                    : col.bounds.size.y / 2f;

                while (currentExpansion < 1f)
                {
                    var damageDistance = currentExpansion / 1f * damageDistanceMax;
                    foreach (var goToDamage in allGOsToDamage)
                    {

                        var otherHealthController = goToDamage.Value;
                        if (otherHealthController == null || allGOsDamaged.ContainsKey(goToDamage.Key))
                            break;

                        var otherClosestPoint = goToDamage.Key.ClosestPoint(transform.position);
                        Debug.Log(Vector2.Distance(otherClosestPoint, transform.position)+" : "+damageDistance);
                        if (Vector2.Distance(otherClosestPoint, transform.position) < damageDistance)
                        {
                            otherHealthController.ReceiveDamage(damage);
                            allGOsDamaged.Add(goToDamage.Key, goToDamage.Value);
                        }
                    }

                    currentExpansion += expansionSpeed / 10000f;
                    yield return null;
                }

                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            var healthController = collision.GetComponentInFamily<HealthController>();

            if (healthController != null)
            {
                if (!allGOsToDamage.ContainsKey(collision))
                    allGOsToDamage.Add(collision, healthController);
                else 
                    allGOsToDamage[collision] = healthController;
            }

            
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (allGOsToDamage.ContainsKey(collision))
                allGOsToDamage[collision] = null;
        }
    }
}
