using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BulletController : MonoBehaviour
    {
        #region [Vars: Data Handlers]

        BulletProperties bulletProperties;
        public BulletProperties BulletProperties => bulletProperties;
        protected bool isActive = false;
        BulletComponents bulletComponents;
        Transform target;
        public Transform Target { get => target; set { target = value; } }
        BulletMovement bulletMovement;
        public BulletMovement BulletMovement 
        { 
            get => bulletMovement; 
            set 
            { 
                bulletMovement = value;
                bulletMovement.ModifyBullet(this);
            } 
        }


        #endregion

        #region [Vars: Components]

        Rigidbody2D rb;
        public Rigidbody2D RigidBody => rb;

        CapsuleCollider2D col;
        public CapsuleCollider2D Col => col;

        public Element Element => bulletProperties.Element;

        #endregion

        float lifetime = 0;

        Coroutine corDestroyingSelf;

        #region [Delegates]

        /// <summary>
        /// Returns <see cref="BulletProperties.damage"/> or <see cref="ElementContainer.GetDamage(float, Element)"/>
        /// </summary>
        Func<float,Element, float> GetProcessedDamage;
        Action OnDie;

        #endregion

        public virtual void Init(BulletProperties bulletProperties)
        {
            this.bulletProperties = bulletProperties;

            bulletComponents = Instantiate(bulletProperties.BulletPrefab, transform);
            bulletComponents.transform.localPosition = Vector3.zero;
            bulletComponents.transform.localEulerAngles = Vector3.zero;
            bulletComponents.Init(bulletProperties, ref OnDie);

            #region [Setup RB2D]

            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            #endregion

            #region [Setup Collider]

            col = GetComponent<CapsuleCollider2D>();

            // Match bulet sprite's collider with this collider
            if (bulletProperties.MatchBulletPrefabCollider && bulletComponents.Col != null)
            {
                col.size = bulletComponents.Col.size * bulletComponents.transform.localScale;
                col.offset = bulletComponents.Col.offset * bulletComponents.transform.localScale;
                bulletComponents.Col.enabled = false; // Extra collider on the bullet sprite is not needed
            }
            else
            {
                col.size = bulletProperties.ColliderSize;
                col.offset = bulletProperties.ColliderOffset;
            }

            #endregion

            #region [Setup ElementContainer]

            if (bulletProperties.Element != null)
            {
                var elementContainer = gameObject.GetComponent<ElementContainer>();
                if (elementContainer == null) elementContainer = gameObject.AddComponent<ElementContainer>();
                elementContainer.Init(bulletProperties.Element, ref GetProcessedDamage);
            }
            else
            {
                GetProcessedDamage = (damage, otherElement) => { return damage; };
            }

            #endregion

            BulletMovement = bulletProperties.BulletMovement;
            isActive = true;

            CountingLifetimeThenDestroySelf();
        }

        void FixedUpdate()
        {
            bulletMovement.Move(this, target);
        }

        /// <summary>
        /// Prevent this bullet to collide with the jet that is firing it
        /// </summary>
        protected void DelayActivation()
        {
            StartCoroutine(Delay(0.05f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                isActive = true;
            }
        }

        void CountingLifetimeThenDestroySelf()
        {
            StartCoroutine(IncrementingLifetime());
            IEnumerator IncrementingLifetime()
            {
                while (lifetime < bulletProperties.LifeDuration)
                {
                    lifetime += Time.deltaTime;
                    yield return null;
                }
                DestroySelf();
            }
        }

        protected void DestroySelf()
        {
            if (corDestroyingSelf != null) return;

            corDestroyingSelf = StartCoroutine(Delay());
            IEnumerator Delay()
            {
                isActive = false;
                col.enabled = false;
                yield return new WaitForSeconds(bulletComponents.Die(lifetime));
                StopAllCoroutines();

                OnDie?.Invoke();
                Destroy(gameObject);
            }
        }

        protected void ApplyDamageTo(HealthController otherHealthController)
        {
            if (otherHealthController == null) return;

            var otherElementContainer = otherHealthController.GetComponent<ElementContainer>();
            var otherElement = otherElementContainer != null ? otherElementContainer.Element : null;
            otherHealthController.ReceiveDamage(GetProcessedDamage(bulletProperties.Damage, otherElement));

        }

    }
}
