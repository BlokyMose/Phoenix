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
        [Serializable]

        public class HealthBarParent
        {
            [SerializeField]
            HealthBarUIUnit healthBarUnitPrefab;

            [SerializeField]
            Transform transform;

            [HorizontalGroup("Color", width: .5f)]
            [SerializeField, ToggleLeft]
            bool isOverrideColor = false;

            [HorizontalGroup("Color", width:.5f)]
            [SerializeField, EnableIf(nameof(isOverrideColor)), LabelWidth(0.1f)]
            Color overrideColor;

            public HealthBarUIUnit HealthBarUnitPrefab { get => healthBarUnitPrefab; }
            public Transform Transform { get => transform; }
            public bool IsOverrideColor { get => isOverrideColor; }
            public Color OverrideColor { get => overrideColor; }


            public HealthBarUIUnit CreateBar(string name)
            {
                var healthBar = Instantiate(healthBarUnitPrefab, transform);
                healthBar.name = name;
                if (isOverrideColor)
                    healthBar.Init(overrideColor);

                return healthBar;
            }
        }

        [SerializeField]
        List<HealthBarParent> healthBarParents = new();

        [SerializeField]
        int healthPerBar = 10;

        [SerializeField]
        int barCountPerParent = 5;

        List<HealthBarUIUnit> healthBarUnits = new();

        Func<float> GetHealth;

        public void Init(HealthController healthController)
        {
            GetHealth += () => { return healthController.Health; };
            int barCount = Mathf.CeilToInt(healthController.MaxHealth / healthPerBar);
            SetupHealthBarUnit(barCount);
            healthController.OnDamaged += ReceiveDamage;
            healthController.OnRecovery += ReceiveRecovery;
        }

        public void Init(HealthBarrierController barrierController)
        {
            GetHealth += () => barrierController.Health;
            int barCount = Mathf.CeilToInt(barrierController.MaxHealth / healthPerBar);
            SetupHealthBarUnit(barCount);
            barrierController.OnDamaged += ReceiveDamage;
        }

        public void Init(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering += FillUnit;
        }

        public void Exit(HealthController healthController)
        {
            GetHealth -= () => { return healthController.Health; };
            healthController.OnDamaged -= ReceiveDamage;
            healthController.OnRecovery -= ReceiveRecovery;
        }

        public void Exit(HealthBarrierController barrierController)
        {
            GetHealth -= () => barrierController.Health;
            barrierController.OnDamaged -= ReceiveDamage;
        }

        public void Exit(RecoveryController recoveryController)
        {
            recoveryController.OnRecovering -= FillUnit;
        }

        void SetupHealthBarUnit(int count)
        {
            foreach (var parent in healthBarParents)
                for (int i = parent.Transform.childCount - 1; i >= 0; i--)
                    Destroy(parent.Transform.GetChild(i).gameObject);


            if (count == 0)
                healthBarUnits.Add(healthBarParents[0].CreateBar("0")); 
            else
                for (int i = 0; i < count; i++)
                    healthBarUnits.Add(healthBarParents.GetAt(Mathf.FloorToInt(i / barCountPerParent), 0).CreateBar(i.ToString()));

            var lastBarFillAmount = GetHealth() % healthPerBar / healthPerBar;
            if (lastBarFillAmount == 0f) lastBarFillAmount = 1f;
            healthBarUnits.GetLast().SetFillAmount(lastBarFillAmount);
            healthBarUnits[0].UseAlternateFirstSprite();

        }

        public void ReceiveDamage(float damage)
        {
            var barCount = Mathf.FloorToInt(damage / healthPerBar);
            for (int i = 0; i < barCount; i++)
                EmptyOneFullUnit();

            var leftoverDamage = Mathf.FloorToInt(damage % healthPerBar);
            DecreaseUnit(leftoverDamage);
        }

        public void ReceiveRecovery(float recovery)
        {
            int barCount = (int)(recovery / healthPerBar);

            for (int i = 0; i < barCount; i++)
                FullOneFillingUnit();
        }

        void DecreaseUnit(int decreaseAmount)
        {
            if (decreaseAmount <= 0) return;

            var _decreaseAmount = decreaseAmount;
            var currentBarHP = Mathf.FloorToInt(GetHealth() % healthPerBar);
            if (currentBarHP == 0) currentBarHP = healthPerBar;

            for (int i = healthBarUnits.Count - 1; i >= 0; i--)
            {
                var healthBar = healthBarUnits[i];
                if (healthBar.Status == HealthBarUIUnit.FillStatus.Decreased ||
                    healthBar.Status == HealthBarUIUnit.FillStatus.Full)
                {
                    currentBarHP -= _decreaseAmount;

                    if (currentBarHP == 0)
                    {
                        healthBar.Empty();
                        break;
                    }
                    else if (currentBarHP < 0)
                    {
                        healthBar.Empty();
                        _decreaseAmount = -currentBarHP;
                        currentBarHP = healthPerBar;
                    }
                    else
                    {
                        healthBar.Decrease(currentBarHP, healthPerBar);
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
                    if (i%barCountPerParent != 0)
                    {
                        healthBarUnits[i].transform.SetSiblingIndex(healthBarUnits.Count - 1);
                        healthBarUnits.Move(healthBarUnits[i], barCountPerParent - 1);
                    }
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
            int barCount = (int)(recovery / healthPerBar);

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
