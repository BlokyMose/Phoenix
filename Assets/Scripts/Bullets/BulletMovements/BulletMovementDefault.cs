using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Default", fileName = "BM_Default")]
    public class BulletMovementDefault : BulletMovement
    {
        public override void ModifyBullet(Bullet bullet)
        {
        }

        public override void Move(Bullet bullet)
        {
            var destination = bullet.transform.up * 1000;
            bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, destination, bullet.BulletProperties.speed / 10f);
        }
    }
}
