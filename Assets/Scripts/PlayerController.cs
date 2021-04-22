using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Snake.Map;

namespace Snake.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MapController mapController;
        [SerializeField] private SettingsSO settings;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text gameOverText;
        private GridSystem gridSystem;

        private LineRenderer lr;

        private readonly List<(int, int)> lrTurnPositions = new List<(int, int)>();
        private readonly List<Action> moveQueue = new List<Action>();

        private string playerTag;
        private string scoreTag;
        private string obstacleTag;
        private string wallTag;

        private int column;
        private int row;

        private int endColumn;
        private int endRow;

        private int currentSnakeLength = 3;

        private float moveTickSpeed;
        private float timer;

        private int score;

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private Direction currentDirection;
        private Direction previousDirection;

        void Start()
        {
            GetSettings();

            scoreText.text = score.ToString();

            // Get middle of the map
            column = Mathf.RoundToInt(GridSystem.amountOfColumns / 2);
            row = Mathf.RoundToInt(GridSystem.amountOfRows / 2);

            endColumn = column - (currentSnakeLength - 1);
            endRow = row;

            lr = GetComponent<LineRenderer>();

            //SetDirectionWhenStart
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
            if (timer >= moveTickSpeed)
            {
                TickMove();
                timer = 0;
            }
        }

        void GetSettings()
        {
            gridSystem = mapController.GridSystem;
            playerTag = settings.playerTag;
            scoreTag = settings.scoreTag;
            obstacleTag = settings.obstacleTag;
            wallTag = settings.wallTag;
            moveTickSpeed = settings.moveTickSpeed;
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
                    currentSnakeLength++;
                    score++;
                    scoreText.text = score.ToString();
                }
                else if (gridSystem.WhatIsInCell(column, row) == obstacleTag)
                {
                    currentSnakeLength--;
                    if (moveQueue.Count >= 2)
                    {
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);

                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                    }
                    if(currentSnakeLength<3)
                    {
                        GameOver();
                    }
                    score--;
                    scoreText.text = score.ToString();
                    mapController.SetObstacleSquare();

                }
                else
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
            if (column < 0)
            {
                GameOver();
            }
            else
            {
                lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
            }
        }
        void MoveHeadRight()
        {
            SetTurn();
            column++;
            if (column > GridSystem.amountOfColumns)
            {
                GameOver();
            }
            else
            {
                lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
            }

        }
        void MoveHeadUp()
        {
            SetTurn();
            row--;
            if (row <0)
            {
                GameOver();
            }
            else
            {
                lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
            }
           
        }
        void MoveHeadDown()
        {
            SetTurn();
            row++;
            if (row > GridSystem.amountOfRows)
            {
                GameOver();
            }
            else
            {
                lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
            }

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
            timer = 0;
            gameOverText.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}


