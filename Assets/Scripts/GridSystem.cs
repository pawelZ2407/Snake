using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Map
{
    public class GridSystem
    {
        public const int amountOfColumns = 20;
        public const int amountOfRows = 20;

        Dictionary<(int, int), string> cellsContent = new Dictionary<(int, int), string>();
       
        public Vector2[,] positionsGrid = new Vector2[amountOfColumns, amountOfRows];

        public Vector2 middlePosition;
        public Vector2 topLeftGridPos;

        List<(int, int)> freeCells = new List<(int, int)>();
        float gridHorSize = 0.5f;
        float gridVertSize = 0.5f;

        float offsetX = 0;
        float offsetY = 0;

        public GridSystem(Vector2 gridMiddle, float xOffset, float yOffset)
        {

            offsetX = xOffset;
            offsetY = yOffset;

            middlePosition = gridMiddle;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            topLeftGridPos = new Vector2(middlePosition.x - ((amountOfColumns-1) * gridHorSize) / 2 +offsetX, middlePosition.y + ((amountOfRows-1) * gridVertSize) / 2+offsetY);
            InitializeContentDictAndFreeCells();
            InitializePositionsGrid();
        }

        private void InitializeContentDictAndFreeCells()
        {
            int column = 0;
            int row = 0;
            for (int i = 1; i <= amountOfColumns * amountOfRows; i++)
            {
                cellsContent[(column, row)] = null;
                freeCells.Add((column, row));
                if (row <= amountOfRows - 2)
                {
                    row++;
                }
                else
                {
                    column++;
                    row = 0;
                }
            }
        }
        public Vector2 SetContentInRandomFreeCell(string content)          // To CHANGE
        {
            (int,int) randomCell = freeCells[Random.Range(0, freeCells.Count)];
            cellsContent[randomCell] = content;
            return positionsGrid[randomCell.Item1, randomCell.Item2];
        }
        public void InitializePositionsGrid()
        {
            int column = 0;
            int row = 0;
            for (int i = 1; i <= amountOfColumns * amountOfRows; i++)
            {
                positionsGrid[column, row] = new Vector2(topLeftGridPos.x + gridHorSize * column, topLeftGridPos.y - gridVertSize * row);
                if (row <= amountOfRows - 2)
                {
                    row++;
                }
                else
                {
                    column++;
                    row = 0;
                }
            }
            Debug.Log("TopLeftPos: " + topLeftGridPos);
            Debug.Log("EndPos: " + positionsGrid[amountOfColumns - 1, amountOfRows - 1]);

        }
   
        public void SetGridAsBlocked(int column, int row, string content)
        {
            if (AreIndexesProper(column, row))
            {            
                cellsContent[(column, row)] = content;
                if (freeCells.Contains((column, row)))
                {
                    freeCells.Remove((column, row));
                }
            }
        }
        public void SetGridAsFree(int column, int row)
        {
            if (AreIndexesProper(column, row))
            {
                cellsContent[(column, row)] = null;
                if (!freeCells.Contains((column, row)))
                {
                    freeCells.Add((column, row));
                }
            }
        }  
        public string WhatIsInCell(int column, int row)
        {
            return cellsContent[(column,row)];
        }
        private bool AreIndexesProper(int column, int row)
        {
            if (column > amountOfRows - 1 || column < 0)
            {
                Debug.LogError("Column index is out of range!");
                return false;
            }
            if (row > amountOfRows - 1 || row < 0)
            {
                Debug.LogError("Row index is out of range!");
                return false;
            }
            return true;
        }
    }

}
