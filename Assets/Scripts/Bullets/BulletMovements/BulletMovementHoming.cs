using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Homing", fileName = "BM_Homing")]
    public class BulletMovementHoming : BulletMovement
    {
        [SerializeField]
        LayerMask targetLayer;

        [SerializeField]
        float loseAccuracyDistance = 2.5f;

        [SerializeField]
        BulletMovement movementAfterReachedTarget;

        public override void ModifyBullet(BulletController bullet)
        {
            var possibleTargets = new List<Transform>();
            var allGOs = FindObjectsOfType<GameObject>();
            foreach (var go in allGOs)
                if (go.layer == (int)Mathf.Log(targetLayer.value, 2))
                    possibleTargets.Add(go.transform);

            var target = possibleTargets[Random.Range(0, possibleTargets.Count)];
            bullet.Target = target;
        }

        public override void Move(BulletController bullet, Transform target)
        {
            var direction = (Vector2)bullet.transform.up;

            if (target != null)
            {
                var margin = Vector2.Distance(bullet.transform.position, target.transform.position);
                if (margin < loseAccuracyDistance)
                {
                    if (movementAfterReachedTarget != null)
                        bullet.BulletMovement = movementAfterReachedTarget;
                }
                else
                {
                    direction = (Vector2)(target.position - bullet.transform.position).normalized;
                    Vector3 relative = bullet.transform.InverseTransformPoint(target.position);
                    var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                    bullet.transform.Rotate(0, 0, -angle);
                }
            }

            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * Time.deltaTime * direction) - bullet.RigidBody.velocity, ForceMode2D.Impulse);
        }
    }
}
