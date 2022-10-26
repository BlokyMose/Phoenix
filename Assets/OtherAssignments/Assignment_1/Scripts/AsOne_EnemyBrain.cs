using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    [RequireComponent(typeof(AsOne_CharacterController))]
    public class AsOne_EnemyBrain : MonoBehaviour
    {
        AsOne_CharacterController characterController;

        private void Start()
        {
            characterController = GetComponent<AsOne_CharacterController>();


            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    //var rand = Random.Range(0, 4);
                    //if(rand==0) characterController.Move(Vector2.up);
                    //else if(rand==1) characterController.Move(Vector2.down);
                    //else if(rand==2) characterController.Move(Vector2.right);
                    //else if(rand==3) characterController.Move(Vector2.left);

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
