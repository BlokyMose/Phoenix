using Encore.Utility;
using Sirenix.OdinInspector;
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

        [SerializeField, LabelText("HP per Bar")]
        int hpPerBar = 10;

        List<HealthBarUIUnit> healthBarUnits = new List<HealthBarUIUnit>();

        Func<float> GetHealth;

        public void Init(HealthController healthController)
        {
            int barCount = (int)(healthController.MaxHealth / hpPerBar);
            SetupHealthBarUnit(barCount);
            GetHealth += () => { return healthController.Health; };
            healthController.OnDamaged += ReceiveDamage;
            healthController.OnRecovery += ReceiveRecovery;
        }

        public void Exit(HealthController healthController)
        {
            GetHealth -= () => { return healthController.Health; };
            healthController.OnDamaged -= ReceiveDamage;
            healthController.OnRecovery -= ReceiveRecovery;
        }

        public void Init(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering += FillUnit;
        }

        public void Exit(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering -= FillUnit;
        }

        public void Init(HealthBarrierController barrierController)
        {
            GetHealth += () => barrierController.Health;
            int barCount = (int)(barrierController.MaxHealth / hpPerBar);
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
            int barCount = (int)(damage / hpPerBar);
            for (int i = 0; i < barCount; i++)
                EmptyOneFullUnit();

            float leftoverDamage = damage % hpPerBar;
            DecreaseUnit(leftoverDamage);
        }

        public void ReceiveRecovery(float recovery)
        {
            int barCount = (int)(recovery / hpPerBar);

            for (int i = 0; i < barCount; i++)
                FullOneFillingUnit();
        }

        void DecreaseUnit(float decreaseAmount)
        {
            if (decreaseAmount <= 0) return;

            var _decreaseAmount = decreaseAmount;
            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                var healthBar = healthBarUnits[i];
                if (healthBar.Status == HealthBarUIUnit.FillStatus.Decreased ||
                    healthBar.Status == HealthBarUIUnit.FillStatus.Full)
                {
                    var currentBarHP = GetHealth() % hpPerBar;
                    if (currentBarHP == 0) currentBarHP = hpPerBar;
                    currentBarHP -= _decreaseAmount;

                    if (currentBarHP == 0)
                    {
                        healthBar.Empty();
                        break;
                    }
                    else if (currentBarHP < 0)
                    {
                        healthBar.Empty();
                        _decreaseAmount -= (GetHealth() % hpPerBar);
                    }
                    else
                    {
                        healthBar.Decrease(currentBarHP, hpPerBar);
                        break;
                    }
                }


            }
        }

        void EmptyOneFullUnit()
        {
            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                if (healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Full)
                {
                    healthBarUnits[i].Empty();
                    healthBarUnits[i].transform.SetSiblingIndex(healthBarUnits.Count - 1);
                    var emptiedUnit = healthBarUnits[i];
                    healthBarUnits.Remove(emptiedUnit);
                    healthBarUnits.Add(emptiedUnit);
                    break;
                }
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

        void FillUnit(float recovery, float time, float duration)
        {
            int barCount = (int)(recovery / hpPerBar);

            for (int i = 0; i < healthBarUnits.Count; i++)
            {
                if (healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Empty || healthBarUnits[i].Status == HealthBarUIUnit.FillStatus.Filling)
                {
                    healthBarUnits[i].Fill(time,duration);
                    
                    barCount--;
                    if (barCount <= 0)
                        break;
                }

            }
        }
    }
}
