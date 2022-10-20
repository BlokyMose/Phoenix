using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    public class AsOne_GameManager : MonoBehaviour
    {
        [SerializeField]
        AsOne_CharacterController player;

        [SerializeField]
        AsOne_CharacterController enemy;

        int currentTurn = 0;

        private void Start()
        {
            player.OnActionDone += NextTurn;
            enemy.OnActionDone += NextTurn;

            NextTurn();
        }

        public void NextTurn()
        {
            if (currentTurn % 2 == 0)
                player.SetCanAction(true);
            else
                enemy.SetCanAction(true);
            currentTurn++;
        }
    }
}
