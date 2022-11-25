using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Properties/Jet Properties", fileName = "JP_")]
    public class JetProperties : ScriptableObject
    {
        public enum MoveMode { Smooth, Constant }

        [SerializeField]
        GameObject jetPrefab;
        public GameObject JetPrefab => jetPrefab;

        [Header("Movement")]
        [SerializeField]
        MoveMode mode = MoveMode.Smooth;
        public MoveMode Mode => mode;

        [SerializeField]
        float moveSpeed = 0.33f;
        public float MoveSpeed => moveSpeed;

        [SerializeField]
        float maxVelocity = 10f;
        public float MaxVelocity => maxVelocity;
        [SerializeField]
        float cursorSpeed = 0.2f;
        public float CursorSpeed => cursorSpeed;

        [SerializeField]
        float linearDrag = 1f;
        public float LinearDrag => linearDrag;

        [Header("Attack")]
        [SerializeField]
        float rps = 2f;
        public float RPS => rps;

        [Header("Cursor")]
        [SerializeField]
        GameObject cursor;
        public GameObject Cursor => cursor;
    }
}
