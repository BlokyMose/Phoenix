using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JetController : MonoBehaviour
    {
        #region [Vars: Components]

        [SerializeField]
        JetProperties jetProperties;
        public JetProperties JetProperties { get { return jetProperties; } }

        [SerializeField]
        List<BulletProperties> bulletProperties = new List<BulletProperties>();

        PlayerBrain brain;
        Rigidbody2D rb;

        #endregion


        #region [Vars: Data Handlers]

        Vector2 moveDirection;
        Vector2 cursorWorldPos;
        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!"+nameof(jet))]
        JetComponents jet;
        float fireCooldown;
        bool isFiring = false;

        #endregion


        private void Awake()
        {
            brain = GetComponent<PlayerBrain>();
            rb = GetComponent<Rigidbody2D>();
            rb.drag = jetProperties.linearDrag;
            InstantiateJet();
        }

        void OnEnable()
        {
            brain.OnMoveInput += (dir) => { moveDirection = dir; };
            brain.OnFireInput += (isFiring) => { this.isFiring = isFiring; };
        }

        void FixedUpdate()
        {
            RotateToCursor();
            Move();
            Fire();
        }

        void InstantiateJet()
        {
            if (jet == null)
            {
                var foundJet = transform.Find("Jet");
                if (foundJet != null)
                {
                    jet = foundJet.gameObject.GetComponent<JetComponents>();
                }
                else
                {
                    jet = Instantiate(jetProperties.jetPrefab, transform);
                    jet.name = "Jet";
                }
            }

        }

        void RotateToCursor()
        {
            cursorWorldPos = brain.GetCursorWorldPosition();
            var margin = (Vector2)transform.position - cursorWorldPos;
            var angle = Mathf.Atan2(margin.y, margin.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, angle));
        }

        void Move()
        {
            if (rb.velocity.magnitude < jetProperties.maxVelocity)
                rb.AddForce(moveDirection * jetProperties.moveSpeed, ForceMode2D.Impulse);
        }

        void Fire()
        {
            fireCooldown -= Time.deltaTime;

            if (!isFiring) return;
            if (fireCooldown > 0) return;
            fireCooldown = 1/jetProperties.rps;

            var bulletGO = new GameObject("Bullet");
            bulletGO.transform.position = jet.FireOrigins[0].transform.position;
            bulletGO.transform.eulerAngles = jet.FireOrigins[0].eulerAngles;
            var bullet = bulletGO.AddComponent<Bullet>();
            bullet.Init(bulletProperties[0]);
        }
    }
}

