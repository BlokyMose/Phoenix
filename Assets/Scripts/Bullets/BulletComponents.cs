using FunkyCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Phoenix
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class BulletComponents : MonoBehaviour
    {
        SpriteRenderer sr;
        public SpriteRenderer SR => sr;

        [SerializeField]
        List<Light2D> lights = new List<Light2D>();
        public List<Light2D> Lights => lights;

        [SerializeField]
        VisualEffect vfx;
        public VisualEffect VFX => vfx;        

        public enum InvokeMode { Always, BeforeLifeDuration }
        [SerializeField]
        InvokeMode vfxDieMode;
        
        [SerializeField]
        VisualEffect vfxDie;
        public VisualEffect VFXDie => vfxDie;

        [SerializeField]
        InvokeMode onDieMode;

        [SerializeField]
        UnityEvent OnDieEvent = new UnityEvent();

        CapsuleCollider2D col;
        public CapsuleCollider2D Col => col;

        private const string IS_EMITTING = "isEmitting";
        private const string LIFE_DURATION = "lifeDuration";
        private const string DEATH_TIME = "deathTime";

        BulletProperties bulletProperties;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<CapsuleCollider2D>();

            if(vfxDie!=null) vfxDie.gameObject.SetActive(false);
        }

        public void Init(BulletProperties bulletProperties, ref Action onBulletControllerDie)
        {
            this.bulletProperties = bulletProperties;
            if (vfx != null && vfx.HasFloat(LIFE_DURATION))
                vfx.SetFloat(LIFE_DURATION, bulletProperties.LifeDuration);

            onBulletControllerDie += this.OnBulletControllerDie;
        }

        void OnBulletControllerDie()
        {
            if (vfx != null)
                Destroy(vfx.gameObject);
            if (vfxDie != null)
                Destroy(vfxDie.gameObject);
        }

        public float Die(float deathTime)
        {
            var allVFXsLifeDuration = 0f;

            sr.enabled = false;
            foreach (var light in lights)
            {
                light.enabled = false;
            }

            if (vfx != null)
            {
                if (vfx.HasBool(IS_EMITTING))
                    vfx.SetBool(IS_EMITTING, false);
                if (vfx.HasFloat(LIFE_DURATION))
                    allVFXsLifeDuration = vfx.GetFloat(LIFE_DURATION);
            }

            if (vfxDie != null)
            {
                switch (vfxDieMode)
                {
                    case InvokeMode.Always: 
                        DoVFXDie();
                        break;
                    case InvokeMode.BeforeLifeDuration:
                        if (deathTime < bulletProperties.LifeDuration)
                            DoVFXDie();
                        break;
                }
            }

            switch (onDieMode)
            {
                case InvokeMode.Always:
                    OnDieEvent.Invoke();
                    break;
                case InvokeMode.BeforeLifeDuration:
                        if (deathTime < bulletProperties.LifeDuration)
                            OnDieEvent.Invoke();
                    break;
            }


            return allVFXsLifeDuration;

            void DoVFXDie()
            {
                vfxDie.transform.parent = null;
                vfxDie.gameObject.SetActive(true);
                if (vfxDie.HasFloat(LIFE_DURATION))
                {
                    if (vfxDie.HasFloat(DEATH_TIME))
                        vfxDie.SetFloat(DEATH_TIME, deathTime);

                    if (allVFXsLifeDuration < vfxDie.GetFloat(LIFE_DURATION))
                        allVFXsLifeDuration += vfxDie.GetFloat(LIFE_DURATION) - allVFXsLifeDuration;
                }
            }
        }
    }
}
