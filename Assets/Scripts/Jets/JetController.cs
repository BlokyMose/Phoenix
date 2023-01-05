using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.FireComponents;
using static Phoenix.JetProperties;

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

        [SerializeField, LabelText("Jet Properties")]
        JetProperties jetPropertiesNonStatic;
        JetPropertiesStatic jetPropertiesStatic;
        public JetPropertiesStatic JetProperties { get { AssignJetPropertiesStaticIfHasnt(); return jetPropertiesStatic; } }

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        RotationSettings rotationSettings = new RotationSettings();

        #endregion

        #region [Vars: Data Handlers]

        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!"+nameof(jetGO)), PropertyOrder(-1)]
        protected GameObject jetGO;
        protected Vector2 moveDirection;
        protected float reduceVelocityMultipler;
        protected bool isActive = true;

        #endregion

        #region [Delegates]

        Func<Vector2> GetCursorWorldPosition;
        Action<Vector2> OnMoveDirection;

        #endregion

        #region [Methods: Initialization]

        protected virtual void Awake()
        {
            Init();

            var brain = GetComponent<Brain>();
            if (brain != null)
                Init(brain);
        }

        protected virtual void OnDestroy()
        {
            var brain = GetComponent<Brain>();
            if (brain!=null)
                Exit(brain);

            StopAllCoroutines();
        }

        public void Init(Brain brain)
        {
            brain.OnMoveInput += SetMoveDirection;
            brain.OnCursorWorldPos += RotateToCursor;
            if (brain.TryGetComponent<HealthController>(out var healthController))
            {
                healthController.OnDie += Deactivate;
                healthController.OnDie += DeactivateJetCollider;
            }
        }

        private void Init()
        {
            AssignJetPropertiesStaticIfHasnt();

            rb = GetComponent<Rigidbody2D>();
            rb.drag = jetPropertiesStatic.LinearDrag;

            switch (jetPropertiesStatic.Mode)
            {
                case MoveMode.Smooth:
                    reduceVelocityMultipler = 0f;
                    break;
                case MoveMode.Constant:
                    reduceVelocityMultipler = 1f;
                    break;
            }

            InstantiateJet();
        }

        private void AssignJetPropertiesStaticIfHasnt()
        {
            if (jetPropertiesStatic == null)
            {
                if (jetPropertiesNonStatic is JetPropertiesStatic)
                {
                    jetPropertiesStatic = jetPropertiesNonStatic as JetPropertiesStatic;
                }
                else if (jetPropertiesNonStatic is JetPropertiesRandom)
                {
                    jetPropertiesStatic = (jetPropertiesNonStatic as JetPropertiesRandom).GenerateJetPropertiesStatic();

                }
            }
        }

        public void Exit(Brain brain)
        {
            brain.OnMoveInput -= SetMoveDirection;
            brain.OnCursorWorldPos -= RotateToCursor;
            if (brain.TryGetComponent<HealthController>(out var healthController))
            {
                healthController.OnDie -= Deactivate;
                healthController.OnDie -= DeactivateJetCollider;
            }

        }

        void InstantiateJet()
        {
            if (jetGO == null)
            {
                var jet = transform.Find("Jet");
                if (jet == null)
                {
                    jetGO = Instantiate(jetPropertiesStatic.JetPrefab, transform).gameObject;
                    jetGO.name = "Jet";
                    jet = jetGO.transform;
                }
                else
                    jetGO = jet.gameObject;

                var audioController = jet.GetComponent<JetAudioController>();
                if (audioController != null)
                    audioController.Init(ref OnMoveDirection);
            }
        }

        #endregion

        protected virtual void FixedUpdate()
        {
            if (!isActive) return;

            Move(moveDirection);
        }

        protected virtual void SetMoveDirection(Vector2 dir)
        {
            moveDirection = dir;
            OnMoveDirection?.Invoke(dir);
        }

        protected virtual void RotateToCursor(Vector2 cursorPos)
        {
            var positionToCursor = (Vector2)transform.position - cursorPos;
            var newAngle = Mathf.Atan2(positionToCursor.y, positionToCursor.x) * Mathf.Rad2Deg + 90;
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

        protected virtual void Move(Vector2 moveDirection)
        {
            if (rb.velocity.magnitude < jetPropertiesStatic.MaxVelocity)
                rb.AddForce((jetPropertiesStatic.MoveSpeed * Time.deltaTime * moveDirection) - (rb.velocity*reduceVelocityMultipler), ForceMode2D.Impulse);
        }

        public virtual void Activate()
        {
            isActive = true;
        }

        public virtual void Deactivate()
        {
            isActive = false;
        }

        public virtual void DeactivateJetCollider()
        {
            if(jetGO.TryGetComponent<Collider2D>(out var col))
            {
                col.enabled = false;
            }
        }

        public virtual void DestroyJetGO()
        {
            Destroy(jetGO);
        }
    }
}

