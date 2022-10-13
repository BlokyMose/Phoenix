using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Phoenix.PhoenixControls;

namespace Phoenix
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerBrain : MonoBehaviour, IPlayerActions
    {

        #region [Vars: Components]

        [SerializeField]
        CursorDisplayer cursorDisplayerPrefab;

        #endregion


        #region [Vars: Data Handlers]

        CursorDisplayer cursorDisplayer;

        #endregion

        #region [Delegates]

        public Action<Vector2> OnPointerPosInput;
        public Action<Vector2> OnMoveInput;
        public Action<bool> OnFireInput;

        #endregion

        #region [Methods: Initializations]

        private void Awake()
        {
            var controls = new PhoenixControls();
            controls.Player.SetCallbacks(this);
            controls.Enable();

            var jet = GetComponent<JetController>();

            Cursor.visible = false;
            cursorDisplayer = Instantiate(cursorDisplayerPrefab);
            cursorDisplayer.Init(ref OnPointerPosInput, jet.JetProperties.cursorSpeed, jet.JetProperties.cursor);
        }

        #endregion

        #region [Methods: Input Handlers]

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
                OnFireInput?.Invoke(true);
            else if (context.canceled)
                OnFireInput?.Invoke(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPointerPos(InputAction.CallbackContext context)
        {
            OnPointerPosInput(context.ReadValue<Vector2>());
        }

        #endregion

        #region [Methods: Getters]

        public Vector2 GetCursorWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(cursorDisplayer.GetCursorPosition());
        }


        #endregion
    }
}
