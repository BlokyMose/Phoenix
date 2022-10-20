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
                    if(rand==0) characterController.Move(Vector2.up);
                    else if(rand==1) characterController.Move(Vector2.down);
                    else if(rand==2) characterController.Move(Vector2.right);
                    else if(rand==3) characterController.Move(Vector2.left);

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
