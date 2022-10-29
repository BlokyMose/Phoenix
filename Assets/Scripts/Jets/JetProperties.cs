using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Jet Properties")]
    public class JetProperties : ScriptableObject
    {
        public FireComponents jetPrefab;

        [Header("Movement")]
        public float moveSpeed = 0.33f;
        public float maxVelocity = 10f;
        public float cursorSpeed = 0.2f;
        public float linearDrag = 1f;

        [Header("Attack")]
        public float rps = 2f;

        [Header("Cursor")]
        public GameObject cursor;
    }
}
