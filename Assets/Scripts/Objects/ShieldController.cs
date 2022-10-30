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
    /// [NOTE}
    /// Only supports VFX; add SpriteRenderer support later if needed
    /// </summary>
    public class ShieldController : MonoBehaviour
    {
        #region [Classes]

        [System.Serializable]
        public class HealthStage
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
            public bool OverrideColorIdle => overrideColorIdle;

            [HorizontalGroup("colorIdle", width: 0.95f)]
            [SerializeField, ColorUsage(true,true), EnableIf(nameof(overrideColorIdle))]
            Color colorIdle = new Color(1, 1, 1, 1);
            public Color ColorIdle => overrideColorIdle ? colorIdle : colorOriginal;


            [HorizontalGroup("colorDamaged", width: 0.05f)]
            [SerializeField, LabelWidth(0.1f)]
            bool overrideColorDamaged = false;
            public bool OverrideColorDamaged => overrideColorDamaged;

            [HorizontalGroup("colorDamaged", width: 0.95f)]
            [SerializeField, ColorUsage(true, true), EnableIf(nameof(overrideColorDamaged))]
            Color colorDamaged = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            public Color ColorDamaged => overrideColorDamaged ? colorDamaged : colorOriginal;

            [SerializeField]
            int vfxMode = 0;
            public int VFXMode => vfxMode;

            public HealthStage(float atHealth, Vector4 colorOriginal, Vector4 colorIdle, Vector4 colorDamaged,  int vfxMode)
            {
                this.atHealth = atHealth;
                this.colorOriginal = colorOriginal;
                this.colorIdle = colorIdle;
                this.colorDamaged = colorDamaged;
                this.vfxMode = vfxMode;
            }

            public HealthStage()
            {

            }
        }

        [Flags]
        public enum FollowMode
        {
            None = 0,
            Position = 1,
            Rotation = 2,
            Scale = 4,
        }

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        Transform targetFollow;

        [SerializeField]
        FollowMode followMode = FollowMode.Position;

        [SerializeField]
        HealthController healthController;

        [Header("VFX")]
        [SerializeField]
        VisualEffect vfx;

        [SerializeField, ListDrawerSettings(HideAddButton = true)]
        List<HealthStage> healthStages = new List<HealthStage>();


        #endregion

        #region [Vars: Data Handlers]

        const string COLOR = "color", MODE = "mode";
        Coroutine corDamageAnimation;
        HealthStage currentHealthStage => healthStages[currentHealthStageIndex];
        int currentHealthStageIndex = 0;

        #endregion

        private void Awake()
        {
            if (healthController==null)
                healthController = GetComponent<HealthController>();
            Init();
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

        void Init()
        {
            if (healthController != null)
            {
                if (healthStages.Count == 0)
                    AddHealthStageToList();
                else
                    ArrangeHealthStagesFromHighest();

                SetStagesColorOriginal(vfx.GetVector4(COLOR));

                healthController.OnDamaged += (damage) => { OnReceiveDamage(healthController.Health); };
                healthController.OnDie += Die;
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

        /// <summary>
        /// Change the VFX default color
        /// </summary>
        public void SetStagesColorOriginal(Color color)
        {
            foreach (var stage in healthStages)
            {
                stage.ColorOriginal = color;
            }
        }

        public void OnReceiveDamage(float health)
        {
            if (currentHealthStageIndex < healthStages.Count-1 && healthStages[currentHealthStageIndex+1].AtHealth >= health)
                currentHealthStageIndex++;

            vfx.SetInt(MODE, currentHealthStage.VFXMode);
            corDamageAnimation = this.RestartCoroutine(AnimatingDamagedColor());


            IEnumerator AnimatingDamagedColor()
            {
                vfx.SetVector4(COLOR, currentHealthStage.ColorDamaged);
                yield return new WaitForSeconds(0.125f);
                vfx.SetVector4(COLOR, currentHealthStage.ColorIdle);
            }
        }

        public void Die()
        {
            StopAllCoroutines();
        }

        #region [Methods: Inspector]

        [Button("Add Health Stage"), GUIColor("@Encore.Utility.ColorUtility.paleGreen")]
        void AddHealthStageToList()
        {
            var colorIdle = new Color();
            if (vfx!=null&& vfx.HasVector4(COLOR))
                colorIdle = vfx.GetVector4(COLOR);

            var maxHealth = (healthController != null) ? healthController.MaxHealth : 100f;
            healthStages.Add(new HealthStage(maxHealth, colorIdle, colorIdle, colorIdle, 0));
        }

        #endregion

    }
}
