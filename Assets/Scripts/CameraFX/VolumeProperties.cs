using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Camera FX/Volume Properties")]
    public class VolumeProperties : ScriptableObject
    {
        [SerializeField]
        VolumeProfile volume;
        public VolumeProfile Volume => volume;

        [SerializeField]
        float inDuration = 0.25f;
        public float InDuration => inDuration;


        [SerializeField]
        float outDuration = 0.75f;
        public float OutDuration => outDuration;
    }
}
