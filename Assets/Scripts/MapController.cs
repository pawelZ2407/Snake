using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] SettingsSO settings;

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

        [SerializeField,Tooltip("Grid X offset")] float xOffset;
        [SerializeField, Tooltip("Grid Y offset")] float yOffset;

        string scoreTag;
        string obstacleTag;
        void Awake()
        {
            scoreTag = settings.scoreTag;
            obstacleTag = settings.obstacleTag;
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
            spawnedScoreSquare.transform.position = gridSystem.SetContentInRandomFreeCell(scoreTag);
        }
        public void SetObstacleSquare()
        {
            spawnedObstacleSquare.transform.position = gridSystem.SetContentInRandomFreeCell(obstacleTag);
        }
    }
}
