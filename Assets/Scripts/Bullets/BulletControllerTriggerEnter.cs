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

            var parent = collider.gameObject;
            var rb = collider.GetComponent<Rigidbody2D>();

            var jetController = collider.GetComponentInParent<JetController>();
            if (jetController != null)
            {
                parent = jetController.gameObject;
                rb = jetController.RB;
            }

            ApplyDamageTo(parent);
            ApplyForceTo(collider, rb);
            ReduceLifeCount();
        }


        void ApplyForceTo(Collider2D collider, Rigidbody2D rb)
        {
            if (rb != null)
            {
                var direction = collider.transform.position - transform.position;
                rb.AddForceAtPosition(direction * BulletProperties.PushForce, collider.ClosestPoint(transform.position));
            }
        }

    }
}
