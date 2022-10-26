using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    public class AsOne_GameManager : MonoBehaviour
    {

        #region [Classes]

        public class Cell
        {
            Vector2 size;
            public Vector2 Size => size;
            
            SpriteRenderer sr;
            public SpriteRenderer SR => sr;

            Vector2Int gridLocation;
            public Vector2Int GridLocation => gridLocation;

            Color originalColor;

            public Cell(Vector2Int gridLocation, SpriteRenderer sr, Vector2 size)
            {
                this.gridLocation = gridLocation;
                this.sr = sr;
                this.size = size;
                this.originalColor = sr.color;
            }

            public void SetColor(Color color)
            {
                sr.color = color;
            }

            public void ResetColor()
            {
                sr.color = originalColor;
            }

            public Vector2 Position => sr.transform.position;
        }

        [Serializable]
        public class Grid
        {
            List<Cell> cells = new List<Cell>();
            public List<Cell> Cells => cells;

            [SerializeField]
            Transform parent;
            public Transform Parent => parent;

            [SerializeField]
            List<GameObject> tilePrefabs = new List<GameObject>();

            [SerializeField]
            Vector2 gridSize = new Vector2Int(8, 8);

            [SerializeField]
            Vector2 cellSpace = new Vector2(1, 1);

            public enum TilePattern { None, Zebra }
            [SerializeField]
            TilePattern pattern = TilePattern.Zebra;

            public Grid(Transform parent, List<GameObject> tilePrefabs, Vector2 gridSize, Vector2 cellSpace, TilePattern pattern)
            {
                this.parent = parent;
                this.tilePrefabs = tilePrefabs;
                this.gridSize = gridSize;
                this.cellSpace = cellSpace;
                this.pattern = pattern;

                this.cells = GenerateCells();
            }

            public Grid(Grid grid)
            {
                this.parent = grid.parent;
                this.tilePrefabs = grid.tilePrefabs;
                this.gridSize = grid.gridSize;
                this.cellSpace = grid.cellSpace;
                this.pattern = grid.pattern;

                this.cells = GenerateCells();
            }

            public List<Cell> GenerateCells()
            {
                var cells = new List<Cell>();

                for (int row = 0; row < gridSize.y; row++)
                {
                    int tileIndex = GetStartTile(row, tilePrefabs.Count, pattern);

                    for (int col = 0; col < gridSize.x; col++)
                    {
                        var position = new Vector2(col * cellSpace.x, row * cellSpace.y);
                        var tile = Instantiate(tilePrefabs[tileIndex], parent);
                        tile.transform.localPosition = position;

                        var sr = tile.GetComponent<SpriteRenderer>();
                        var location = new Vector2Int(col, row);
                        var cell = new Cell(location, sr, cellSpace);
                        cells.Add(cell);

                        tileIndex = (tileIndex + 1) % tilePrefabs.Count;
                    }
                }

                return cells;

                int GetStartTile(int row, int tilePrefabsCount, TilePattern pattern)
                {
                    switch (pattern)
                    {
                        case TilePattern.None:
                            return 0;
                        case TilePattern.Zebra:
                            return row % tilePrefabsCount;
                        default:
                            return 0;
                    }

                }
            }

            public List<Cell> GetCells(Cell currentCell, Vector2Int range)
            {
                var targetCells = new List<Cell>();

                for (int row = 0; row < range.y; row++)
                {
                    for (int col = 0; col < range.x; col++)
                    {
                        var location = new Vector2Int(currentCell.GridLocation.x+col, currentCell.GridLocation.y+row);
                        var targetCell = GetCell(location);
                        if (targetCell != null)
                            targetCells.Add(targetCell);
                    }
                }

                return targetCells;
            }

            public List<Cell> GetCells(List<Cell> currentCells, Vector2Int size)
            {
                //int lowestIndex = 0;
                //Vector2Int lowestLocation = currentCells[lowestIndex].GridLocation;

                //int index = 0;
                //foreach (var cell in currentCells)
                //{
                //    if (cell.GridLocation.x < lowestLocation.x)
                //    {
                //        lowestLocation = cell.GridLocation;
                //        lowestIndex = index;
                //    }
                //    else if (cell.GridLocation.y < lowestLocation.y)
                //    {
                //        lowestLocation = cell.GridLocation;
                //        lowestIndex = index;
                //    }
                //    index++;
                //}

                return GetCells(currentCells[0], size);
            }

            public Cell GetCell(Vector2Int gridLocation)
            {
                foreach (var cell in cells)
                    if (cell.GridLocation == gridLocation) 
                        return cell;

                return null;
            }

            public List<Cell> GetCells(List<Vector2Int> targetLocations)
            {
                var targetCells = new List<Cell>();

                foreach (var cell in cells)
                {
                    for (int locIndex = targetLocations.Count - 1; locIndex >= 0; locIndex--)
                    {
                        if (cell.GridLocation == targetLocations[locIndex])
                        {
                            targetCells.Add(cell);
                            targetLocations.RemoveAt(locIndex);
                            if (targetCells.Count == 0)
                                return targetCells;
                        }
                    }
                }

                return targetCells;
            }
        }

        #endregion


        [SerializeField]
        List<AsOne_CharacterController> characters = new List<AsOne_CharacterController> ();

        int currentCharacterIndex = 0;
        int currentCharacterActionCountLeft = 0;

        [SerializeField]
        Grid grid;

        private void Start()
        {
            grid = new Grid(grid);


            // Testing
            characters[0].Init(
                grid.GetCell(new Vector2Int(0, 0)), 
                SetCurrentCountLeft,
                NextTurn,
                () => { return characters; },
                () => { return grid;  }
                );
            //characters[1].Init(
            //    grid.GetCell(new Vector2Int(4, 4)), 
            //    SetCurrentCountLeft,
            //    NextTurn,
            //    () => { return characters; },
            //    () => { return grid; }
            //    );

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
