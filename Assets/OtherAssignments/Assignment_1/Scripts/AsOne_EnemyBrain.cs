using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_CharacterController;

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
                    var attackableCells = characterController.GetAttackableCells();
                    if (attackableCells.Count > 0)
                    {
                        characterController.Attack(attackableCells[0]);
                    }
                    else
                    {
                        var rand = Random.Range(0, 4);
                        if (rand == 0) characterController.Move(MoveDirection.Up);
                        else if (rand == 1) characterController.Move(MoveDirection.Down);
                        else if (rand == 2) characterController.Move(MoveDirection.Right);
                        else if (rand == 3) characterController.Move(MoveDirection.Left);
                    }


                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
