using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Phoenix.HealthController;
using static Phoenix.ShieldController;

namespace Phoenix
{
    public class HealthController : MonoBehaviour
    {

        #region [Classes]

        [Serializable]
        public class HealthStage
        {
            [Serializable]
            public class SpriteSet
            {
                [HorizontalGroup]
                [SerializeField, LabelWidth(0.1f)]
                SpriteRenderer sr;
                public SpriteRenderer SR => sr;

                [HorizontalGroup]
                [SerializeField, LabelWidth(0.1f)]
                Sprite sprite;
                public Sprite Sprite => sprite;

                [FoldoutGroup("Overrides")]
                [HorizontalGroup("Overrides/1", width: 0.25f), LabelWidth(60f), SerializeField, LabelText("Idle")]
                bool overrideColorIdle = false;
                public bool OverrideColorIdle => overrideColorIdle;

                [HorizontalGroup("Overrides/1", width: 0.75f),LabelWidth(0.1f), SerializeField]
                Color colorIdle;
                public Color ColorIdle => colorIdle;

                [FoldoutGroup("Overrides")]
                [HorizontalGroup("Overrides/2", width: 0.25f), LabelWidth(60f), SerializeField, LabelText("Damaged")]
                bool overrideColorDamaged = false;
                public bool OverrideColorDamaged => overrideColorDamaged;

                [HorizontalGroup("Overrides/2", width: 0.75f), LabelWidth(0.1f), SerializeField]
                Color colorDamaged;
                public Color ColorDamaged => colorDamaged;

                public SpriteSet(SpriteRenderer sr, Sprite sprite, bool overrideColorIdle, Color colorIdle, bool overrideColorDamaged, Color colorDamaged)
                {
                    this.sr = sr;
                    this.sprite = sprite;
                    this.overrideColorIdle = overrideColorIdle;
                    this.colorIdle = colorIdle;
                    this.overrideColorDamaged = overrideColorDamaged;
                    this.colorDamaged = colorDamaged;
                }

                public void ApplyIdle(Color defaultColorIdle)
                {
                    if (sprite != null)
                        sr.sprite = sprite;
                    if (overrideColorIdle)
                        sr.color = colorIdle;
                    else
                        sr.color = defaultColorIdle;
                }

                public void ApplyDamaged(Color defaultColorDamaged)
                {
                    if (sprite != null)
                        sr.sprite = sprite;
                    if (overrideColorDamaged)
                        sr.color = colorDamaged;
                    else
                        sr.color = defaultColorDamaged;
                }
            }

            [SerializeField]
            float atHealth;
            public float AtHealth => atHealth;

            [SerializeField]
            Color colorIdle;
            public Color ColorIdle => colorIdle;

            [SerializeField]
            Color colorDamaged;
            public Color ColorDamaged => colorDamaged;

            [SerializeField]
            List<SpriteSet> spriteSets = new List<SpriteSet>();
            public List<SpriteSet> SpriteSets => spriteSets;

            public bool TryApply(float currentHealth)
            {
                if (currentHealth > atHealth) 
                    return false;

                ApplyIdle();
                return true;
            }

            public void ApplyIdle()
            {
                foreach (var set in spriteSets)
                    set.ApplyIdle(colorIdle);
            }

            public void ApplyDamaged()
            {
                foreach (var set in spriteSets)
                    set.ApplyDamaged(colorDamaged);
            }            
            
            public void ApplyRecovery()
            {
                //foreach (var set in spriteSets)
                //    set.ApplyDamaged(colorDamaged);
            }
        }

        #endregion

        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        [SerializeField]
        List<HealthStage> healthStages = new List<HealthStage>();

        [SerializeField]
        UnityEvent onDieAction;

        float health;
        public float Health => health;
        bool isUsingHealthStages = true;
        HealthStage currentHealthStage => healthStages[currentHealthStageIndex];
        int currentHealthStageIndex = 0;

        public Action<float> OnDamaged;
        public Action<float> OnRecovery;
        public Func<float,float> OnDepleteBarrier;
        public Action OnDie;


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            health = maxHealth;
            if (healthStages.Count == 0)
            {
                isUsingHealthStages = false;
            }
            else
            {
                ArrangeHealthStagesFromHighest();
                healthStages[0].ApplyIdle();
            }

            OnDie += () => onDieAction.Invoke();

            var playerBrain = GetComponent<PlayerBrain>();
            if (playerBrain != null)
                playerBrain.ConnectToHealthBar(this);
        }

        public void Init(ShieldProperties properties)
        {
            maxHealth = properties.MaxHealth;
            health = maxHealth;
        }

        Coroutine corAnimatingHealth;
        public void ReceiveDamage(float damage)
        {
            if (damage <= 0) return;

            if (OnDepleteBarrier == null)
            {
                OnDamaged?.Invoke(damage);
                health -= damage;
            }

            else
            {
                var excessDamage = OnDepleteBarrier(damage);
                if (excessDamage > 0)
                {
                    OnDamaged?.Invoke(excessDamage);
                    health -= damage;
                }
            }

            if (isUsingHealthStages)
            {
                if (currentHealthStageIndex < healthStages.Count - 1 && healthStages[currentHealthStageIndex + 1].AtHealth >= health)
                {
                    currentHealthStageIndex++;
                    currentHealthStage.ApplyIdle();
                }

                corAnimatingHealth = this.RestartCoroutine(AnimatingDamaged());
                IEnumerator AnimatingDamaged()
                {
                    currentHealthStage.ApplyDamaged();
                    yield return new WaitForSeconds(0.125f);
                    currentHealthStage.ApplyIdle();
                }
            }

            if (health <= 0)
                Die();

        }

        public void ReceiveRecovery(float recovery)
        {
            if (recovery <= 0) return;

            health += recovery;
            OnRecovery?.Invoke(recovery);

            if (isUsingHealthStages)
            {
                if (currentHealthStageIndex > 0 && healthStages[currentHealthStageIndex - 1].AtHealth < health)
                {
                    currentHealthStageIndex--;
                    currentHealthStage.ApplyIdle();
                }

                corAnimatingHealth = this.RestartCoroutine(AnimatingDamaged());
                IEnumerator AnimatingDamaged()
                {
                    currentHealthStage.ApplyRecovery();
                    yield return new WaitForSeconds(0.125f);
                    currentHealthStage.ApplyIdle();
                }
            }

            if (health > maxHealth)
                health = maxHealth;
        }

        public void Die()
        {
            OnDie.Invoke();
        }

        void ArrangeHealthStagesFromHighest()
        {
            var newList = new List<HealthStage>();
            for (int i = healthStages.Count - 1; i >= 0; i--)
            {
                var top = healthStages[0];
                var topIndex = 0;
                for (int k = 0; k < healthStages.Count; k++)
                {
                    if (healthStages[k].AtHealth >= top.AtHealth)
                    {
                        top = healthStages[k];
                        topIndex = k;
                    }
                }

                healthStages.RemoveAt(topIndex);
                newList.Add(top);
            }
            healthStages = newList;
        }

    }
}
