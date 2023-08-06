using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TilePiece : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameManagerr gameManager;

    public GridLayoutGroup gridLayoutGroup;

    [System.NonSerialized] public RectTransform parentRect;

    [System.NonSerialized] public bool pieceState;

    [System.NonSerialized] public int gridIndexI, gridIndexJ;

    private bool isGameStarted;

    private void Start()
    {
        isGameStarted = false;
        Invoke("StartGame", 4f);
        gameManager = FindObjectOfType<GameManagerr>();

        // gameManager değişkeninin null olup olmadığını kontrol et
        if (gameManager == null)
        {
            Debug.LogWarning("GameManagerr nesnesi bulunamadı!");
        }
        pieceState = false;
    }

    private void StartGame()
    {
        isGameStarted = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("isGameStarted = " + isGameStarted);
        if (isGameStarted)
        {
            GameObject parentPrefab = transform.parent.gameObject;
            parentPrefab.transform.Rotate(0f, 0f, -90f);
            if (pieceState)
            {
                Transform pieceTransform = transform.parent;
                foreach (Transform tileTransform in pieceTransform)
                {
                    TilePiece tilePiece = tileTransform.GetComponent<TilePiece>();
                    tilePiece.pieceState = false;

                    gameManager.occupiedCells[tilePiece.gridIndexI, tilePiece.gridIndexJ] = false;
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isGameStarted)
        {
            transform.parent.position = Input.mousePosition; //oynat
            float newSize = (float)gameManager.edgeLength / 100;

            transform.parent.localScale = new Vector2(newSize, newSize); //boyutu artır
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isGameStarted)
        {
            Transform[] targetCells = new Transform[transform.parent.childCount];
            int[,] occupyIndexes = new int[2, transform.parent.childCount];
            if (isPositionAvaible(targetCells, occupyIndexes))
            {
                //    Debug.Log("pozisyon uygun");

                Transform pieceTransform = transform.parent;
                int i = 0;
                foreach (Transform tileTransform in pieceTransform)
                {
                    tileTransform.position = targetCells[i].position;
                    gameManager.setOccupied(occupyIndexes[0, i], occupyIndexes[1, i]);

                    TilePiece tilePiece = tileTransform.GetComponent<TilePiece>();
                    tilePiece.pieceState = true;
                    tilePiece.gridIndexI = occupyIndexes[0, i];
                    tilePiece.gridIndexJ = occupyIndexes[1, i];

                    i++;
                }
            }
            else
            {
                transform.parent.localScale = new Vector2(1, 1);
                Debug.Log("pozisyonu = " + transform.parent.position);

                parentRect = GetComponent<RectTransform>();
                parentRect.parent.localPosition = new Vector2(UnityEngine.Random.Range(-380, 381), UnityEngine.Random.Range(-420, -800));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isGameStarted && !pieceState)
        {
            GameObject parentPrefab = transform.parent.gameObject;
            parentPrefab.transform.Rotate(0f, 0f, 90f);
        }
    }

    private bool isPositionAvaible(Transform[] targetCells,int[,] occupyIndexes)
    {
        Transform pieceTransform = transform.parent;

        // Eğer parent obje null değilse, yani bir parent objeye sahipsek
        if (pieceTransform != null)
        {
            int tileNumber = 0;
            int closestCellIndexI = 0, closestCellIndexJ = 0;
            // Parent objenin bütün child objelerine ulaşmak için foreach döngüsü kullanıyoruz.
            foreach (Transform tileTransform in pieceTransform)
            {
                float closestCellLength = 9999999;
                int gridSize = gameManager.gridSize;
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        float distance = CalculateDistance(tileTransform, gameManager.tileObjects[i, j]);
                        if (distance < closestCellLength)
                        {
                            closestCellLength = distance;
                            targetCells[tileNumber] = gameManager.tileObjects[i,j];
                            occupyIndexes[0, tileNumber] = i;
                            occupyIndexes[1, tileNumber] = j;
                            closestCellIndexI = i; closestCellIndexJ = j;
                        }
                    }
                }
             //   Debug.Log("bu tile'a en yakin cell, i = " + closestCellIndexI + " j = " + closestCellIndexJ + " uzaklik = " + closestCellLength);
                if (gameManager.isOccupied(closestCellIndexI, closestCellIndexJ) || closestCellLength>16075)
                {
                    return false;
                }// işgal edilmediyse yerleştirme işlemleri
                tileNumber++;
            }
        }
        return true;
    }

    private float CalculateDistance(Transform object1, Transform object2)
    {
        // İki objenin pozisyonlarını ekran koordinatlarında al
        Vector3 screenPos1 = Camera.main.WorldToScreenPoint(object1.position);
        Vector3 screenPos2 = Camera.main.WorldToScreenPoint(object2.position);

        // İki ekran koordinatı arasındaki farkı alarak uzaklığı hesapla
        float distance = Vector3.Distance(screenPos1, screenPos2);

        return distance;
    }

    


}
