using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    public class InstantiateGO : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        [HorizontalGroup("scale", 10), SerializeField, LabelWidth(0.1f)]
        bool isOverrideScale = false;

        [HorizontalGroup("scale"), SerializeField, EnableIf(nameof(isOverrideScale))]
        Vector2 scaleOverride = Vector2.one;

        public void Invoke()
        {
            var go = Instantiate(prefab, null);
            go.transform.position = transform.position;
            go.transform.localEulerAngles = transform.localEulerAngles;

            if (isOverrideScale) 
                go.transform.localScale = scaleOverride;

        }
    }
}
