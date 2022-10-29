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
        public Action<Vector2> OnPointerPosInput;
        public Action<Vector2> OnCursorWorldPos;
        public Action OnFireModeInput;

        #endregion

        protected virtual void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
        }
    }
}
