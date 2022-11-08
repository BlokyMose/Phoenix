using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField]
        HealthBarUIUnit healthBarUnitPrefab;

        [SerializeField]
        Transform healthBarParent;

        List<HealthBarUIUnit> healthBarUnits = new List<HealthBarUIUnit>();

        void Awake()
        {
        }

        public void Init(HealthController healthController)
        {
            int barCount = (int)(healthController.MaxHealth / 10);
            SetupHealthBarUnit(barCount);

            healthController.OnDamaged += ReceiveDamage;
            healthController.OnRecovery += ReceiveRecovery;
        }

        public void Init(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering += FillUnit;
        }

        public void Init(HealthBarrierController barrierController)
        {
            int barCount = (int)(barrierController.MaxHealth / 10);
            SetupHealthBarUnit(barCount);

            barrierController.OnDamaged += ReceiveDamage;
        }


        void SetupHealthBarUnit(int count)
        {
            for (int i = healthBarParent.childCount - 1; i >= 0; i--)
                Destroy(healthBarParent.GetChild(i).gameObject);

            for (int i = 0; i < count; i++)
            {
                var healthBarGO = Instantiate(healthBarUnitPrefab, healthBarParent);
                healthBarGO.name = i.ToString();
                healthBarUnits.Add(healthBarGO);
            }

            healthBarUnits[0].UseAlternateFirstSprite();
        }

        public void ReceiveDamage(float damage)
        {
            int barCount = (int)(damage / 10);
            for (int i = 0; i < barCount; i++)
            {
                EmptyOneFullUnit();
            }
        }

        public void ReceiveRecovery(float recovery)
        {
            int barCount = (int)(recovery / 10);
            for (int i = 0; i < barCount; i++)
            {
                FullOneFillingUnit();
            }
        }

        void EmptyOneFullUnit()
        {
            HealthBarUIUnit emptiedUnit = null;

            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                if (healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Full)
                {
                    healthBarUnits[i].Empty();
                    healthBarUnits[i].transform.SetSiblingIndex(healthBarUnits.Count - 1);
                    emptiedUnit = healthBarUnits[i];

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
                if (healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Filling)
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
                if (healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Empty || healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Filling)
                {
                    healthBarUnits[i].Fill(time,duration);
                    break;
                }

            }
        }
    }
}
