using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;
using Vector2 = UnityEngine.Vector2;

namespace AsOne
{
    [CreateAssetMenu(menuName = "SO/AsOne/MM_Rat", fileName = "MM_Rat")]
    public class AsOne_MM_Rat : AsOne_MoveModule
    {
        public override List<Cell> GetMoveableCells(Cell pivotCell, List<Cell> currentCells, Grid grid)
        {
            var targetLocations = new List<Vector2Int>()
            {
                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y+2),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y+2),

                new Vector2Int(pivotCell.GridLocation.x, pivotCell.GridLocation.y-1),
                new Vector2Int(pivotCell.GridLocation.x+1, pivotCell.GridLocation.y-1),

                new Vector2Int(pivotCell.GridLocation.x+2, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x+2, pivotCell.GridLocation.y+1),

                new Vector2Int(pivotCell.GridLocation.x-1, pivotCell.GridLocation.y),
                new Vector2Int(pivotCell.GridLocation.x-1, pivotCell.GridLocation.y+1),
            };

            return grid.GetCells(targetLocations);
        }

        public override Cell Move(Cell pivotCell, Cell clickedCell, Grid grid)
        {
            var targetLocation = pivotCell.GridLocation;

            if (clickedCell.GridLocation.y - pivotCell.GridLocation.y == 2)
            {
                targetLocation = new Vector2Int(targetLocation.x, targetLocation.y+1);
            }
            else if (clickedCell.GridLocation.y - pivotCell.GridLocation.y == -1)
            {
                targetLocation = new Vector2Int(targetLocation.x, targetLocation.y - 1);

            }
            else if (clickedCell.GridLocation.x - pivotCell.GridLocation.x == 2)
            {
                targetLocation = new Vector2Int(targetLocation.x+1, targetLocation.y);

            }
            else if (clickedCell.GridLocation.x - pivotCell.GridLocation.x == -1)
            {
                targetLocation = new Vector2Int(targetLocation.x - 1, targetLocation.y);
            }

            return grid.GetCell(targetLocation);
        }

        public override List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, Grid grid)
        {
            var sameCells = new List<Cell>() { hoveredCell };

            if (hoveredCell.GridLocation.y - pivotCell.GridLocation.y == 2 ||
                hoveredCell.GridLocation.y - pivotCell.GridLocation.y == -1)
            {
                if (hoveredCell.GridLocation.x == pivotCell.GridLocation.x)
                {
                    sameCells.Add(grid.GetCell(new Vector2Int(hoveredCell.GridLocation.x + 1, hoveredCell.GridLocation.y)));
                }
                else
                {
                    sameCells.Add(grid.GetCell(new Vector2Int(hoveredCell.GridLocation.x - 1, hoveredCell.GridLocation.y)));
                }
            }
            else if (hoveredCell.GridLocation.x - pivotCell.GridLocation.x == 2 ||
                hoveredCell.GridLocation.x - pivotCell.GridLocation.x == -1)
            {
                if (hoveredCell.GridLocation.y == pivotCell.GridLocation.y)
                {
                    sameCells.Add(grid.GetCell(new Vector2Int(hoveredCell.GridLocation.x , hoveredCell.GridLocation.y + 1)));
                }
                else
                {
                    sameCells.Add(grid.GetCell(new Vector2Int(hoveredCell.GridLocation.x , hoveredCell.GridLocation.y - 1)));
                }
            }

            return sameCells;
        }
    }
}
