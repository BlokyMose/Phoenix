using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Bullet Movements/Homing", fileName = "BM_Homing")]
    public class BulletMovementHoming : BulletMovement
    {
        public class Cache
        {
            public Transform target;

            public Cache(Transform target)
            {
                this.target = target;
            }
        }

        [SerializeField]
        protected LayerMask targetLayer;

        [SerializeField]
        protected float loseAccuracyDistance = 2.5f;

        [SerializeField, Range(0,1)]
        protected float accuracy = 1f;

        [SerializeField]
        protected BulletMovement movementAfterReachedTarget;

        public override void ModifyBullet(BulletController bullet, out System.Object cache)
        {
            cache = new Cache(GetRandomTarget());
        }

        public override void Move(BulletController bullet, ref System.Object cache)
        {
            var direction = (Vector2)bullet.transform.up;
            var _cache = cache as Cache;

            if (_cache.target != null)
            {
                var margin = Vector2.Distance(bullet.transform.position, _cache.target.transform.position);
                if (margin < loseAccuracyDistance)
                {
                    if (movementAfterReachedTarget != null)
                        bullet.BulletMovement = movementAfterReachedTarget;
                }
                else
                {
                    Vector3 relative = bullet.transform.InverseTransformPoint(_cache.target.position);
                    var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg * GetAccuracy(_cache);
                    bullet.transform.Rotate(0, 0, -angle);
                }
            }

            bullet.RigidBody.AddForce((bullet.BulletProperties.Speed * Time.deltaTime * direction) - bullet.RigidBody.velocity, ForceMode2D.Impulse);
        }

        protected Transform GetRandomTarget()
        {
            var possibleTargets = new List<Transform>();
            var allGOs = FindObjectsOfType<GameObject>();
            foreach (var go in allGOs)
                if (go.layer == (int)Mathf.Log(targetLayer.value, 2))
                    possibleTargets.Add(go.transform);

            return possibleTargets[Random.Range(0, possibleTargets.Count)];
        }

        protected virtual float GetAccuracy(Cache cache)
        {
            return   accuracy;
        }
    }
}
