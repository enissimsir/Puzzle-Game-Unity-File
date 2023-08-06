using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerr : MonoBehaviour
{
    [SerializeField] GameController gameController;


    [System.NonSerialized] public int level;

    [SerializeField] GameViewModel gameViewModel;

    private GridLayoutGroup gridLayout;

    public GameObject gridObject;

    public GameObject tilePrefab;

    public GameObject[] pieces;

    public RectTransform canvasRect;

    private GameObject[] prefabInstances;

    [System.NonSerialized] public int edgeLength;
    [System.NonSerialized] public int gridSize;

    [System.NonSerialized] public Transform[,] tileObjects;

    [System.NonSerialized] public bool[,] occupiedCells;


    // Start is called before the first frame update
    void Start()
    {
        prefabInstances = new GameObject[10];
        tileObjects = new Transform[10, 10];
        occupiedCells = new bool[10, 10];
        level = 0;
        gameViewModel.OnJsonLoaded += LevelDesign; // OnJsonLoaded olayını dinle
        
        if (gameViewModel.gridSize != null) // gridSize değeri null değilse LevelDesign'i çağır
        {
            LevelDesign();
        }
       
    }

    public void setOccupied(int i,int j)
    {
        occupiedCells[i, j] = true;
        if (areAllCellsOccupied())
        {
            loadNextLevel();
        }
    }

    private void loadNextLevel()
    {
        level++;
        LevelDesign();
    }

    private bool areAllCellsOccupied()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (!occupiedCells[i, j]) return false;
            }
        }
        return true;
    }

    public bool isOccupied(int i, int j)
    {
        return occupiedCells[i,j];
    }

    public int CalculateScore()
    {
        float score = 0;
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                if (isOccupied(i,j))
                {
                    score++;
                }
            }
        }
        score = (score / (gridSize * gridSize)) * 100;
        return (int)score;
    }

    public Transform getTileObject(int i, int j)
    {
        return tileObjects[i, j];
    }

    public void LevelDesign()
    {
        gameController.LoadHudAndStartGame();

        DestroyOldPieces();
        gridSize = gameViewModel.gridSize[level];

        gridLayout = gridObject.GetComponent<GridLayoutGroup>();
        edgeLength = 1000 / gridSize;
        gridLayout.cellSize = new Vector2(edgeLength, edgeLength);
        Debug.Log(gridSize);

        // Önce gridi temizle
        foreach (Transform child in gridObject.transform)
        {
            Destroy(child.gameObject);
        }

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // Tile prefabını instantiate et ve grid içinde konumlandır
                GameObject tileObject = Instantiate(tilePrefab, gridObject.transform);
                // Tile'ın pozisyonunu ve boyutunu ayarla
                tileObject.transform.localPosition = Vector3.zero;
                tileObject.transform.localScale = Vector3.one;

                tileObjects[row,col] = tileObject.transform;
                occupiedCells[row,col] = false;
          //      Debug.Log("row = " + row + " col = " + col + " transform = " + tileObjects[row, col]);
            }
        }
        
        //Parcalari olustur
        int[] pieceIDs = gameViewModel.pieceIDs[level];

        for (int i = 0; i < pieceIDs.Length; i++)
        {
            prefabInstances[i] = Instantiate(pieces[pieceIDs[i] - 1], canvasRect);
            Vector2 spawnPosition = new Vector2(UnityEngine.Random.Range(-380, 381), UnityEngine.Random.Range(-420, -800));
            

            // Prefab'ı canvas üzerinde belirtilen konuma yerleştir
            RectTransform prefabRect = prefabInstances[i].GetComponent<RectTransform>();
            prefabRect.anchoredPosition = spawnPosition;
            if (gameViewModel.pieceAngles[level][i] == 90)
            {
                prefabRect.Rotate(new Vector3(0f,0f,90));
            }
        }
    }

    private void DestroyOldPieces()
    {
        for(int i = 0; i < 10; i++)
        {
            Destroy(prefabInstances[i]);
        }
    }

    public int GetTime()
    {
        return gameViewModel.time[level];
    }

}