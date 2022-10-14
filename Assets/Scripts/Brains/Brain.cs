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
        public Action<bool> OnFireInput;
        public Action<Vector2> OnPointerPosInput;
        public Action OnFireModeInput;

        #endregion

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnEnable()
        {
            var jet = GetComponent<JetController>();
            jet.Init(this);
        }

        protected virtual void OnDisable()
        {
            var jet = GetComponent<JetController>();
            jet.Disable(this);
        }

        public virtual void Init()
        {
        }


        public abstract Vector2 GetCursorWorldPosition();

    }
}
