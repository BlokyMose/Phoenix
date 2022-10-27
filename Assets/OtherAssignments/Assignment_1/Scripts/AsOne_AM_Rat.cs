using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;
using Vector2 = UnityEngine.Vector2;

namespace AsOne
{
    [CreateAssetMenu(menuName = "SO/AsOne/AM_Rat", fileName = "AM_Rat")]
    public class AsOne_AM_Rat : AsOne_AttackModule
    {
        public override List<Cell> GetAttackableCells(Cell pivotCell, List<Cell> currentCells, Grid grid)
        {
            var targetLocations = new List<Vector2Int>()
            {
                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y+2),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y+2),
                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y+3),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y+3),

                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y-1),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y-1),
                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y-2),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y-2),

                new Vector2Int(pivotCell.GridLocation.x+2, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x+2, pivotCell.GridLocation.y+1),
                new Vector2Int(pivotCell.GridLocation.x+3, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x+3, pivotCell.GridLocation.y+1),

                new Vector2Int(pivotCell.GridLocation.x-1, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x-1, pivotCell.GridLocation.y+1),
                new Vector2Int(pivotCell.GridLocation.x-2, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x-2, pivotCell.GridLocation.y+1),

            };

            return grid.GetCells(targetLocations);
        }

        public override List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, Grid grid)
        {
            var sameCells = new List<Cell>() { };
            Cell pivotAttackCell = null;

            if (hoveredCell.GridLocation.y - pivotCell.GridLocation.y >= 2)
            {
                pivotAttackCell = grid.GetCell(new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y + 2));
            }
            else if (hoveredCell.GridLocation.y - pivotCell.GridLocation.y <= -1)
            {
                pivotAttackCell = grid.GetCell(new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y - 2));
            }
            else if (hoveredCell.GridLocation.x - pivotCell.GridLocation.x >= 2)
            {
                pivotAttackCell = grid.GetCell(new Vector2Int(pivotCell.GridLocation.x+2, pivotCell.GridLocation.y));
            }
            else if (hoveredCell.GridLocation.x - pivotCell.GridLocation.x <= -1)
            {
                pivotAttackCell = grid.GetCell(new Vector2Int(pivotCell.GridLocation.x - 2, pivotCell.GridLocation.y));
            }

            if (pivotAttackCell!=null)
                sameCells.AddRange(grid.GetCells(pivotAttackCell, new Vector2Int(2, 2)));

            return sameCells;
        }
    }
}
