using Encore.Utility;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.FireComponents;

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

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        RotationSettings rotationSettings = new RotationSettings();

        #endregion

        #region [Vars: Data Handlers]

        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!"+nameof(jetGO)), PropertyOrder(-1)]
        GameObject jetGO;
        Vector2 moveDirection;

        #endregion

        #region [Delegates]

        Func<Vector2> GetCursorWorldPosition;

        #endregion

        #region [Methods: Initialization]

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void OnEnable()
        {
            var brain = GetComponent<Brain>();
            if (brain != null)
                Init(brain);
        }

        void OnDisable()
        {
            var brain = GetComponent<Brain>();
            if (brain!=null)
                Disable(brain);
        }

        public void Init(Brain brain)
        {
            brain.OnMoveInput += (dir) => { moveDirection = dir; };
            brain.OnCursorWorldPos += RotateToCursor;

            rb.drag = jetProperties.LinearDrag;
            InstantiateJet();
        }

        public void Disable(Brain brain)
        {
            brain.OnMoveInput -= (dir) => { moveDirection = dir; };
            brain.OnCursorWorldPos -= RotateToCursor;
        }

        void InstantiateJet()
        {
            if (jetGO == null)
            {
                jetGO = transform.Find("Jet").gameObject;
                if (jetGO == null)
                {
                    jetGO = Instantiate(jetProperties.JetPrefab, transform).gameObject;
                    jetGO.name = "Jet";
                }
            }
        }

        #endregion

        void FixedUpdate()
        {
            Move(moveDirection);
        }

        void RotateToCursor(Vector2 cursorPos)
        {
            var positionToCursor = (Vector2)transform.position - cursorPos;
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

        void Move(Vector2 moveDirection)
        {
            if (rb.velocity.magnitude < jetProperties.MaxVelocity)
                rb.AddForce(moveDirection * jetProperties.MoveSpeed, ForceMode2D.Impulse);
        }

    }
}

