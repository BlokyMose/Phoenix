using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.JetComponents;

namespace Phoenix
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JetController : MonoBehaviour
    {
        #region [Classes]

        [Serializable]
        public class RotationSettings
        {
            [SerializeField, Range(0, 360)]
            float angleLimit = 360;
            public float AngleLimit => angleLimit;

            [SerializeField]
            Direction4 faceMode = Direction4.Right;
            public Direction4 FaceMode => faceMode;

            public RotationSettings(float angleLimit, Direction4 faceMode)
            {
                this.angleLimit = angleLimit;
                this.faceMode = faceMode;
            }

            public RotationSettings()
            {

            }
        }

        #endregion

        #region [Vars: Components]

        [SerializeField]
        JetProperties jetProperties;
        public JetProperties JetProperties { get { return jetProperties; } }

        [SerializeField]
        List<BulletProperties> bulletProperties = new List<BulletProperties>();

        Rigidbody2D rb;

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        RotationSettings rotationSettings = new RotationSettings();

        #endregion

        #region [Vars: Data Handlers]

        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!"+nameof(components)), PropertyOrder(-1)]
        JetComponents components;
        Vector2 moveDirection;
        float fireCooldown;
        bool isFiring = false;
        int currentFireModeIndex;
        int currentFireOrigin;
        BulletProperties currentBulletProperties;

        #endregion

        #region [Delegates]

        Func<Vector2> GetCursorWorldPosition;

        #endregion

        #region [Methods: Initialization]

        public void Init(Brain brain)
        {
            brain.OnMoveInput += (dir) => { moveDirection = dir; };
            brain.OnFireInput += (isFiring) => { this.isFiring = isFiring; };
            GetCursorWorldPosition += brain.GetCursorWorldPosition;
            brain.OnFireModeInput += () => { currentFireModeIndex = (currentFireModeIndex + 1) % components.FireModes.Count; };

            rb = GetComponent<Rigidbody2D>();
            rb.drag = jetProperties.linearDrag;
            InstantiateJet();
            currentFireModeIndex = 0;
            currentFireOrigin = 0;
            currentBulletProperties = bulletProperties[0];
        }

        public void Disable(Brain brain)
        {
            brain.OnMoveInput -= (dir) => { moveDirection = dir; };
            brain.OnFireInput -= (isFiring) => { this.isFiring = isFiring; };
            GetCursorWorldPosition -= brain.GetCursorWorldPosition;
        }

        void InstantiateJet()
        {
            if (components == null)
            {
                var foundJet = transform.Find("Jet");
                if (foundJet != null)
                {
                    components = foundJet.gameObject.GetComponent<JetComponents>();
                }
                else
                {
                    components = Instantiate(jetProperties.jetPrefab, transform);
                    components.name = "Jet";
                }
            }

        }

        #endregion

        void FixedUpdate()
        {
            RotateToCursor();
            Move();
            Fire();
        }

        void RotateToCursor()
        {
            var positionToCursor = (Vector2)transform.position - GetCursorWorldPosition();
            var newAngle = Mathf.Atan2(positionToCursor.y, positionToCursor.x) * Mathf.Rad2Deg;
            var validatedAngle = ValidateAngle(newAngle);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, validatedAngle));

            float ValidateAngle(float newAngle)
            {
                switch (rotationSettings.FaceMode)
                {
                    case Direction4.Right:
                        if (newAngle > 0 && newAngle < 180 - rotationSettings.AngleLimit / 2)
                            return 180 - rotationSettings.AngleLimit / 2;
                        else if (newAngle < 0 && newAngle > -180 + rotationSettings.AngleLimit / 2)
                            return -180 + rotationSettings.AngleLimit / 2;
                        else break;
                    case Direction4.Left:
                        if (newAngle > rotationSettings.AngleLimit / 2)
                            return rotationSettings.AngleLimit / 2;
                        else if (newAngle < -rotationSettings.AngleLimit / 2)
                            return -rotationSettings.AngleLimit / 2;
                        else break;
                    case Direction4.Up:
                        if (newAngle > -90 + rotationSettings.AngleLimit / 2)
                            return -90 + rotationSettings.AngleLimit / 2;
                        else if (newAngle < -90 - rotationSettings.AngleLimit / 2)
                            return -90 - rotationSettings.AngleLimit / 2;
                        else break;
                    case Direction4.Down:
                        if (newAngle < 90 - rotationSettings.AngleLimit / 2)
                            return 90 - rotationSettings.AngleLimit / 2;
                        else if (newAngle > 90 + rotationSettings.AngleLimit / 2)
                            return 90 + rotationSettings.AngleLimit / 2;
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
            var currentFireMode = components.FireModes[currentFireModeIndex];

            var bulletGO = new GameObject("Bullet");
            bulletGO.transform.position = currentFireMode.origins[currentFireOrigin].transform.position;
            bulletGO.transform.eulerAngles = currentFireMode.origins[currentFireOrigin].eulerAngles;
            var bullet = bulletGO.AddComponent<Bullet>();
            bullet.Init(currentBulletProperties);

            currentFireOrigin++;

            switch (currentFireMode.pattern)
            {
                case FireMode.FirePattern.Sequence: currentFireOrigin %= currentFireMode.origins.Count;
                    break;
                case FireMode.FirePattern.ConcurrentInstant:
                    if (currentFireOrigin / currentFireMode.origins.Count != 1)
                    {
                        fireCooldown = 0;
                        Fire();
                    }
                    else
                        currentFireOrigin = 0;
                    break;
                case FireMode.FirePattern.ConcurrentCooldown:
                    if (currentFireOrigin / currentFireMode.origins.Count != 1)
                        Fire();
                    else
                        currentFireOrigin = 0;
                    break;

            }
        }
    }
}

