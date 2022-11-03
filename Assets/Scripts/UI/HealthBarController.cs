using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField]
        GameObject healthBarPrefab;

        [SerializeField]
        Transform healthBarParent;

        List<GameObject> healthBars = new List<GameObject>();

        float health;

        public void Init(HealthController healthController)
        {
            for (int i = healthBarParent.childCount - 1; i >= 0; i--)
                Destroy(healthBarParent.GetChild(i).gameObject);

            health = healthController.MaxHealth;
            int barCount = (int)(healthController.MaxHealth / 10);
            for (int i = 0; i < barCount; i++)
            {
                var healthBarGO = Instantiate(healthBarPrefab, healthBarParent);
                healthBars.Add(healthBarGO);
            }

            healthController.OnDamaged += ReceiveDamage;
        }

        public void ReceiveDamage(float damage = 10)
        {
            health -= damage;
            Destroy(healthBars.GetLast().gameObject);
            healthBars.RemoveLast();
        }
    }
}
