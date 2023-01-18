using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Bullet Movements/Homing Gradually", fileName = "BM_HomingGradually")]
    public class BulletMovementHomingGradually : BulletMovementHoming
    {
        public class CacheGradually:Cache
        {
            public float timeLeft;

            public CacheGradually(Transform target, float timeLeft):base(target)
            {
                this.timeLeft = timeLeft;
            }
        }


        [SerializeField]
        float duration = 1f;

        public override void ModifyBullet(BulletController bullet, out System.Object cache)
        {
            cache = new CacheGradually(GetRandomTarget(), duration);
        }

        public override void Move(BulletController bullet, ref System.Object cache)
        {
            (cache as CacheGradually).timeLeft -= Time.deltaTime;
            base.Move(bullet, ref cache);
        }

        protected override float GetAccuracy(Cache cache)
        {
            return accuracy * (1 - ((cache as CacheGradually).timeLeft / duration));
        }
    }
}
