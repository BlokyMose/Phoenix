using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="SO/Bullet Properties", fileName = "BP_New")]
    public class BulletProperties : ScriptableObject
    {
        [InlineButton(nameof(AssignDefaultBulletPrefab), "Default", ShowIf = "@!"+nameof(bulletPrefab))]
        public BulletComponents bulletPrefab;
        [InlineButton(nameof(AssignDefaultBulletMovement), "Default", ShowIf ="@!"+nameof(bulletMovement))]
        public BulletMovement bulletMovement;
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
