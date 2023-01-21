using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class DestroyBulletTriggerWhenEnter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponentInFamily<BulletControllerTriggerEnter>(out var bullet))
            {
                bullet.DestroySelf();
            }
        }
    }
}
