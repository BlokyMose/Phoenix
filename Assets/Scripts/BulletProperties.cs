using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Bullet Properties")]
    public class BulletProperties : ScriptableObject
    {
        public GameObject bulletPrefab;
        public GameObject destroyedVFX;
        public float damage = 10f;
        public float lifeDuration = 4f;
        public float speed = 1f;
        public float pushForce = 3.33f;

        [Header("Capsule Collider")]
        public bool matchBulletPrefabCollider = true;
        [EnableIf("@!"+nameof(matchBulletPrefabCollider))]
        public Vector2 colliderSize = new Vector2(0.05f, 0.5f);
        [EnableIf("@!" + nameof(matchBulletPrefabCollider))]
        public Vector2 colliderOffset = new Vector2(0, 0.5f);
    }
}
