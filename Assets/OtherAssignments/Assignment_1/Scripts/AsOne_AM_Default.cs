using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    [CreateAssetMenu(menuName = "SO/AsOne/AM_Default", fileName = "AM_Default")]

    public class AsOne_AM_Default : AsOne_AttackModule
    {
        public override List<Cell> GetAttackableCells(Cell startingCell, List<Cell> currentCells, AsOne_GameManager.Grid grid)
        {
            var targetLocations = new List<Vector2Int>()
            {
                new Vector2Int(startingCell.GridLocation.x, startingCell.GridLocation.y+1),
                new Vector2Int(startingCell.GridLocation.x, startingCell.GridLocation.y-1),
                new Vector2Int(startingCell.GridLocation.x+1, startingCell.GridLocation.y),
                new Vector2Int(startingCell.GridLocation.x-1, startingCell.GridLocation.y),
            };

            return grid.GetCells(targetLocations);
        }

        public override List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, AsOne_GameManager.Grid grid)
        {
            return new List<Cell>() { hoveredCell };
        }

    }
}
