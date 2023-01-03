using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Camera Shake Properties")]
    public class CameraShakeProperties : ScriptableObject
    {
        [SerializeField]
        Cinemachine.CinemachineImpulseDefinition.ImpulseTypes impulseType = Cinemachine.CinemachineImpulseDefinition.ImpulseTypes.Uniform;

        public Cinemachine.CinemachineImpulseDefinition.ImpulseTypes ImpulseType => impulseType;

        [SerializeField]
        Cinemachine.CinemachineImpulseDefinition.ImpulseShapes impulseShape = Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Bump;
        public Cinemachine.CinemachineImpulseDefinition.ImpulseShapes ImpulseShape => impulseShape;

        [Header("Duration")]
        [SerializeField, LabelText("Min")]
        float durationMin = 0.2f;
        public float DurationMin => durationMin;    

        [SerializeField, LabelText("Max")]
        float durationMax = 0.225f;
        public float DurationMax => durationMax;

        [SerializeField, LabelText("Randomize +/-")]
        float durationRange = 0.025f;
        public float DurationRange => durationRange;

        [Header("Velocity")]
        [SerializeField, LabelText("Min")]
        Vector2 velocityMin = new(0.1f, 0.133f);
        public Vector2 VelocityMin => velocityMin;

        [SerializeField, LabelText("Max")]
        Vector2 velocityMax = new(0.125f, 0.15f);
        public Vector2 VelocityMax => velocityMax;

        [SerializeField, LabelText("Randomize +/-")]
        Vector2 velocityRange = new(0.33f, 0.366f);
        public Vector2 VelocityRange => velocityRange;

        [Header("Force")]
        [SerializeField, LabelText("Min")]
        float forceMin = 0.25f;
        public float ForceMin => forceMin;

        [SerializeField, LabelText("Max")]
        float forceMax = 0.3f;
        public float ForceMax => forceMax;

        [SerializeField, LabelText("Randomize +/-")]
        float forceRange = 0.1f;
        public float ForceRange => forceRange;
    }

}
