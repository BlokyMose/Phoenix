using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static AsOne.AsOneInput;

namespace AsOne
{
    [RequireComponent(typeof(AsOne_CharacterController))]
    public class AsOne_PlayerBrain : MonoBehaviour, IPlayerActions
    {
        AsOne_CharacterController characterController;

        private void Awake()
        {
            var input = new AsOneInput();
            input.Player.SetCallbacks(this);
            input.Enable();

            characterController = GetComponent<AsOne_CharacterController>();
        }

        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.started)
                characterController.AutoAction();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            //if (context.started)
            //    characterController.Attack();
        }

        public void OnPointerPos(InputAction.CallbackContext context)
        {
            characterController.OnPointerWorldPos(Camera.main.ScreenToWorldPoint( context.ReadValue<Vector2>()));
        }

        public void OnMove(InputAction.CallbackContext context)
        {
           
        }
    }
}
