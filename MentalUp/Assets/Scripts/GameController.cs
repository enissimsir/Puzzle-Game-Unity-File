using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameManagerr gameManager;
    [SerializeField] Tween tween;

    private float timeLimit;
    private float time;
    private bool isGameLoaded;
    private bool isGameOn;

    private void Awake()
    {
        isGameOn = true;
        Debug.Log("Controller burada",gameObject);
        isGameLoaded = false;
    }

 

    private void Update()
    {
        if (time > 0f && isGameLoaded && isGameOn)
        {
            time -= Time.deltaTime; // Her çerçevede zamanı güncelle
        }
        else if(isGameOn)
        {
            EndGame();
        }

        if (tween.replay)
        {
            gameManager.LevelDesign();
        }
    }

    private void EndGame()
    {
        tween.GameLostPopUps(gameManager.CalculateScore());
        isGameOn = false;
        Debug.Log("sure doldu :(");
    }

    public void LoadHudAndStartGame()
    {
        if (gameManager != null)
        {
            timeLimit = gameManager.GetTime();
            time = timeLimit;
            isGameLoaded = true;
            Debug.Log("time = " + timeLimit);
            tween.StartCounter();
        }
        else
        {
            Debug.LogError("GameController, GameManagerr'i bulamadi!");
        }
    }
}
