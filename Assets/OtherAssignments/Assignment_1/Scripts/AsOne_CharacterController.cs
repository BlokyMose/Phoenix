using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    public class AsOne_CharacterController : MonoBehaviour
    {
        #region [Vars: Properties]

        [SerializeField]
        int maxHealth = 100;

        [SerializeField]
        int attackDamage = 10;

        [SerializeField]
        int maxActionCount = 1;
        public int ActionCount => maxActionCount;

        [SerializeField]
        Vector2Int cellsSize = new Vector2Int(1, 1);

        [SerializeField]
        Color moveTileColor = new Color(0.4f, 0.8f, 0.4f, 1f);        
        
        [SerializeField]
        Color hoverTileColor = new Color(0.25f, 0.95f, 0.25f, 1f);

        [SerializeField]
        Color attackTileColor = new Color(0.9f, 0.2f, 0.2f, 0.75f);

        [SerializeField]
        Animator animator;

        #endregion

        #region [Vars: Data Handlers]

        Vector2 pointerPos;

        int currentActionCount;

        int currentHealth;

        Color originalColor;

        const int moveDistance = 1;

        List<Cell> actionableCells;
        List<Cell> currentCells;
        Cell currentHoveredCell;

        #endregion

        Action OnActionDone;
        Action<int> OnAction;
        Func<List<AsOne_CharacterController>> GetCharacters;
        Func<Grid> GetGrid;

        public void Init(Cell startingCell, 
            Action<int> onAction,
            Action onActionDone,
            Func<List<AsOne_CharacterController>> getCharacters,
            Func<Grid> getGrid
            )
        {
            this.OnAction = onAction;
            this.OnActionDone = onActionDone;
            this.GetCharacters = getCharacters;
            this.GetGrid = getGrid;

            currentHealth = maxHealth;
            transform.position = startingCell.Position;
            currentCells = GetGrid().GetCells(startingCell, cellsSize);
            actionableCells = GetActionableCells(startingCell, currentCells, GetGrid());
        }

        List<Cell> GetActionableCells(Cell startingCell, List<Cell> currentCells, Grid grid)
        {
            var targetLocations = new List<Vector2Int>()
            {
                new Vector2Int(startingCell.GridLocation.x, startingCell.GridLocation.y+1),
                new Vector2Int(startingCell.GridLocation.x, startingCell.GridLocation.y-1),
                new Vector2Int(startingCell.GridLocation.x+1, startingCell.GridLocation.y),
                new Vector2Int(startingCell.GridLocation.x-1, startingCell.GridLocation.y),
            };

            // Prevent having actionable cells in current cells
            foreach (var cell in currentCells)
            {
                for (int locIndex = targetLocations.Count - 1; locIndex >= 0; locIndex--)
                {
                    if (cell.GridLocation == targetLocations[locIndex])
                    {
                        targetLocations.RemoveAt(locIndex);
                    }
                }
            }

            return grid.GetCells(targetLocations);
        }

        // Current system doesn't support simple directions for WASD
        //public void Move(Vector2 direction)
        //{
        //    foreach (var cell in actionableCells)
        //    {
        //        if (cell.Direction == direction)
        //        {
        //            OnPointerWorldPos(cell.BoxCollider.transform.position);
        //            Move();
        //            break;
        //        }
        //    }
        //}

        public void Move()
        {
            if (currentActionCount <= 0) return;
            if (currentHoveredCell == null) return;

            var previousPositon = transform.position;
            // TODO: transition
            transform.position = currentHoveredCell.Position;
            if (IsPositionInvalid())
            {
                transform.position = previousPositon;
            }
            else
            {
                foreach (var cell in actionableCells)
                    cell.ResetColor();

                currentCells = GetGrid().GetCells(currentHoveredCell, cellsSize);
                actionableCells = GetActionableCells(currentHoveredCell, currentCells, GetGrid());
                DecreaseCurrentActionCount();
            }

            bool IsPositionInvalid()
            {
                if (GetCharacters != null)
                    foreach (var character in GetCharacters())
                    {
                        if (character != this && transform.position == character.transform.position)
                            return true;
                    }
                return false;
            }
        }

        public void Attack()
        {
            if (currentActionCount <= 0) return;

            if (currentHoveredCell == null) return;

            if (GetCharacters != null)
                foreach (var character in GetCharacters())
                {
                    //if (character.transform.position == currentBoxInput.BoxCollider.transform.position)
                    //{
                    //    character.ReceiveAttack(attackDamage);
                    //    break;
                    //}
                }
            //currentBoxInput.Attack();
            DecreaseCurrentActionCount();
        }

        public void ReceiveAttack(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                Die();


            StartCoroutine(TurnRed(0.33f));
            IEnumerator TurnRed(float delay)
            {
                //sr.color = new Color(0.85f, 0.15f, 0.15f, 1f);
                yield return new WaitForSeconds(delay);
                //sr.color = originalColor;
            }
        }

        public void Die()
        {
            currentHealth = 0;
        }

        public void OnPointerWorldPos(Vector2 pointerPos)
        {
            this.pointerPos = pointerPos;

            UnhoverAllBoxInputs();
            currentHoveredCell = CheckMouseInsideAnyActionalCells();
            // TODO: prevent move to enemy's current cells; allow attack only

            void UnhoverAllBoxInputs()
            {
                foreach (var cell in actionableCells)
                {
                    cell.SetColor(moveTileColor);
                }
            }

            Cell CheckMouseInsideAnyActionalCells()
            {
                foreach (var cell in actionableCells)
                {
                    if (IsMouseInsideBox(pointerPos, cell))
                    {
                        cell.SetColor(hoverTileColor);
                        return cell;
                    }
                }

                return null;
            }
        }

        public void SetFullActionCount()
        {
            actionableCells = GetActionableCells(currentCells[0], currentCells, GetGrid());
            currentActionCount = maxActionCount;
            foreach (var cell in actionableCells)
                cell.SetColor(moveTileColor);
        }

        void DecreaseCurrentActionCount()
        {
            currentActionCount--;
            if (currentActionCount <= 0)
            {
                foreach (var cell in actionableCells)
                    cell.ResetColor();

                actionableCells.Clear();
                OnActionDone?.Invoke();
            }
        }

        bool IsMouseInsideBox(Vector2 pointerPos, Cell cell)
        {
            return (pointerPos.x < cell.Position.x + cell.Size.x / 2 &&
                    pointerPos.x > cell.Position.x - cell.Size.x / 2 &&
                    pointerPos.y < cell.Position.y + cell.Size.x / 2 &&
                    pointerPos.y > cell.Position.y - cell.Size.x / 2);

        }
    }
}
