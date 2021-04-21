using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake.Map;

namespace Snake.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]   MapController mapController;
        [SerializeField]   SettingsSO settings;

         GridSystem gridSystem;

        Vector2 previousHeadPos;
        Vector2 currentHeadPos;
        Vector2 headDestination;

        Vector2 previousEndPos;
        Vector2 currentEndPos;
        Vector2 endDestination;

         string playerTag;
         string scoreTag;
         string obstacleTag;

         int column;
         int row;

         int endColumn;
         int endRow;

         int currentSnakeLength = 3;

         float moveTickSpeed;
         float timer;

        LineRenderer lr;

        readonly List<(int, int)> lrTurnPositions = new List<(int, int)>();
        readonly List<Action> moveQueue = new List<Action>();

        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        Direction currentDirection;
        Direction previousDirection;

        void Start()
        {
            gridSystem = mapController.GridSystem;
            playerTag = settings.playerTag;
            scoreTag = settings.scoreTag;
            obstacleTag = settings.obstacleTag;
            moveTickSpeed = settings.moveTickSpeed;

            // Get middle of the map
            column = Mathf.RoundToInt(GridSystem.amountOfColumns / 2);
            row = Mathf.RoundToInt(GridSystem.amountOfRows / 2);

            endColumn = column - (currentSnakeLength - 1);
            endRow = row;

            lr = GetComponent<LineRenderer>();

            currentDirection = Direction.Right;
            previousDirection = currentDirection;

            lr.SetPosition(1, gridSystem.positionsGrid[column, row]);
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);

            gridSystem.SetGridAsBlocked(column, row, playerTag);

            // Initialize first moves to match start snake shape

            for (int i = 0; i < currentSnakeLength - 1; i++)
            {
                moveQueue.Add(MoveSnakeEndRight);
            }

        }
        void Update()
        {
            timer += Time.deltaTime;
            InputMove();
            currentHeadPos = Vector2.Lerp(previousHeadPos, headDestination, moveTickSpeed);
            currentEndPos = Vector2.Lerp(previousEndPos, endDestination, moveTickSpeed);
            lr.SetPosition(0, currentEndPos);
            lr.SetPosition(lr.positionCount - 1, currentHeadPos);
            if (timer >= moveTickSpeed)
            {
                TickMove();
                timer = 0;
            }
        }
        void InputMove()
        {
                if (Input.GetKeyDown(KeyCode.W) && previousDirection != Direction.Down)
                {
                    currentDirection = Direction.Up;
                }
                else if (Input.GetKeyDown(KeyCode.S) && previousDirection != Direction.Up)
                {
                    currentDirection = Direction.Down;
                }
                else if (Input.GetKeyDown(KeyCode.A) && previousDirection != Direction.Right)
                {
                    currentDirection = Direction.Left;
                }
                else if (Input.GetKeyDown(KeyCode.D) && previousDirection != Direction.Left)
                {
                    currentDirection = Direction.Right;
                }
        }
        void TickMove()
        {
            previousHeadPos = gridSystem.positionsGrid[column, row];
            previousEndPos = gridSystem.positionsGrid[endColumn, endRow];
            switch (currentDirection)
            {
                case Direction.Up:
                    moveQueue.Add(MoveSnakeEndUp);
                    MoveHeadUp();
                    break;
                case Direction.Down:
                    moveQueue.Add(MoveSnakeEndDown);
                    MoveHeadDown();
                    break;
                case Direction.Left:
                    moveQueue.Add(MoveSnakeEndLeft);
                    MoveHeadLeft();
                    break;
                case Direction.Right:

                    moveQueue.Add(MoveSnakeEndRight);
                    MoveHeadRight();
                    break;
            }

            GridCellContentInteraction();
            headDestination = gridSystem.positionsGrid[column, row];
            endDestination = gridSystem.positionsGrid[endColumn, endRow];
            // To prevent 180 degrees turn
            previousDirection = currentDirection;
        }
        void GridCellContentInteraction()
        {

            if (gridSystem.WhatIsInCell(column, row) == null)
            {
                moveQueue[0].Invoke();
                DeleteTurn();
                moveQueue.RemoveAt(0);
                gridSystem.SetGridAsFree(endColumn, endRow);
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
                else if (gridSystem.WhatIsInCell(column, row) == obstacleTag)
                {

                    if (moveQueue.Count >= 2)
                    {
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);

                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                    }

                    else
                    {
                        GameOver();
                    }
                    mapController.SetObstacleSquare();

                }
                else if (gridSystem.WhatIsInCell(column, row) == playerTag)
                {
                    GameOver();
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
                // if end position is the same as the last turn position
                if ((endColumn, endRow) == lrTurnPositions[0])
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
        void MoveSnakeEndLeft()
        {
            endColumn--;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        void MoveSnakeEndRight()
        {
            endColumn++;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        void MoveSnakeEndUp()
        {
            endRow--;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        void MoveSnakeEndDown()
        {
            endRow++;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        #endregion;

        void GameOver()
        {
            Debug.Log("GameOver");
            Time.timeScale = 0;
        }
    }
}


