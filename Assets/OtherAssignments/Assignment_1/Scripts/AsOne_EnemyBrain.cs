using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    [RequireComponent(typeof(AsOne_CharacterController))]
    public class AsOne_EnemyBrain : MonoBehaviour
    {
        AsOne_CharacterController characterController;

        AsOne_CharacterController.BoxInput boxInputUp;
        AsOne_CharacterController.BoxInput boxInputDown;
        AsOne_CharacterController.BoxInput boxInputRight;
        AsOne_CharacterController.BoxInput boxInputLeft;

        private void Start()
        {
            characterController = GetComponent<AsOne_CharacterController>();
            foreach (var box in characterController.AllBoxInputs)
            {
                if (box.Direction == Vector2.up)
                    boxInputUp = box;
                else if (box.Direction == Vector2.down)
                    boxInputDown = box;
                else if (box.Direction == Vector2.right)
                    boxInputRight = box;
                else if (box.Direction == Vector2.left)
                    boxInputLeft = box;
            }


            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    var rand = Random.Range(0, 4);
                    if(rand==0) Move(Vector2.up);
                    else if(rand==1) Move(Vector2.down);
                    else if(rand==2) Move(Vector2.right);
                    else if(rand==3) Move(Vector2.left);

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        void Move(Vector2 direction)
        {
            if(direction == Vector2.up)
            {
                characterController.OnPointerWorldPos(boxInputUp.BoxCollider.transform.position);
            }
            else if (direction == Vector2.down)
            {
                characterController.OnPointerWorldPos(boxInputDown.BoxCollider.transform.position);
            }
            else if (direction == Vector2.right)
            {
                characterController.OnPointerWorldPos(boxInputRight.BoxCollider.transform.position);
            }
            else if (direction == Vector2.left)
            {
                characterController.OnPointerWorldPos(boxInputLeft.BoxCollider.transform.position);
            }

            characterController.Move();
        }


    }
}
