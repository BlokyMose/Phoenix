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
                #region [Sprite]

                [HorizontalGroup]
                [SerializeField, LabelWidth(0.1f)]
                protected SpriteRenderer sr;
                public SpriteRenderer SR => sr;

                [HorizontalGroup]
                [SerializeField, LabelWidth(0.1f)]
                protected Sprite sprite;
                public Sprite Sprite => sprite;

                #endregion

                [FoldoutGroup("Overrides"), SerializeField, ToggleLeft]
                protected bool isMatchingElement = false;
                public bool IsMatchingElement => isMatchingElement;

                #region [Colord Idle]

                [FoldoutGroup("Overrides")]
                [HorizontalGroup("Overrides/1", width: 0.1f), LabelWidth(0.1f), SerializeField]
                protected bool overrideColorIdle = false;
                public bool OverrideColorIdle => overrideColorIdle;

                [HorizontalGroup("Overrides/1", width: 0.9f), LabelWidth(60f), SerializeField, LabelText("Idle")]
                protected Color colorIdle;
                public Color ColorIdle => colorIdle;

                #endregion

                #region [Color Damaged]

                [FoldoutGroup("Overrides")]
                [HorizontalGroup("Overrides/2", width: 0.1f), LabelWidth(0.1f), SerializeField]
                protected bool overrideColorDamaged = false;
                public bool OverrideColorDamaged => overrideColorDamaged;

                [HorizontalGroup("Overrides/2", width: 0.9f), LabelWidth(60f), SerializeField, LabelText("Damged")]
                protected Color colorDamaged;
                public Color ColorDamaged => colorDamaged;


                #endregion

                #region [Color Recovery]

                [FoldoutGroup("Overrides")]
                [HorizontalGroup("Overrides/3", width: 0.1f), LabelWidth(0.1f), SerializeField]
                protected bool overrideColorRecovery = false;
                public bool OverrideColorRecovery => overrideColorRecovery;

                [HorizontalGroup("Overrides/3", width: 0.9f), LabelWidth(60f), SerializeField, LabelText("Recovery")]
                protected Color colorRecovery;
                public Color ColorRecovery => colorRecovery;

                #endregion

                public SpriteSet(SpriteRenderer sr, bool isMatchingElement, Sprite sprite = null, Color? colorIdle = null, Color? colorDamaged = null, Color? colorRecovery = null)
                {
                    this.sr = sr;
                    this.isMatchingElement = isMatchingElement;
                    this.sprite = sprite;

                    if (colorIdle != null)
                    {
                        overrideColorIdle = true;
                        this.colorIdle = (Color)colorIdle;
                    }

                    if (colorDamaged != null)
                    {
                        overrideColorDamaged = true;
                        this.colorDamaged = (Color)colorDamaged;
                    }

                    if (colorRecovery != null)
                    {
                        overrideColorRecovery = true;
                        this.colorRecovery = (Color)colorRecovery;
                    }
                }

                public void ApplyIdle(Color defaultColorIdle, Element element = null)
                {
                    if (sr == null) return;
                    if (sprite != null)
                        sr.sprite = sprite;

                    if (isMatchingElement && element != null)
                        sr.color = element.Color;
                    else if (overrideColorIdle)
                        sr.color = colorIdle;
                    else
                        sr.color = defaultColorIdle;
                }

                public void ApplyDamaged(Color defaultColorDamaged, Element element = null)
                {
                    if (sr == null) return;
                    if (sprite != null)
                            sr.sprite = sprite;

                    if(isMatchingElement && element != null)
                        sr.color = element.ColorDim;
                    else if (overrideColorDamaged)
                        sr.color = colorDamaged;
                    else
                        sr.color = defaultColorDamaged;
                }

                public void ApplyRecovery(Color defaultColorRecovery, Element element = null)
                {
                    if (sr == null) return;
                    if (sprite != null)
                        sr.sprite = sprite;

                    if (isMatchingElement && element != null)
                        sr.color = element.ColorBright;
                    else if (overrideColorDamaged)
                        sr.color = colorRecovery;
                    else
                        sr.color = defaultColorRecovery;
                }

            }

            #region [Properties]

            [SerializeField]
            protected float atHealth;
            public float AtHealth => atHealth;

            [SerializeField]
            protected Color colorIdle;
            public virtual Color ColorIdle => colorIdle;

            [SerializeField]
            protected Color colorDamaged;
            public virtual Color ColorDamaged => colorDamaged;            
            
            [SerializeField]
            protected Color colorRecovery;
            public virtual Color ColorRecovery => colorRecovery;

            [SerializeField]
            protected List<SpriteSet> spriteSets = new();
            public List<SpriteSet> SpriteSets => spriteSets;

            #endregion

            protected Element currentElement;
            protected const float DELAY= 0.125f;

            public HealthStage(float atHealth, Color colorIdle, Color colorDamaged, Color colorRecovery, List<SpriteSet> spriteSets)
            {
                this.atHealth = atHealth;
                this.colorIdle = colorIdle;
                this.colorDamaged = colorDamaged;
                this.colorRecovery = colorRecovery;
                this.spriteSets = spriteSets;
            }

            public bool TryApply(float currentHealth)
            {
                if (currentHealth > atHealth) 
                    return false;

                ApplyIdle();
                return true;
            }

            public virtual void Init(ref Action<Element> onNewElement)
            {
                onNewElement += OnNewElement;
            }

            public virtual void Exit(ref Action<Element> onNewElement)
            {
                onNewElement -= OnNewElement;
            }

            protected virtual void OnNewElement(Element element)
            {
                currentElement = element;
            }

            public virtual IEnumerator PlayDamagedThenIdle(float delay = DELAY)
            {
                ApplyDamaged();
                yield return new WaitForSeconds(delay);
                ApplyIdle();
            }

            public virtual IEnumerator PlayRecoverThenIdle(float delay = DELAY)
            {
                ApplyRecovery();
                yield return new WaitForSeconds(delay);
                ApplyIdle();
            }

            public virtual void ApplyIdle()
            {
                foreach (var set in spriteSets)
                    set.ApplyIdle(colorIdle, currentElement);
            }

            public virtual void ApplyDamaged()
            {
                foreach (var set in spriteSets)
                    set.ApplyDamaged(colorDamaged, currentElement);
            }            
            
            public virtual void ApplyRecovery()
            {
                foreach (var set in spriteSets)
                    set.ApplyRecovery(colorRecovery, currentElement);
            }
        }

        #endregion

        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        [SerializeField]
        List<HealthStage> healthStages = new List<HealthStage>();

        [SerializeField]
        HealthBarUI healthBarUI;
        public HealthBarUI HealthBarUI => healthBarUI;

        [SerializeField]
        UnityEvent onDieAction = new UnityEvent();

        float health;
        public float Health => health;
        bool isUsingHealthStages = true;
        HealthStage currentHealthStage => healthStages[currentHealthStageIndex];
        int currentHealthStageIndex = 0;
        bool isDead = false;

        public Action<float> OnDamaged;
        public Action<float> OnRecovery;
        public Func<float,float> OnDepleteBarrier;
        public Action OnDie;


        private void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            Exit();
        }

        public void Init(int maxHealth)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;
        }

        public void Init(ShieldProperties properties)
        {
            maxHealth = properties.MaxHealth;
            health = maxHealth;
        }

        void Init()
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

            if (healthBarUI != null)
                healthBarUI.Init(this);

            if (isUsingHealthStages && TryGetComponent<ElementContainer>(out var elementContainer))
                foreach (var stage in healthStages)
                    stage.Init(ref elementContainer.OnNewElement);

            if (TryGetComponent<CameraFXController>(out var cameraFX))
                cameraFX.Init(this);
        }

        void Exit()
        {
            OnDie -= () => onDieAction.Invoke();

            if (healthBarUI != null)
                healthBarUI.Exit(this);

            if (isUsingHealthStages && TryGetComponent<ElementContainer>(out var elementContainer))
                foreach (var stage in healthStages)
                    stage.Exit(ref elementContainer.OnNewElement);

            if (TryGetComponent<CameraFXController>(out var cameraFX))
                cameraFX.Exit(this);

        }




        Coroutine corAnimatingHealth;
        public float ReceiveDamage(float damage)
        {
            if (damage <= 0) return health;

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
                    currentHealthStageIndex++;

                corAnimatingHealth = this.RestartCoroutine(currentHealthStage.PlayDamagedThenIdle());
            }

            if (health <= 0)
                Die();

            return health;

        }

        public void ReceiveRecovery(float recovery)
        {
            if (recovery <= 0) return;

            health += recovery;
            OnRecovery?.Invoke(recovery);

            if (isUsingHealthStages)
            {
                if (currentHealthStageIndex > 0 && currentHealthStage.AtHealth < health)
                    currentHealthStageIndex--;

                corAnimatingHealth = this.RestartCoroutine(currentHealthStage.PlayRecoverThenIdle());
            }

            if (health > maxHealth)
                health = maxHealth;

        }

        public void Die()
        {
            if (isDead) return;

            isDead = true;
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
