using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    public abstract class AsOne_AttackModule : ScriptableObject
    {
        [SerializeField]
        int damage = 10;

        public int Damage => damage;
        public abstract List<Cell> GetAttackableCells(Cell pivotCell, List<Cell> currentCells, Grid grid);
        public abstract List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, Grid grid);
        public virtual List<AsOne_CharacterController> Attack(Cell pivotCell, Cell clickedCell, Grid grid, List<AsOne_CharacterController> characters)
        {
            var attackedCharacters = new List<AsOne_CharacterController>();
            var attackCells = GetSameCells(pivotCell, clickedCell, grid);

            foreach (var character in characters)
            {
                foreach (var cell in attackCells)
                {
                    if (character.CurrentCells.Contains(cell))
                    {
                        attackedCharacters.Add(character);
                        break;
                    }
                }
            }

            return attackedCharacters;
        }

        public virtual AsOne_CharacterController GetAttackableCharacter(Cell pivotCell, Cell hoveredCell, Grid grid, List<AsOne_CharacterController> characters)
        {
            var sameCells = GetSameCells(pivotCell, hoveredCell, grid);

            foreach (var character in characters)
            {
                foreach (var cell in sameCells)
                {
                    if (character.CurrentCells.Contains(hoveredCell))
                    {
                        return character;
                    }
                }
            }

            return null;
        }

    }
}
