using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] GameObject scoreSquarePrefab;
        [SerializeField] GameObject obstacleSquarePrefab;
        GameObject spawnedScoreSquare;
        GameObject spawnedObstacleSquare;
        GridSystem gridSystem;

        public GridSystem GridSystem
        {
            get { return gridSystem; }
            private set { gridSystem = value; }
        }

        [SerializeField] float xOffset;
        [SerializeField] float yOffset;

        string scoreTag = "Score";
        string obstacleTag = "Obstacle"; //To SO with settings
        void Awake()
        {
            gridSystem = new GridSystem(transform.position, xOffset, yOffset);
            spawnedScoreSquare = Instantiate(scoreSquarePrefab);
            spawnedObstacleSquare = Instantiate(obstacleSquarePrefab);
        }
        private void Start()
        {
            SetScoreSquare();
            SetObstacleSquare();
        }
        public void SetScoreSquare()
        {
            spawnedScoreSquare.transform.position = gridSystem.BlockRandomFreeCell(scoreTag);
        }
        public void SetObstacleSquare()
        {
            spawnedObstacleSquare.transform.position = gridSystem.BlockRandomFreeCell(obstacleTag);
        }
    }
}
