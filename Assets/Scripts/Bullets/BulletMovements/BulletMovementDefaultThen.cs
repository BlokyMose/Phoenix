using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Bullet Movements/Default Then", fileName = "BM_DefaultThen")]
    public class BulletMovementDefaultThen : BulletMovement
    {
        [SerializeField]
        float delay = 0.5f;

        [SerializeField]
        BulletMovement movementAfterDelay;

        public override void ModifyBullet(BulletController bullet, out System.Object cache)
        {
            cache = delay;
        }

        public override void Move(BulletController bullet, ref System.Object cache)
        {
            cache = (float)cache - Time.deltaTime;

            if ((float)cache < 0f)
            {
                bullet.BulletMovement = movementAfterDelay;
            }

            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * Time.deltaTime * (Vector2)bullet.transform.up) - bullet.RigidBody.velocity, ForceMode2D.Impulse);
        }
    }
}
