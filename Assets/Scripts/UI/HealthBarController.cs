using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField]
        HealthBarUnit healthBarUnitPrefab;

        [SerializeField]
        Transform healthBarParent;

        List<HealthBarUnit> healthBarUnits = new List<HealthBarUnit>();

        public void Init(HealthController healthController)
        {
            for (int i = healthBarParent.childCount - 1; i >= 0; i--)
                Destroy(healthBarParent.GetChild(i).gameObject);

            int barCount = (int)(healthController.MaxHealth / 10);
            for (int i = 0; i < barCount; i++)
            {
                var healthBarGO = Instantiate(healthBarUnitPrefab, healthBarParent);
                healthBarGO.name = i.ToString();
                healthBarUnits.Add(healthBarGO);
            }

            healthController.OnDamaged += ReceiveDamage;
        }

        public void Init(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering += FillUnit;
        }

        public void ReceiveDamage(float damage)
        {
            if (damage > 0)
            {
                int barCount = (int)(damage / 10);
                for (int i = 0; i < barCount; i++)
                {
                    EmptyOneFullUnit();
                }
            }
            else if (damage < 0)
            {
                int barCount = (int)(-damage / 10);
                for (int i = 0; i < barCount; i++)
                {
                    FullOneFillingUnit();
                }
            }

            
        }

        void EmptyOneFullUnit()
        {
            HealthBarUnit emptiedUnit = null;

            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                if (healthBarUnits[i].Status == HealthBarUnit.FillStatus.Full)
                {
                    healthBarUnits[i].Empty();
                    emptiedUnit = healthBarUnits[i];
                    healthBarUnits[i].transform.SetSiblingIndex(healthBarUnits.Count - 1);

                    break;
                }
            }

            if(emptiedUnit != null)
            {
                healthBarUnits.Remove(emptiedUnit);
                healthBarUnits.Add(emptiedUnit);
            }
        }

        void FullOneFillingUnit()
        {
            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                if (healthBarUnits[i].Status == HealthBarUnit.FillStatus.Filling)
                {
                    healthBarUnits[i].Full();
                    break;
                }
            }
        }

        void FillUnit(float time, float duration)
        {
            for (int i = 0; i < healthBarUnits.Count; i++)
            {
                if (healthBarUnits[i].Status == HealthBarUnit.FillStatus.Empty || healthBarUnits[i].Status == HealthBarUnit.FillStatus.Filling)
                {
                    healthBarUnits[i].Fill(time,duration);
                    break;
                }

            }
        }
    }
}
