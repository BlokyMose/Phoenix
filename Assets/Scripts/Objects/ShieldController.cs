using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    public class ShieldController : MonoBehaviour, IHealth
    {
        #region [Classes]

        [System.Serializable]
        public class HealthStage
        {
            [SerializeField]
            float atHealth = 100;
            public float AtHealth => atHealth;

            [SerializeField, ColorUsage(true,true)]
            Color color;
            public Color Color => color;

            [SerializeField]
            int vfxMode = 0;
            public int VFXMode => vfxMode;

            public HealthStage(float atHealth, Vector4 color, int vfxMode)
            {
                this.atHealth = atHealth;
                this.color = color;
                this.vfxMode = vfxMode;
            }
        }

        #endregion

        [Header("Health")]
        [SerializeField]
        float maxHealth = 100;
        public float MaxHealth => maxHealth;

        float health;
        public float Health => health;

        [Header("VFX")]
        [SerializeField]
        VisualEffect vfx;

        [SerializeField]
        List<HealthStage> healthStages = new List<HealthStage>();

        [SerializeField]
        bool overrideIdleColor = false;

        [SerializeField, ColorUsage(true, true), ShowIf("@"+nameof(overrideIdleColor))]
        Color colorIdle;

        [SerializeField, ColorUsage(true, true)]
        Color colorDamaged;

        const string COLOR = "color", MODE = "mode";
        Coroutine corDamageAnimation;

        private void Awake()
        {
            if(!overrideIdleColor)
                colorIdle = vfx.GetVector4(COLOR);
        }

        public void ReceiveDamage(float damage)
        {
            health -= damage;
            if (health < 0) 
                Die();

            corDamageAnimation = this.RestartCoroutine(AnimatingDamagedColor());
            IEnumerator AnimatingDamagedColor()
            {
                vfx.SetVector4(COLOR, colorDamaged);
                yield return new WaitForSeconds(0.25f);
                vfx.SetVector4(COLOR, colorIdle);
            }
        }

        public void Die()
        {

        }
    }
}
