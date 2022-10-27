using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    [CreateAssetMenu(menuName = "SO/AsOne/MM_DefaultMovement", fileName = "MM_DefaultMovement")]
    public class AsOne_MM_Default : AsOne_MoveModule
    {
        public override List<Cell> GetMoveableCells(Cell startingCell, List<Cell> currentCells, AsOne_GameManager.Grid grid)
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

        public override List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, Grid grid)
        {
            return new List<Cell>() { hoveredCell };
        }

        public override Cell Move(Cell pivotCell, Cell clickedCell, Grid grid)
        {
            return clickedCell;
        }
    }
}
