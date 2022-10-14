using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Bullet Movements/Wind", fileName = "BM_Wind")]
    public class BulletMovementWind : BulletMovement
    {
        [SerializeField]
        [MinMaxSlider(1,10)]
        Vector2 massRange = new Vector2(1,3);

        public override void ModifyBullet(Bullet bullet)
        {
            bullet.gameObject.layer = LayerMask.NameToLayer("Wind");
            bullet.RigidBody.mass = Random.Range(massRange.x,massRange.y);
        }

        public override void Move(Bullet bullet)
        {
            var destination = bullet.transform.up * 1000;
            bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, destination, bullet.BulletProperties.speed / 10f);
        }
    }
}
