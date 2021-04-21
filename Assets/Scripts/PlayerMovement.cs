using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake.Map;
using System;
using System.Linq;
namespace Snake.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] const float moveTickSpeed = 0.2f;

        [SerializeField] MapController mapController;


        GridSystem gridSystem;

        LineRenderer lr;

        List<(int, int)> lrTurnPositions = new List<(int, int)>();

        List<Action> moveQueue = new List<Action>();

        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        Direction currentDirection;
        Direction previousDirection;

        int column;
        int row;

        int lastColumn;
        int lastRow;

        int currentSnakeLength = 3;
        bool wasMoveKeyClicked;

        string playerTag = "Snake";
        string scoreTag = "Score";
        string obstacleTag = "Obstacle";

        float timer;


        private void Start()
        {
            lr = GetComponent<LineRenderer>();

            currentDirection = Direction.Right;
            previousDirection = currentDirection;

            gridSystem = mapController.GridSystem;

            column = Mathf.RoundToInt(GridSystem.amountOfColumns / 2);
            row = Mathf.RoundToInt(GridSystem.amountOfRows / 2);

            lastColumn = column - (currentSnakeLength - 1);
            lastRow = row;

            lr.SetPosition(1, gridSystem.positionsGrid[column, row]);
            lr.SetPosition(0, gridSystem.positionsGrid[lastColumn, lastRow]);

            gridSystem.SetGridAsBlocked(column, row, playerTag);

            // Initialize first moves to match start snake shape

            for (int i = 0; i < currentSnakeLength - 1; i++)
            {
                moveQueue.Add(MoveEndRight);
            }

        }
        private void Update()
        {
            timer += Time.deltaTime;

            InputMove();
            TickMove();
        }
        void InputMove()
        {
            if (!wasMoveKeyClicked)
            {
                if (Input.GetKeyDown(KeyCode.W) && previousDirection != Direction.Down)
                {
                    wasMoveKeyClicked = true;
                    currentDirection = Direction.Up;
                }
                else if (Input.GetKeyDown(KeyCode.S) && previousDirection != Direction.Up)
                {
                    wasMoveKeyClicked = true;
                    currentDirection = Direction.Down;
                }
                else if (Input.GetKeyDown(KeyCode.A) && previousDirection != Direction.Right)
                {
                    wasMoveKeyClicked = true;
                    currentDirection = Direction.Left;
                }
                else if (Input.GetKeyDown(KeyCode.D) && previousDirection != Direction.Left)
                {
                    wasMoveKeyClicked = true;
                    currentDirection = Direction.Right;
                }
            }

        }
        void TickMove()
        {
            if (timer >= moveTickSpeed)
            {
                timer = 0;
                wasMoveKeyClicked = false;

                switch (currentDirection)
                {
                    case Direction.Up:
                        moveQueue.Add(MoveEndUp);
                        MoveHeadUp();
                        break;
                    case Direction.Down:
                        moveQueue.Add(MoveEndDown);
                        MoveHeadDown();
                        break;
                    case Direction.Left:
                        moveQueue.Add(MoveEndLeft);
                        MoveHeadLeft();
                        break;
                    case Direction.Right:

                        moveQueue.Add(MoveEndRight);
                        MoveHeadRight();
                        break;
                }

                //   moveQueue.Add(currentEndMove);



              

                GridCellContentInteraction();

                previousDirection = currentDirection;

            }
        }
        void GridCellContentInteraction()
        {

            if (gridSystem.WhatIsInCell(column, row) == null)
            {
                moveQueue[0].Invoke();
                DeleteTurn();
                moveQueue.RemoveAt(0);
                gridSystem.SetGridAsFree(lastColumn, lastRow);
                gridSystem.SetGridAsBlocked(column, row, playerTag);
            }
            else
            {
                if (gridSystem.WhatIsInCell(column, row) == scoreTag)
                {
                    gridSystem.SetGridAsBlocked(column, row, playerTag);

                    mapController.SetScoreSquare();
                    Debug.Log("Score!");
                }
                else if(gridSystem.WhatIsInCell(column,row)== obstacleTag)
                {
                    
                    if (moveQueue[0] != null)
                    {
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                    }
                    if (moveQueue[0] != null)
                    {
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                    }
                    else
                    {
                        Debug.Log("GameOver");
                    }
                    mapController.SetObstacleSquare();

                }
                else if (gridSystem.WhatIsInCell(column, row) == playerTag)
                {
                    Debug.Log("GameOver");
                    Time.timeScale = 0;
                }
            }
        }
        #region DirectionMoveMethods
        void SetTurn()
        {
            if (currentDirection != previousDirection)
            {
                lr.positionCount += 1;
                lr.SetPosition(lr.positionCount - 2, gridSystem.positionsGrid[column, row]);
                lrTurnPositions.Add((column, row));
            }

        }
        void DeleteTurn()
        {
            if (lrTurnPositions.Count > 0)
            {
                if ((lastColumn, lastRow) == lrTurnPositions[0])
                {
                    int newPositionCount = lr.positionCount - 1;
                    Vector3[] newPositions = new Vector3[newPositionCount];

                    for (int i = 0; i < newPositionCount; i++)
                    {
                        newPositions[i] = lr.GetPosition(i + 1);
                    }
                    lr.positionCount = newPositionCount;
                    lr.SetPositions(newPositions);
                    lrTurnPositions.RemoveAt(0);
                }
            }
        }
        void MoveHeadLeft()
        {
            SetTurn();
            column--;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        void MoveHeadRight()
        {
            SetTurn();
            column++;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        void MoveHeadUp()
        {
            SetTurn();
            row--;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        void MoveHeadDown()
        {
            SetTurn();
            row++;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        void MoveEndLeft()
        {
            lastColumn--;
            lr.SetPosition(0, gridSystem.positionsGrid[lastColumn, lastRow]);
        }
        void MoveEndRight()
        {
            lastColumn++;
            lr.SetPosition(0, gridSystem.positionsGrid[lastColumn, lastRow]);
        }
        void MoveEndUp()
        {
            lastRow--;
            lr.SetPosition(0, gridSystem.positionsGrid[lastColumn, lastRow]);
        }
        void MoveEndDown()
        {
            lastRow++;
            lr.SetPosition(0, gridSystem.positionsGrid[lastColumn, lastRow]);
        }
        #endregion;

    }
}

