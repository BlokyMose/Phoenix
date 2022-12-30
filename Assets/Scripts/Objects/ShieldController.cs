using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.VFX;
using Color = UnityEngine.Color;
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

        [Serializable]
        public class HealthStageVFX : HealthController.HealthStage
        {
            const string COLOR = "color", MODE = "mode";

            [SerializeField]
            bool isMatchingElement = true;

            [SerializeField]
            VisualEffect vfx;

            public override Color ColorIdle => isMatchingElement && currentElement != null ? currentElement.Color : colorIdle;
            public override Color ColorDamaged => isMatchingElement && currentElement != null ? currentElement.ColorDim : colorDamaged;
            public override Color ColorRecovery => isMatchingElement && currentElement != null ? currentElement.ColorBright : colorRecovery;

            [SerializeField]
            int vfxMode = 0;
            public int VFXMode => vfxMode;

            public HealthStageVFX(float atHealth, Vector4 colorIdle, Vector4 colorDamaged, Vector4 colorRecovery, List<SpriteSet> spriteSets,  int vfxMode) 
                : base(atHealth, colorIdle, colorDamaged, colorRecovery, spriteSets)
            {
                this.vfxMode = vfxMode;
            }

            public override void Init(ref Action<Element> onNewElement)
            {
                base.Init(ref onNewElement);
            }

            protected override void OnNewElement(Element element)
            {
                base.OnNewElement(element);
                ApplyIdle();
            }

            public override void ApplyIdle()
            {
                base.ApplyIdle();
                vfx.SetVector4(COLOR, ColorIdle);

            }

            public override void ApplyDamaged()
            {
                base.ApplyDamaged();
                vfx.SetVector4(COLOR, ColorDamaged);
                vfx.SetInt(MODE, VFXMode);

            }

            public override void ApplyRecovery()
            {
                base.ApplyRecovery();
                vfx.SetVector4(COLOR, ColorRecovery);
                vfx.SetInt(MODE, VFXMode);
            }
        }

        #endregion

        #region [Vars: Properties]

        Transform targetFollow;


        [SerializeField]
        FollowMode followMode = FollowMode.Position;

        [SerializeField, ListDrawerSettings(HideAddButton = true)]
        List<HealthStageVFX> healthStages = new();

        Collider2D col;

        #endregion

        #region [Vars: Data Handlers]

        Coroutine corDamageAnimation;
        HealthStageVFX currentHealthStage => healthStages[currentHealthStageIndex];
        int currentHealthStageIndex = 0;

        #endregion

        public Action OnDie;

        private void OnDestroy()
        {
            Exit();
        }

        public void Init(Transform targetFollow, ShieldProperties properties, List<SpriteRenderer> srs)
        {
            this.targetFollow = targetFollow;

            col = GetComponent<Collider2D>();

            if (TryGetComponent<HealthController>(out var healthController))
            {
                healthController.Init(properties);

                // Setup Health Stages
                if (healthStages.Count == 0)
                    AddNewHealthStageToList();
                else
                    ArrangeHealthStagesFromHighest();

                //vfx.SetVector4(COLOR, currentHealthStage.ColorIdle);

                // Setup delegates
                healthController.OnDamaged += (damage) => { OnReceiveDamage(healthController.Health); };
                healthController.OnDie += Die;
            }

            if (TryGetComponent<ElementContainer>(out var elementContainer))
            {
                elementContainer.Init(properties);
                foreach (var stage in healthStages)
                    stage.Init(ref elementContainer.OnNewElement);
            }

            foreach (var stage in healthStages)
                foreach (var sr in srs)
                    stage.SpriteSets.Add(new HealthController.HealthStage.SpriteSet(sr, true));
        }

        void Exit()
        {
            if (TryGetComponent<ElementContainer>(out var elementContainer))
            {
                foreach (var stage in healthStages)
                    stage.Exit(ref elementContainer.OnNewElement);
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
            var newList = new List<HealthStageVFX>();
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
            }

            corDamageAnimation = this.RestartCoroutine(AnimatingDamagedColor());


            IEnumerator AnimatingDamagedColor()
            {
                currentHealthStage.ApplyDamaged();
                yield return new WaitForSeconds(0.125f);
                currentHealthStage.ApplyIdle();
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

        #region [Methods: Inspector]

        [Button("Add Health Stage"), GUIColor("@Encore.Utility.ColorUtility.paleGreen")]
        void AddNewHealthStageToList()
        {
            var colorIdle = new Color();
            //if (vfx!=null&& vfx.HasVector4(COLOR))
            //    colorIdle = vfx.GetVector4(COLOR);

            healthStages.Add(new HealthStageVFX(
                atHealth: 100f, 
                colorIdle: colorIdle, 
                colorDamaged: Color.red, 
                colorRecovery: Color.green, 
                spriteSets: new(), 
                vfxMode: 0));
        }


        //[Button("Add Health Component"), HideIf(nameof(healthController))]
        //void AddHealthController()
        //{
        //    healthController = gameObject.GetComponent<HealthController>();
        //    if (healthController == null)
        //        healthController = gameObject.AddComponent<HealthController>();
        //}

        //[Button("Add Element Container"), HideIf(nameof(elementContainer))]
        //void AddElementContainer()
        //{
        //    elementContainer = gameObject.GetComponent<ElementContainer>();
        //    if (elementContainer == null)
        //        elementContainer = gameObject.AddComponent<ElementContainer>();
        //}

        #endregion

    }
}
