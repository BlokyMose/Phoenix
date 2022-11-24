using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.VFX;
using ColorUtility = Encore.Utility.ColorUtility;

namespace Phoenix
{
    /// <summary>
    /// [NOTE}<br></br>
    /// - Only supports VFX; add SpriteRenderer support later if needed <br></br>
    /// - ShieldController is meant to be added when in-game <br></br>
    /// </summary>

    [RequireComponent(typeof(Collider2D))]
    public class ShieldController : MonoBehaviour
    {
        #region [Classes]

        [System.Serializable]
        public class VFXHealthStage
        {
            [SerializeField]
            float atHealth = 100;
            public float AtHealth => atHealth;

            Color colorOriginal = new Color(1, 1, 1, 1);
            public Color ColorOriginal
            {
                get => colorOriginal;
                set { colorOriginal = value; }
            }

            [HorizontalGroup("colorIdle", width: 0.05f)]
            [SerializeField, LabelWidth(0.1f)]
            bool overrideColorIdle = false;
            public bool OverrideColorIdle
            {
                get => overrideColorIdle;
                set { overrideColorIdle = value; }
            }

            [HorizontalGroup("colorIdle", width: 0.95f)]
            [SerializeField, ColorUsage(true,true), EnableIf(nameof(overrideColorIdle))]
            Color colorIdle = new Color(1, 1, 1, 1);
            public Color ColorIdle => overrideColorIdle ? colorIdle : colorOriginal;


            [HorizontalGroup("colorDamaged", width: 0.05f)]
            [SerializeField, LabelWidth(0.1f)]
            bool overrideColorDamaged = false;
            public bool OverrideColorDamaged
            {
                get => overrideColorDamaged;
                set { overrideColorDamaged = value; }
            }

            [HorizontalGroup("colorDamaged", width: 0.95f)]
            [SerializeField, ColorUsage(true, true), EnableIf(nameof(overrideColorDamaged))]
            Color colorDamaged = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            public Color ColorDamaged => overrideColorDamaged ? colorDamaged : colorOriginal;

            [SerializeField]
            int vfxMode = 0;
            public int VFXMode => vfxMode;

            public VFXHealthStage(float atHealth, Vector4 colorOriginal, bool overrideColorIdle, Vector4 colorIdle, bool overrideColorDamaged, Vector4 colorDamaged,  int vfxMode)
            {
                this.atHealth = atHealth;
                this.colorOriginal = colorOriginal;
                this.overrideColorIdle = overrideColorIdle;
                this.colorIdle = colorIdle;
                this.overrideColorDamaged = overrideColorDamaged;
                this.colorDamaged = colorDamaged;
                this.vfxMode = vfxMode;
            }
        }



        #endregion

        #region [Vars: Properties]

        Transform targetFollow;

        [SerializeField, HideInInspector]
        HealthController healthController;

        [SerializeField, HideInInspector]
        ElementContainer elementContainer;

        [SerializeField]
        FollowMode followMode = FollowMode.Position;


        [Header("VFX")]
        [SerializeField]
        VisualEffect vfx;

        [SerializeField]
        bool useElementAsColorIdle = true;

        [SerializeField, ListDrawerSettings(HideAddButton = true)]
        List<VFXHealthStage> healthStages = new List<VFXHealthStage>();

        [Header("SpriteRenderers")]
        [SerializeField]
        List<SpriteRenderer> srs = new List<SpriteRenderer>();


        Collider2D col;

        #endregion

        #region [Vars: Data Handlers]

        const string COLOR = "color", MODE = "mode";
        Coroutine corDamageAnimation;
        VFXHealthStage currentHealthStage => healthStages[currentHealthStageIndex];
        int currentHealthStageIndex = 0;

        #endregion

        public Action OnDie;

        public void Init(Transform targetFollow, ShieldProperties properties, List<SpriteRenderer> srs)
        {
            this.targetFollow = targetFollow;

            this.srs = srs;

            #region [Get/Add components]

            col = GetComponent<Collider2D>();

            if (healthController == null)
            {
                healthController = GetComponent<HealthController>();
                if (healthController == null && properties != null)
                    healthController = gameObject.AddComponent<HealthController>();
            }

            if (elementContainer == null)
            {
                elementContainer = GetComponent<ElementContainer>();
                if (elementContainer == null && properties != null)
                    elementContainer = gameObject.AddComponent<ElementContainer>();
            }

            #endregion

            #region [Setup components]

            if (healthController != null)
            {
                healthController.Init(properties);

                // Setup Health Stahes
                if (healthStages.Count == 0)
                    AddNewHealthStageToList();
                else
                    ArrangeHealthStagesFromHighest();

                // Setup the HealthStages' colors
                if (useElementAsColorIdle && elementContainer != null)
                {
                    SetStagesColorOriginalAndCannotOverrideColodIdle(elementContainer.Element.Color);
                }
                else
                {
                    SetStagesColorOriginal(vfx.GetVector4(COLOR));
                }

                vfx.SetVector4(COLOR, currentHealthStage.ColorIdle);

                // Setup delegates
                healthController.OnDamaged += (damage) => { OnReceiveDamage(healthController.Health); };
                healthController.OnDie += Die;

            }

            if (elementContainer != null)
            {
                elementContainer.Init(properties);
                elementContainer.OnNewElement += OnNewElement; //TODO 
            }
            
            void OnNewElement(Element element)
            {
                SetStagesColorOriginalAndCannotOverrideColodIdle(element.Color);
            }

            #endregion

            void SetStagesColorOriginalAndCannotOverrideColodIdle(Color color)
            {
                foreach (var stage in healthStages)
                {
                    stage.ColorOriginal = color;
                    stage.OverrideColorIdle = false;
                }
                SetColor(currentHealthStage.ColorIdle);
            }

            void SetStagesColorOriginal(Color color)
            {
                foreach (var stage in healthStages)
                {
                    stage.ColorOriginal = color;
                }
                SetColor(currentHealthStage.ColorIdle);
            }
        }


        void FixedUpdate()
        {
            SyncToTargetTransform();
        }

        void SyncToTargetTransform()
        {
            if (targetFollow == null) 
                return;

            if (followMode.HasFlag(FollowMode.Position))
                transform.position = targetFollow.position;
            if (followMode.HasFlag(FollowMode.Rotation))
                transform.localEulerAngles = targetFollow.localEulerAngles;
            if (followMode.HasFlag(FollowMode.Scale))
                transform.localScale = targetFollow.localScale;
        }

        void ArrangeHealthStagesFromHighest()
        {
            var newList = new List<VFXHealthStage>();
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

        public void OnReceiveDamage(float health)
        {
            if (currentHealthStageIndex < healthStages.Count-1 && healthStages[currentHealthStageIndex+1].AtHealth >= health)
            {
                currentHealthStageIndex++;
                vfx.SetInt(MODE, currentHealthStage.VFXMode);
            }

            corDamageAnimation = this.RestartCoroutine(AnimatingDamagedColor());


            IEnumerator AnimatingDamagedColor()
            {
                SetColor(currentHealthStage.ColorDamaged);
                yield return new WaitForSeconds(0.125f);
                SetColor(currentHealthStage.ColorIdle);
            }
        }

        public void Die()
        {
            StopAllCoroutines();

            StartCoroutine(AnimatingVFXDie());
            IEnumerator AnimatingVFXDie()
            {
                col.enabled = false;
                // TODO: Do VFXDie
                yield return new WaitForSeconds(0f);

                OnDie?.Invoke();
                Destroy(gameObject);
            }
        }

        void SetColor(Color color)
        {
            vfx.SetVector4(COLOR, color);
            foreach (var sr in srs)
                sr.color = color;
        }

        #region [Methods: Inspector]

        [Button("Add Health Stage"), GUIColor("@Encore.Utility.ColorUtility.paleGreen")]
        void AddNewHealthStageToList()
        {
            var colorIdle = new Color();
            if (vfx!=null&& vfx.HasVector4(COLOR))
                colorIdle = vfx.GetVector4(COLOR);

            var maxHealth = (healthController != null) ? healthController.MaxHealth : 100f;
            healthStages.Add(new VFXHealthStage(maxHealth, colorIdle, true, colorIdle, true, colorIdle, 0));
        }


        [Button("Add Health Component"), HideIf(nameof(healthController))]
        void AddHealthController()
        {
            healthController = gameObject.GetComponent<HealthController>();
            if (healthController == null)
                healthController = gameObject.AddComponent<HealthController>();
        }

        [Button("Add Element Container"), HideIf(nameof(elementContainer))]
        void AddElementContainer()
        {
            elementContainer = gameObject.GetComponent<ElementContainer>();
            if (elementContainer == null)
                elementContainer = gameObject.AddComponent<ElementContainer>();
        }

        #endregion

    }
}
