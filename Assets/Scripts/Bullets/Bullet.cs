using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        #region [Vars: Data Handlers]

        BulletProperties bulletProperties;
        bool isActive = false;
        Vector2 destination;
        GameObject bulletSprite;

        #endregion

        Coroutine corDestroyingSelf;

        public void Init(BulletProperties bulletProperties)
        {
            this.bulletProperties = bulletProperties;
            destination = transform.up * 1000;

            #region [Setup RB2D]

            var rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.mass = 0;

            #endregion

            #region [Setup Collider]

            var col = GetComponent<CapsuleCollider2D>();
            col.isTrigger = false;

            // Match bulet sprite's collider with this collider
            bulletSprite = Instantiate(bulletProperties.bulletPrefab, transform);
            var bulletSpriteCol = bulletSprite.GetComponent<CapsuleCollider2D>();
            if (bulletProperties.matchBulletPrefabCollider && bulletSpriteCol != null)
            {
                col.size = bulletSpriteCol.size * bulletSprite.transform.localScale;
                col.offset = bulletSpriteCol.offset * bulletSprite.transform.localScale;
            }
            else
            {
                col.size = bulletProperties.colliderSize;
                col.offset = bulletProperties.colliderOffset;
            }
            Destroy(bulletSpriteCol); // Extra collider on the bullet sprite is not needed

            #endregion

            DelayActivation();
            CountingLifeDuration();
        }

        void FixedUpdate()
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, bulletProperties.speed/10f);
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
                Destroy(bulletSprite);
                // TODO: VFX
                yield return new WaitForSeconds(2f);
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActive) return;

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
