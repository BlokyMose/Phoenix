using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.BulletProperties;
using static UnityEngine.ParticleSystem;

namespace Phoenix
{
    public class BulletControllerCollide : BulletController
    {
        public override void Init(BulletProperties bulletProperties)
        {
            base.Init(bulletProperties);
            Col.isTrigger = false;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActive) return;

            ApplyDamageTo(collision.gameObject);
            ApplyForceTo(collision);
            DestroySelf();
        }

        void ApplyForceTo(Collision2D collision)
        {
            if (collision.rigidbody != null)
            {
                var direction = collision.transform.position - transform.position;
                collision.rigidbody.AddForceAtPosition(direction * BulletProperties.PushForce, collision.GetContact(0).point);
            }
        }

    }
}
