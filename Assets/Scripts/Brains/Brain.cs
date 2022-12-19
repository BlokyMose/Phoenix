using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Phoenix.PhoenixControls;

namespace Phoenix
{
    public abstract class Brain : MonoBehaviour
    {
        #region [Delegates]

        public Action<Vector2> OnMoveInput;
        public Action OnFireInput;
        public Action<bool> OnFiring;
        public Action<Vector2> OnPointerPosInput;
        public Action<Vector2> OnCursorWorldPos;
        public Action OnNextFireModeInput;
        public Action OnNextBulletInput;

        #endregion

        protected bool _isFiring = false;
        protected bool isFiring
        {
            get => _isFiring;
            set
            {
                if (_isFiring != value)
                    OnFiring?.Invoke(value);
                _isFiring = value;
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnDisable()
        {
            Exit();
        }

        public virtual void Init()
        {
            var fireController = GetComponent<FireController>();
            if (fireController != null)
                fireController.Init(this);
        }

        public virtual void Exit()
        {
            var fireController = GetComponent<FireController>();
            if (fireController != null)
                fireController.Exit(this);
        }
    }
}
