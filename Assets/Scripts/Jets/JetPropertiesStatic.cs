using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.JetProperties;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Properties/Jet Properties Static", fileName = "JPS_")]
    public class JetPropertiesStatic : JetProperties
    {

        [SerializeField]
        GameObject jetPrefab;
        public override GameObject JetPrefab => jetPrefab;

        [Header("Movement")]
        [SerializeField, GUIColor(nameof(GetModeColor))]
        MoveMode mode = MoveMode.Smooth;
        public override MoveMode Mode => mode;

        [SerializeField]
        float moveSpeed = 15f;
        public override float MoveSpeed => moveSpeed;

        [SerializeField]
        float maxVelocity = 10f;
        public override float MaxVelocity => maxVelocity;

        [SerializeField]
        float cursorSpeed = 0.2f;
        public override float CursorSpeed => cursorSpeed;

        [SerializeField]
        float linearDrag = 1f;
        public override float LinearDrag => linearDrag;

        [Header("Attack")]
        [SerializeField]
        float rps = 2f;
        public override float RPS => rps;

        [Header("Cursor")]
        [SerializeField]
        GameObject cursor;
        public override GameObject Cursor => cursor;

        public JetPropertiesStatic Init(GameObject jetPrefab, MoveMode mode, float moveSpeed, float maxVelocity, float cursorSpeed, float linearDrag, float rps, GameObject cursor)
        {
            this.jetPrefab = jetPrefab;
            this.mode = mode;
            this.moveSpeed = moveSpeed;
            this.maxVelocity = maxVelocity;
            this.cursorSpeed = cursorSpeed;
            this.linearDrag = linearDrag;
            this.rps = rps;
            this.cursor = cursor;

            return this;
        }

        Color GetModeColor()
        {
            switch (mode)
            {
                case MoveMode.Smooth:
                    return Encore.Utility.ColorUtility.mediumSpringGreen;
                case MoveMode.Constant:
                    return Encore.Utility.ColorUtility.goldenRod;
                default:
                    return Color.gray;
            }
        }
    }
}
