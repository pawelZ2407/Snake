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

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private Direction currentDirection;
        private Direction previousDirection;

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


        #region UnityMethodsAndInit
        private void Start()
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
        private void Update()
        {
            timer += Time.deltaTime;
            InputMove();
            if (timer >= moveTickSpeed)
            {
                TickMove();
                timer = 0;
            }
        }

        private void GetSettings()
        {
            gridSystem = mapController.GridSystem;
            playerTag = settings.playerTag;
            scoreTag = settings.scoreTag;
            obstacleTag = settings.obstacleTag;
            wallTag = settings.wallTag;
            moveTickSpeed = settings.moveTickSpeed;
        }
        #endregion;
        private void InputMove()
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
        private void TickMove()
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    if (row <= 0)
                    {
                        GameOver();
                        return;
                    }

                    moveQueue.Add(MoveSnakeEndUp);
                    MoveHeadUp();
                    break;
                case Direction.Down:
                    if (row >= GridSystem.amountOfRows-1)
                    {
                        GameOver();
                        return;
                    }
                    moveQueue.Add(MoveSnakeEndDown);
                    MoveHeadDown();
                    break;
                case Direction.Left:
                    if (column <= 0)
                    {
                        GameOver();
                        return;
                    }
                    moveQueue.Add(MoveSnakeEndLeft);
                    MoveHeadLeft();
                    break;
                case Direction.Right:
                    if (column >= GridSystem.amountOfColumns-1)
                    {
                        GameOver();
                        return;
                    }
                    moveQueue.Add(MoveSnakeEndRight);
                    MoveHeadRight();
                    break;
            }

            GridCellContentInteraction();

            // To prevent 180 degrees turn
            previousDirection = currentDirection;
        }
        // Collision handler 
        private void GridCellContentInteraction()
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
                        gridSystem.SetGridAsFree(endColumn, endRow);
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                        gridSystem.SetGridAsFree(endColumn, endRow);
                        moveQueue[0].Invoke();
                        DeleteTurn();
                        moveQueue.RemoveAt(0);
                        gridSystem.SetGridAsFree(endColumn, endRow);
                    }
                    if(currentSnakeLength<3)
                    {
                        GameOver();
                    }
                    score--;
                    scoreText.text = score.ToString();
                    gridSystem.SetGridAsFree(column, row);
                    mapController.SetObstacleSquare();

                }
                else if(gridSystem.WhatIsInCell(column, row) == wallTag || gridSystem.WhatIsInCell(column, row) == playerTag)
                {
                    GameOver();
                }
            }
        }
        #region MoveMethods
        private void SetTurn()
        {
            if (currentDirection != previousDirection)
            {
                lr.positionCount += 1;
                lr.SetPosition(lr.positionCount - 2, gridSystem.positionsGrid[column, row]);
                lrTurnPositions.Add((column, row));
            }

        }
        private void DeleteTurn()
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

        private void MoveHeadLeft()
        {
            SetTurn();

            column--;
                lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        private void MoveHeadRight()
        {
            SetTurn();

            column++;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        private void MoveHeadUp()
        {
            SetTurn();
   

            row--;
             lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        private void MoveHeadDown()
        {
            SetTurn();

            row++;
            lr.SetPosition(lr.positionCount - 1, gridSystem.positionsGrid[column, row]);
        }
        private void MoveSnakeEndLeft()
        {
            endColumn--;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        private void MoveSnakeEndRight()
        {
            endColumn++;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        private void MoveSnakeEndUp()
        {
            endRow--;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        private void MoveSnakeEndDown()
        {
            endRow++;
            lr.SetPosition(0, gridSystem.positionsGrid[endColumn, endRow]);
        }
        #endregion;

        private void GameOver()
        {
            Debug.Log("Dead reason: " + gridSystem.WhatIsInCell(column, row));
            timer = 0;
            gameOverText.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}


