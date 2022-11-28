using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.BulletProperties;
using static UnityEngine.ParticleSystem;

namespace Phoenix
{
    public class BulletControllerTriggerEnter : BulletController
    {
        int currentLifeCount;

        public override void Init(BulletProperties bulletProperties)
        {
            base.Init(bulletProperties);
            Col.isTrigger = true;
            currentLifeCount = bulletProperties.LifeCount;
        }

        void ReduceLifeCount()
        {
            currentLifeCount--;
            if (currentLifeCount <= 0)
                DestroySelf();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!isActive) return;

            ApplyDamageTo(collider.GetComponentInFamily<HealthController>());
            ApplyForceTo(collider, collider.GetComponentInFamily<Rigidbody2D>());
            ReduceLifeCount();
        }


        void ApplyForceTo(Collider2D collider, Rigidbody2D rb)
        {
            if (rb == null) return;
            
            var direction = collider.transform.position - transform.position;
            rb.AddForceAtPosition(direction * BulletProperties.PushForce, collider.ClosestPoint(transform.position));
        }

    }
}
