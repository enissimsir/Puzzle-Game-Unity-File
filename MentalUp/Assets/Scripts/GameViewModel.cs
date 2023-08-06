using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class GameViewModel : MonoBehaviour
{
    [System.NonSerialized]
    public int[] gridSize;
    [System.NonSerialized]
    public int[] time;
    public int[][] pieceIDs;
    public int[][] pieceAngles;

    public event Action OnJsonLoaded;
    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Game159Params"); // JSON dosyasını yükle

        if (jsonFile != null)
        {
            Debug.Log("dosya mevcut");
            JObject jsonObject = JObject.Parse(jsonFile.text);

            // gridSize parametresini al
            JArray gridSizeArray = (JArray)jsonObject["gridSize"];
            gridSize = gridSizeArray.Select(x => (int)x).ToArray();

            // time parametresini al
            JArray timeArray = (JArray)jsonObject["time"];
            time = timeArray.Select(x => (int)x).ToArray();

            // pieceIDs parametresini al
            JArray pieceIDsArray = (JArray)jsonObject["pieceIDs"];
            pieceIDs = pieceIDsArray.Select(x => x.Select(y => (int)y).ToArray()).ToArray();

            // pieceAngles parametresini al
            JArray pieceAnglesArray = (JArray)jsonObject["pieceAngles"];
            pieceAngles = pieceAnglesArray.Select(x => x.Select(y => (int)y).ToArray()).ToArray();

            // Parametreleri kullanarak oyunu başlat veya diğer yerlerde kullan
            // Örnek olarak, gridSize parametresini kullanarak oyun tahtasını oluşturabilirsiniz.

            OnJsonLoaded?.Invoke();

        }
        else
        {
            Debug.LogError("Game159Params dosyası bulunamadı!");
        }
    }
}