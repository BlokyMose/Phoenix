using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    public class AsOne_GameManager : MonoBehaviour
    {
        [SerializeField]
        List<AsOne_CharacterController> characters = new List<AsOne_CharacterController> ();

        int currentCharacterIndex = 0;
        int currentCharacterActionCountLeft = 0;

        private void Start()
        {
            foreach (var character in characters)
            {
                character.OnAction += SetCurrentCountLeft;
                character.OnActionDone += NextTurn;
                character.GetCharacters += () => { return characters; };
            }

            NextTurn();
        }

        public void NextTurn()
        {
            characters[currentCharacterIndex].SetFullActionCount();
            currentCharacterActionCountLeft = characters[currentCharacterIndex].ActionCount;

            currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        }

        void SetCurrentCountLeft(int count)
        {
            currentCharacterActionCountLeft = count;
        }
    }
}
