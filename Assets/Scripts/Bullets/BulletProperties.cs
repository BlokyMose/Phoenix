using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Bullet Properties", fileName = "BP_New")]
    public class BulletProperties : ScriptableObject
    {
        [InlineButton(nameof(AssignDefaultBulletPrefab), "Default", ShowIf = "@!"+nameof(bulletPrefab))]
        [SerializeField]
        BulletComponents bulletPrefab;
        public BulletComponents BulletPrefab => bulletPrefab;

        [InlineButton(nameof(AssignDefaultBulletMovement), "Default", ShowIf ="@!"+nameof(bulletMovement))]
        [SerializeField]
        BulletMovement bulletMovement;
        public BulletMovement BulletMovement => bulletMovement;

        [SerializeField]
        Element element;
        public Element Element => element;

        [Tooltip("Amount of damage the opponent will receive")]
        [SerializeField]
        float damage = 10f;
        public float Damage => damage;

        [Tooltip("How long until this auto-destroy; This variable also affect the VFX's life duration")]
        [SerializeField]
        float lifeDuration = 4f;
        public float LifeDuration => lifeDuration;

        [Tooltip("How fast the bullet moves")]
        [SerializeField]
        float speed = 1f;
        public float Speed => speed;

        [Tooltip("Amount of force to push back the opponent when hit")]
        [SerializeField]
        float pushForce = 3.33f;
        public float PushForce => pushForce;

        [Header("Capsule Collider"), Tooltip("Modify the bullet's collider to have the same size and offset from the prefab")]
        [SerializeField]
        bool matchBulletPrefabCollider = true;
        public bool MatchBulletPrefabCollider => matchBulletPrefabCollider;

        [EnableIf("@!"+nameof(matchBulletPrefabCollider))]
        [SerializeField]
        Vector2 colliderSize = new Vector2(0.05f, 0.5f);
        public Vector2 ColliderSize => colliderSize;

        [EnableIf("@!" + nameof(matchBulletPrefabCollider))]
        [SerializeField]
        Vector2 colliderOffset = new Vector2(0, 0.5f);
        public Vector2 ColliderOffset => colliderOffset;

        [Header("Icon")]
        [SerializeField]
        Sprite icon;
        public Sprite Icon => icon;

        void AssignDefaultBulletPrefab()
        {
#if UNITY_EDITOR

            var allBullet = UnityEditor.AssetDatabase.FindAssets("t:GameObject" + " Bullet_Default");
            var bulletDefaultPath = UnityEditor.AssetDatabase.GUIDToAssetPath(allBullet[0]);
            var bulletDefault = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(bulletDefaultPath);
            bulletPrefab = bulletDefault.GetComponent<BulletComponents>();

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        void AssignDefaultBulletMovement()
        {
#if UNITY_EDITOR

            var allBulletMovements = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(BulletMovement) + " Default");
            var bulletMovementDefaultPath = UnityEditor.AssetDatabase.GUIDToAssetPath(allBulletMovements[0]);
            var bulletMovementDefault = UnityEditor.AssetDatabase.LoadAssetAtPath<BulletMovement>(bulletMovementDefaultPath);
            bulletMovement = bulletMovementDefault;

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
