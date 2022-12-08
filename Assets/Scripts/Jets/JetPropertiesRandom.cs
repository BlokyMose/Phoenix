using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Properties/Jet Properties Random", fileName = "JPR_")]
    public class JetPropertiesRandom : JetProperties
    {
        [SerializeField]
        List<GameObject> jetPrefabs = new List<GameObject>();
        public override GameObject JetPrefab => jetPrefabs.GetRandom();

        [Header("Movement")]
        [SerializeField]
        bool isRandomizeMode = false;

        [SerializeField, HideIf(nameof(isRandomizeMode)), GUIColor(nameof(GetModeColor))]
        MoveMode mode = MoveMode.Smooth;

        public override MoveMode Mode 
        { 
            get
            {
                if (!isRandomizeMode)
                    return mode;
                else
                {
                    var random = Random.Range(0, Enum.GetNames(typeof(MoveMode)).Length);
                    if (random == 0)
                        return MoveMode.Smooth;
                    else
                        return MoveMode.Constant;
                }
            } 
        }

        [SerializeField]
        Vector2 moveSpeedRange = new();
        public override float MoveSpeed => Random.Range(moveSpeedRange.x, moveSpeedRange.y);

        [SerializeField]
        Vector2 maxVelociyRange = new();
        public override float MaxVelocity => Random.Range(maxVelociyRange.x, maxVelociyRange.y);

        [SerializeField]
        Vector2 cursorSpeedRange = new();
        public override float CursorSpeed => Random.Range(cursorSpeedRange.x, cursorSpeedRange.y);

        [SerializeField]
        Vector2 linearDragRange = new();
        public override float LinearDrag => Random.Range(linearDragRange.x, linearDragRange.y);

        [Header("Attack")]
        [SerializeField]
        Vector2 rpsRange = new();
        public override float RPS => Random.Range(rpsRange.x, rpsRange.y);

        [SerializeField]
        List<GameObject> cursors = new List<GameObject>();
        public override GameObject Cursor => cursors.GetRandom();

        public JetPropertiesStatic GenerateJetPropertiesStatic()
        {
            var jps = CreateInstance<JetPropertiesStatic>();
            return jps.Init(
                JetPrefab,
                Mode,
                MoveSpeed,
                MaxVelocity,
                CursorSpeed,
                LinearDrag,
                RPS,
                Cursor
                );
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
