using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Collider2D))]
    public class Enemy2 : MonoBehaviour
    {

        #region [Vars: Properties]

        [SerializeField]
        float maxHealth = 100;

        private Transform player;
        private Vector3 playerLastPos, startPos, movementPos;
        [SerializeField]
        private float chasespeed = 0.8f, turningDelay = 1f;
        private float lastFollowTime, turningTimeDelay = 1f;

        #endregion

        #region [Vars: Data Handlers]

        float health;

        #endregion

        #region [Delegates]

        Action OnDie;

        #endregion

        #region [Methods: Initialization]


        void Start()
        {

            player = GameObject.FindWithTag("Player").transform;
            playerLastPos = player.position;
            startPos = transform.position;
            lastFollowTime = Time.time;
            health = maxHealth;
        }

        private void Update()
        {
            transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 3), transform.position.z);
        }


        public void Init(Action onDie)
        {
            this.OnDie += onDie;
        }

        #endregion


        public float GetHealth() => health;

        public void ReceiveDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
                Die();
        }

        public void Die()
        {
            OnDie?.Invoke();
            Destroy(gameObject);
        }

        void FixedUpdate()
        {
            Chase();
            void Chase()
            {
                if (Time.time - lastFollowTime > turningTimeDelay)
                {
                    playerLastPos = player.transform.position;
                    lastFollowTime -= Time.time;

                }

                if (Vector3.Distance(transform.position, playerLastPos) > 0.15f)
                {
                    movementPos = (playerLastPos - transform.position).normalized * chasespeed;
                }
                else
                {
                    movementPos = Vector3.zero;
                }
                CharacterMovement(movementPos.x, movementPos.y);

            }
        }

        [SerializeField]
        protected float xSpeed = 1.5f, ySpeed = 1.5f;
        private Vector2 moveDelta;

        protected void CharacterMovement(float x, float y)
        {
            moveDelta = new Vector2(x * xSpeed, y * ySpeed);
            transform.Translate(moveDelta.x * Time.deltaTime, moveDelta.y * Time.deltaTime, 0);

           
        }
    }
}
