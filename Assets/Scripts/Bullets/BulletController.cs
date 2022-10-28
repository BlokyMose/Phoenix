using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Phoenix
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletController : MonoBehaviour
    {
        #region [Vars: Data Handlers]

        BulletProperties bulletProperties;
        public BulletProperties BulletProperties => bulletProperties;
        bool isActive = false;
        BulletComponents bulletComponents;

        #endregion


        #region [Vars: Components]

        Rigidbody2D rb;
        public Rigidbody2D RigidBody => rb;

        CapsuleCollider2D col;
        public CapsuleCollider2D Col => col;

        #endregion


        Coroutine corDestroyingSelf;

        public void Init(BulletProperties bulletProperties)
        {
            this.bulletProperties = bulletProperties;

            bulletComponents = Instantiate(bulletProperties.bulletPrefab, transform);
            bulletComponents.Init(bulletProperties);

            #region [Setup RB2D]

            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            #endregion

            #region [Setup Collider]

            col = GetComponent<CapsuleCollider2D>();
            col.isTrigger = false;

            // Match bulet sprite's collider with this collider
            if (bulletProperties.matchBulletPrefabCollider && bulletComponents.Col != null)
            {
                col.size = bulletComponents.Col.size * bulletComponents.GO.transform.localScale;
                col.offset = bulletComponents.Col.offset * bulletComponents.GO.transform.localScale;
                bulletComponents.Col.enabled = false; // Extra collider on the bullet sprite is not needed
            }
            else
            {
                col.size = bulletProperties.colliderSize;
                col.offset = bulletProperties.colliderOffset;
            }

            #endregion

            bulletProperties.bulletMovement.ModifyBullet(this);
            DelayActivation();
            CountingLifeDuration();
        }

        void FixedUpdate()
        {
            bulletProperties.bulletMovement.Move(this);
        }

        /// <summary>
        /// Prevent this bullet to collide with the jet that is firing it
        /// </summary>
        void DelayActivation()
        {
            StartCoroutine(Delay(0.05f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                isActive = true;
            }
        }

        /// <summary>
        /// Self destroy after lifeDuration passed
        /// </summary>
        void CountingLifeDuration()
        {
            StartCoroutine(Delay(bulletProperties.lifeDuration));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                DestroySelf();
            }
        }

        void DestroySelf()
        {
            if (corDestroyingSelf != null) return;

            corDestroyingSelf = StartCoroutine(Delay());
            IEnumerator Delay()
            {
                isActive = false;
                col.enabled = false;
                yield return new WaitForSeconds(bulletComponents.Die());
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActive) return;

            // TODO: prevent damaging self

            #region [Apply Damage to IHealth]

            var iHealth = collision.gameObject.GetComponent<IHealth>();
            if (iHealth != null)
                iHealth.ReceiveDamage(bulletProperties.damage);

            #endregion

            #region [Apply Force to RB2D]

            if (collision.rigidbody != null)
            {
                var direction = collision.transform.position - transform.position;
                collision.rigidbody.AddForceAtPosition(direction * bulletProperties.pushForce, collision.GetContact(0).point);
            }

            #endregion

            DestroySelf();
        }
    }
}
