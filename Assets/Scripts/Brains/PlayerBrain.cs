using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Phoenix.PhoenixControls;
using Cinemachine;
using Encore.Utility;
using Sirenix.OdinInspector;

namespace Phoenix
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerBrain : Brain, IPlayerActions
    {
        #region [Vars: Components]

        [SerializeField]
        CursorDisplayer cursorDisplayerPrefab;

        [SerializeField, Tooltip("Can be overridden by "+nameof(LevelManager))]
        bool instantiateVCam = true;

        [SerializeField, ShowIf(nameof(instantiateVCam))]
        CinemachineVirtualCamera vCamPrefab;

        public bool InstatiateVCam { get { return instantiateVCam; }  set { instantiateVCam = value; } }

        #endregion

        #region [Vars: Properties]

        public enum FireInputMode 
        { 
            [Tooltip("Hold button to keep firing")]
            Automatic, 
            [Tooltip("Click button fires one time")]
            SemiAutomatic, 
            [Tooltip("Click button to keep firing, click again to stop firing")]
            Toggle
        }
        [SerializeField]
        FireInputMode fireInputMode = FireInputMode.Automatic;


        #endregion


        #region [Vars: Data Handlers]

        CursorDisplayer cursorDisplayer;
        bool isFiring = false;

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
            cursorDisplayer.OnCursorPosition += (pos) => { OnCursorWorldPos?.Invoke(Camera.main.ScreenToWorldPoint(pos)); };

            SetupFireInputMode(fireInputMode);
        }

        Coroutine corAutomaticFireInput;
        public void SetupFireInputMode(FireInputMode fireInputMode)
        {
            this.fireInputMode = fireInputMode;

            switch (fireInputMode)
            {
                case FireInputMode.Automatic:
                    corAutomaticFireInput = this.RestartCoroutine(Firing());
                    break;
                case FireInputMode.SemiAutomatic:
                    if (corAutomaticFireInput != null) StopCoroutine(corAutomaticFireInput);
                    break;
                case FireInputMode.Toggle:
                    corAutomaticFireInput = this.RestartCoroutine(Firing());
                    break;
            }

            IEnumerator Firing()
            {
                while (true)
                {
                    if (isFiring)
                        OnFireInput?.Invoke();
                    yield return null;
                }
            }
        }

        #endregion

        #region [Methods: Input Handlers]

        public void OnFire(InputAction.CallbackContext context)
        {
            switch (fireInputMode)
            {
                case FireInputMode.Automatic:
                    if (context.started)
                        isFiring = true;
                    else if (context.canceled)
                        isFiring = false;
                    break;
                case FireInputMode.SemiAutomatic:
                    if (context.started)
                        OnFireInput?.Invoke();
                    break;
                case FireInputMode.Toggle:
                    if (context.started)
                        isFiring = !isFiring;
                    break;
            }
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
