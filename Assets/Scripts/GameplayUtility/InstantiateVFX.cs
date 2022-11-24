using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    public class InstantiateVFX : MonoBehaviour
    {
        [SerializeField]
        VisualEffectAsset vfx;

        [SerializeField]
        float destroySelfDelay = 3f;

        [HorizontalGroup("scale", 10), SerializeField, LabelWidth(0.1f)]
        bool isOverrideScale = false;

        [HorizontalGroup("scale"), SerializeField, EnableIf(nameof(isOverrideScale))]
        Vector2 vfxScaleOverride = Vector2.one;

        public void Invoke()
        {
            var go = new GameObject("VFX_"+name);
            go.transform.position = transform.position;
            go.transform.localEulerAngles = transform.localEulerAngles;
            if (isOverrideScale) go.transform.localScale = vfxScaleOverride;

            var vfxComponent = go.AddComponent<VisualEffect>();
            vfxComponent.visualEffectAsset = vfx;

            var destroySelf = go.AddComponent<DestroySelf>();
            destroySelf.Init(destroySelfDelay);
        }
    }
}
