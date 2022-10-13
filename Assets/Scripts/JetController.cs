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


        #region [Vars: Properties]

        [SerializeField, Range(0,360)]
        float angleLimit = 360;

        public enum FaceMode { Right, Left, Up, Down }
        [SerializeField]
        FaceMode faceMode = FaceMode.Right;

        #endregion


        #region [Vars: Data Handlers]

        Vector2 moveDirection;
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
            var positionToCursor = (Vector2)transform.position - brain.GetCursorWorldPosition();
            var newAngle = Mathf.Atan2(positionToCursor.y, positionToCursor.x) * Mathf.Rad2Deg;
            var validatedAngle = ValidateAngle(newAngle);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, validatedAngle));

            float ValidateAngle(float newAngle)
            {
                switch (faceMode)
                {
                    case FaceMode.Right:
                        if (newAngle > 0 && newAngle < 180 - angleLimit / 2)
                            return 180 - angleLimit / 2;
                        else if (newAngle < 0 && newAngle > -180 + angleLimit / 2)
                            return -180 + angleLimit / 2;
                        else break;
                    case FaceMode.Left:
                        if (newAngle > angleLimit / 2)
                            return angleLimit / 2;
                        else if (newAngle < -angleLimit / 2)
                            return -angleLimit / 2;
                        else break;
                    case FaceMode.Up:
                        if (newAngle > -90 + angleLimit / 2)
                            return -90 + angleLimit / 2;
                        else if (newAngle < -90 - angleLimit / 2)
                            return -90 - angleLimit / 2;
                        else break;
                    case FaceMode.Down:
                        if (newAngle < 90 - angleLimit / 2)
                            return 90 - angleLimit / 2;
                        else if (newAngle > 90 + angleLimit / 2)
                            return 90 + angleLimit / 2;
                        else break;
                }

                return newAngle;
            }
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

