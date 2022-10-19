using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Phoenix.PhoenixControls;
using Cinemachine;

namespace Phoenix
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerBrain : Brain, IPlayerActions
    {
        #region [Vars: Components]

        [SerializeField]
        CursorDisplayer cursorDisplayerPrefab;

        [SerializeField]
        CinemachineVirtualCamera vCamPrefab;

        [SerializeField]
        bool instantiateVCam = true;

        public bool InstatiateVCam { get { return instantiateVCam; }  set { instantiateVCam = value; } }

        #endregion

        #region [Vars: Data Handlers]

        CursorDisplayer cursorDisplayer;

        #endregion

        #region [Methods: Initializations]

        public override void Init()
        {
            base.Init();

            var controls = new PhoenixControls();
            controls.Player.SetCallbacks(this);
            controls.Enable();


            if (instantiateVCam)
            {
                var vCam = Instantiate(vCamPrefab);
                vCam.Follow = transform;
                if (Camera.main.GetComponent<CinemachineBrain>() == null)
                    Camera.main.gameObject.AddComponent<CinemachineBrain>();
            }

            var jet = GetComponent<JetController>();

            Cursor.visible = false;
            cursorDisplayer = Instantiate(cursorDisplayerPrefab);
            if (jet != null)
                cursorDisplayer.Init(ref OnPointerPosInput, jet.JetProperties);
        }

        #endregion

        #region [Methods: Getters]

        public override Vector2 GetCursorWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(cursorDisplayer.GetCursorPosition());
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

        public void OnFireMode(InputAction.CallbackContext context)
        {
            OnFireModeInput();
        }

        #endregion


    }
}
