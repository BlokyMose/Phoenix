using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Default", fileName = "BM_Default")]
    public class BulletMovementDefault : BulletMovement
    {
        public override void ModifyBullet(BulletController bullet)
        {
        }

        public override void Move(BulletController bullet)
        {
            bullet.RigidBody.AddForce((bullet.BulletProperties.speed * Time.deltaTime * (Vector2)bullet.transform.up) - bullet.RigidBody.velocity, ForceMode2D.Impulse);
        }
    }
}
