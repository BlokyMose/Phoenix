using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class BulletComponents : MonoBehaviour
    {
        public GameObject GO => gameObject;

        SpriteRenderer sr;
        public SpriteRenderer SR => sr;

        [SerializeField]
        VisualEffect vfx;
        public VisualEffect VFX => vfx;        
        
        [SerializeField]
        VisualEffect vfxDie;
        public VisualEffect VFXDie => vfxDie;

        CapsuleCollider2D col;
        public CapsuleCollider2D Col => col;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<CapsuleCollider2D>();

            if(vfxDie!=null) vfxDie.gameObject.SetActive(false);
        }

        public void Init(BulletProperties bulletProperties)
        {
            if (vfx != null && vfx.HasFloat(nameof(bulletProperties.lifeDuration)))
                vfx.SetFloat(nameof(bulletProperties.lifeDuration), bulletProperties.lifeDuration);
        }

        public float Die()
        {
            var allVFXsLifeDuration = 0f;

            sr.enabled = false;

            if (vfx != null)
            {
                if (vfx.HasBool("isEmitting"))
                    vfx.SetBool("isEmitting", false);
                if (vfx.HasFloat("lifeDuration"))
                    allVFXsLifeDuration = vfx.GetFloat("lifeDuration")/4f;
            }

            if (vfxDie != null)
            {
                vfxDie.gameObject.SetActive(true);
                if (vfxDie.HasFloat("lifeDuration"))
                {
                    if (allVFXsLifeDuration < vfxDie.GetFloat("lifeDuration"))
                        allVFXsLifeDuration += vfxDie.GetFloat("lifeDuration") - allVFXsLifeDuration;
                }
            }


            Debug.Log(nameof(allVFXsLifeDuration) + " : " + allVFXsLifeDuration);
            return allVFXsLifeDuration;
        }
    }
}
