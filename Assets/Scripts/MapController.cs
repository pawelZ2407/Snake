using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private SettingsSO settings;

        [SerializeField] private GameObject scoreSquarePrefab;
        [SerializeField] private GameObject obstacleSquarePrefab;
        [SerializeField] private GameObject wallSquarePrefab;
        private GameObject spawnedScoreSquare;
        private GameObject spawnedObstacleSquare;

        private Dictionary<GameObject, (int, int)> wallsCoords = new Dictionary<GameObject, (int, int)>();
        private List<GameObject> spawnedWallSquare = new List<GameObject>();

        private GridSystem gridSystem;
        public GridSystem GridSystem
        {
            get { return gridSystem; }
            private set { gridSystem = value; }
        }

        [SerializeField, Tooltip("Grid X offset")] 
        private float xOffset;
        [SerializeField, Tooltip("Grid Y offset")]
        private float yOffset;

        private string scoreTag;
        private string obstacleTag;
        private string wallTag;

        private readonly int wallsToSpawnPerScore = 4;
        void Awake()
        {
            Time.timeScale = 0;
            scoreTag = settings.scoreTag;
            obstacleTag = settings.obstacleTag;
            wallTag = settings.wallTag;

            gridSystem = new GridSystem(transform.position, xOffset, yOffset);

            spawnedScoreSquare = Instantiate(scoreSquarePrefab);
            spawnedObstacleSquare = Instantiate(obstacleSquarePrefab);
            for (int i = 0; i < 10; i++)
            {
                spawnedWallSquare.Add(Instantiate(wallSquarePrefab));
                wallsCoords.Add(spawnedWallSquare[i], (0, 0));
            }

        }
        private void Start()
        {
            SetScoreSquare();
            SetObstacleSquare();

        }
        private void SetRandomWalls(int amountOfWalls)
        {
            DeleteWalls();
            for (int i = 0; i < amountOfWalls; i++)
            {

                (int, int) randomCoords = gridSystem.GetRandomFreeCell();
                spawnedWallSquare[i].transform.position = gridSystem.positionsGrid[randomCoords.Item1, randomCoords.Item2];

                gridSystem.SetGridAsBlocked(randomCoords.Item1, randomCoords.Item2, wallTag);

                spawnedWallSquare[i].SetActive(true);
                wallsCoords[spawnedWallSquare[i]] = randomCoords;
            }
        }
        private void DeleteWalls()
        {

            for (int i = 0; i < spawnedWallSquare.Count; i++)
            {
                if (spawnedWallSquare[i].activeSelf)
                {
                    gridSystem.SetGridAsFree(wallsCoords[spawnedWallSquare[i]].Item1, wallsCoords[spawnedWallSquare[i]].Item2);
                    spawnedWallSquare[i].SetActive(false);
                }

            }
        }
        public void SetScoreSquare()
        {
            (int, int) randomCoords = gridSystem.GetRandomFreeCell();
            spawnedScoreSquare.transform.position = gridSystem.positionsGrid[randomCoords.Item1, randomCoords.Item2];
            gridSystem.SetGridAsBlocked(randomCoords.Item1, randomCoords.Item2 ,scoreTag);
            SetRandomWalls(wallsToSpawnPerScore);
        }
        public void SetObstacleSquare()
        {
            (int, int) randomCoords = gridSystem.GetRandomFreeCell();
            spawnedObstacleSquare.transform.position = gridSystem.positionsGrid[randomCoords.Item1, randomCoords.Item2];

            gridSystem.SetGridAsBlocked(randomCoords.Item1, randomCoords.Item2, obstacleTag);
        }
       
    }
}
