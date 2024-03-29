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
        CursorPack cursorPackMenu;

        [SerializeField]
        CursorDisplayerAnimation cursorDisplayerPrefab;

        [SerializeField]
        CursorDisplayerMenu cursorDisplayerMenuPrefab;

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

        CursorDisplayerAnimation cursorDisplayerGame;
        CursorDisplayerMenu cursorDisplayerMenu;
        bool isInitialized = false;

        #endregion

        #region [Methods: Initializations]

        public override void Init()
        {
            if (isInitialized) return;
            isInitialized = true;

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
            if (jet != null && cursorDisplayerPrefab != null)
            {
                cursorDisplayerGame = Instantiate(cursorDisplayerPrefab);
                cursorDisplayerGame.Init(this, jet.JetProperties);
                cursorDisplayerGame.OnCursorPositionWorld += (pos) => { OnCursorWorldPos?.Invoke(pos); };
                cursorDisplayerGame.Show(false);
            }

            if (cursorDisplayerMenuPrefab != null)
            {
                cursorDisplayerMenu = Instantiate(cursorDisplayerMenuPrefab);
                cursorDisplayerMenu.Init(this, 1f, cursorPackMenu);
                cursorDisplayerMenu.Show(true);
            }

            SetupFireInputMode(fireInputMode);

            base.Init();
        }

        public override void Init(LevelManager levelManager)
        {
            Init();
            if (cursorDisplayerGame!=null)
                cursorDisplayerGame.Show(true);
            if (cursorDisplayerMenu!=null)
                cursorDisplayerMenu.Show(false);
        }

        public override void Exit()
        {
            if (cursorDisplayerGame != null)
            {
                cursorDisplayerGame.Exit(this);
                cursorDisplayerGame.OnCursorPositionWorld -= (pos) => { OnCursorWorldPos?.Invoke(pos); };

                Destroy(cursorDisplayerGame);
            }
            if (cursorDisplayerMenu != null)
            {
                cursorDisplayerMenu.Exit(this);
                Destroy(cursorDisplayerMenu);
            }
            base.Exit();
        }

        public void DisplayCursorGame(bool activateGameplayComponents = true)
        {
            if (cursorDisplayerGame != null)
                cursorDisplayerGame.Show(true);
            if (cursorDisplayerMenu != null)
                cursorDisplayerMenu.Show(false);
            if(activateGameplayComponents)
                ActivateGameplayComponents();
        }

        public void DisplayCursorMenu(bool deactivateGameplayComponents = true)
        {
            if (cursorDisplayerGame != null)
                cursorDisplayerGame.Show(false);
            if (cursorDisplayerMenu != null)
                cursorDisplayerMenu.Show(true);
            if(deactivateGameplayComponents)
                DeactivateGameplayComponents();
        }

        public void ConnectToCursorDisplayer(FireController fireController)
        {
            if (cursorDisplayerGame != null)
                cursorDisplayerGame.Init(fireController);
        }        
        
        public void DisconnectFromCursorDisplayer(FireController fireController)
        {
            if (cursorDisplayerGame != null)
                cursorDisplayerGame.Exit(fireController);
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
            OnPointerPosInput?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnNextFireMode(InputAction.CallbackContext context)
        {
            if (context.started)
                OnNextFireModeInput?.Invoke();
        }

        public void OnNextBullet(InputAction.CallbackContext context)
        {
            if (context.started)
                OnNextBulletInput?.Invoke();
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            if (context.started)
                OnQuitInput?.Invoke();
        }

        #endregion

        #region [Methods]

        public void ActivateGameplayComponents()
        {
            if (TryGetComponent<FireController>(out var fireController))
                fireController.Activate();

            if (TryGetComponent<JetController>(out var jetController))
                jetController.Activate();
        }

        public void DeactivateGameplayComponents()
        {
            if (TryGetComponent<FireController>(out var fireController))
                fireController.Deactivate();

            if (TryGetComponent<JetController>(out var jetController))
                jetController.Deactivate();
        }

        public void DeactivateJetCollider()
        {
            if (TryGetComponent<JetController>(out var jetController))
                jetController.DeactivateJetCollider();
        }

        #endregion

    }
}
