using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Map
{
    public class GridSystem
    {
        public bool[,] boolsGrid = new bool[amountOfColumns, amountOfRows];
        public Vector2[,] positionsGrid = new Vector2[amountOfColumns, amountOfRows];

        public Vector2 middlePosition;
        public Vector2 topLeftGridPos;

        float gridHorSize = 0.5f;
        float gridVertSize = 0.5f;

        float offsetX = 0;
        float offsetY = 0;
        
        const int amountOfRows = 20;
        const int amountOfColumns = 20;

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
            InitializePositionsGrid();
        }
        #region Bools    
        public void SetGridAsBlocked(int column, int row)
        {
            if (AreIndexesProper(column, row))
            {
                if (IsGridFree(column, row))
                {
                    boolsGrid[row, column] = false;
                }
            }
        }
        public void SetGridAsFree(int column, int row)
        {
            if (AreIndexesProper(column, row))
            {
                if (!IsGridFree(column, row))
                {
                    boolsGrid[row, column] = false;
                }
            }
        }
        public bool IsGridFree(int column, int row)
        {
            if (AreIndexesProper(column, row))
            {
                if (boolsGrid[column, row])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
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
        #endregion;
        #region Positions
        public void InitializePositionsGrid()
        {
            int column = 0;
            int row = 0;
            for(int i = 1; i <= amountOfColumns * amountOfRows; i++)
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
        #endregion;
    }

}
