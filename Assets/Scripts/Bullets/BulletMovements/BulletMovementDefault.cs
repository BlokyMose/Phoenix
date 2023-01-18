using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Default", fileName = "BM_Default")]
    public class BulletMovementDefault : BulletMovement
    {
        public override void Move(BulletController bullet, ref System.Object cache)
        {
            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * Time.deltaTime * (Vector2)bullet.transform.up) - bullet.RigidBody.velocity, ForceMode2D.Impulse);
        }
    }
}
