using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;

namespace Phoenix
{
    public class DetectCollisionGiveDamage : DetectCollision
    {
        [SerializeField]
        float damage = 10;

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            var healthController = collision.collider.GetComponentInFamily<HealthController>();
            if (healthController != null)
                healthController.ReceiveDamage(damage);

            base.OnCollisionEnter2D(collision);
        }
    }
}
