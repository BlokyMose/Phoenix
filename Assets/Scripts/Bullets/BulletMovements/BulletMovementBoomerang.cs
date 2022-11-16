using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Encore.Utility.MathUtility;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Boomerang", fileName = "BM_Boomerang")]
    public class BulletMovementBoomerang : BulletMovement
    {
        [SerializeField]
        float slowDownDistance = 3f;

        [SerializeField]
        float stopDistance = 0.5f;

        [SerializeField]
        LayerMask stopAtLayer;

        [SerializeField]
        BulletMovement movementAfterStop;


        public override void ModifyBullet(BulletController bullet)
        {

        }

        public override void Move(BulletController bullet, Transform target)
        {
            var speedRate = 1f;
            var rays = Physics2D.RaycastAll(bullet.transform.position, bullet.transform.up, slowDownDistance);
            foreach (var ray in rays)
                if (ray.collider.gameObject.layer == (int)Mathf.Log(stopAtLayer.value, 2))
                {
                    var distance = Vector2.Distance(bullet.transform.position, ray.point);
                    if (distance < stopDistance)
                    {
                        bullet.RigidBody.velocity = Vector2.zero;
                        speedRate = 0f;
                        bullet.BulletMovement = movementAfterStop;
                    }
                    else
                    {
                        speedRate = distance / (slowDownDistance - stopDistance);
                    }

                    break;
                }

            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * speedRate * Time.deltaTime * (Vector2)bullet.transform.up) - bullet.RigidBody.velocity, ForceMode2D.Impulse);




        }
    }
}
