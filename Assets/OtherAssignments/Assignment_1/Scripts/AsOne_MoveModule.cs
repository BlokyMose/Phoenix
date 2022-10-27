using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    public abstract class AsOne_MoveModule : ScriptableObject
    {
        public abstract List<Cell> GetMoveableCells(Cell pivotCell, List<Cell> currentCells, Grid grid);
        public abstract Cell Move(Cell pivotCell, Cell clickedCell, Grid grid);
        public abstract List<Cell> GetSameCells(Cell pivotCell, Cell hoveredCell, Grid grid);

    }
}
