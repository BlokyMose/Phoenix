using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Bullet Movements/Magnet", fileName = "BM_Magnet")]
    public class BulletMovementMagnet : BulletMovement
    {
        [SerializeField]
        [MinMaxSlider(1,10)]
        Vector2 massRange = new Vector2(1,3);

        [SerializeField, Tooltip("Instead of using Jet Controller's bulletLayer")]
        bool overrideLayerMask = false;

        [SerializeField, ShowIf("@"+nameof(overrideLayerMask))]
        LayerMask layerMask;

        public override void ModifyBullet(BulletController bullet)
        {
            if (overrideLayerMask)
                bullet.gameObject.layer = layerMask.value;
            bullet.RigidBody.mass = Random.Range(massRange.x,massRange.y);
        }

        public override void Move(BulletController bullet)
        {
            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * Time.deltaTime * (Vector2)bullet.transform.up), ForceMode2D.Force);
        }
    }
}
