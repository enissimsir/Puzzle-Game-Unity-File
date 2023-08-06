using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tween : MonoBehaviour
{
    [SerializeField] GameObject losingImagePrefab; // Prefab'ı Unity düzenleyicisinde buraya sürükleyin
    [SerializeField] Transform Canvas;
    [SerializeField] GameObject backPanelPrefab;
    private GameObject losingImage;
    private GameObject backPanel;

    [SerializeField] AudioSource countdownSound;
    [SerializeField] AudioClip countdownClip;
    [SerializeField] AudioSource gameoverSound;
    [SerializeField] AudioClip gameoverClip;

    public Text countdownText;
    public Button button;
    public int startingCountdownValue = 3;
    private int currentCountdownValue;
    public bool replay;


    private void Start()
    {
        LoadPanel();
    }

    private void LoadPanel()
    {
        losingImage = Instantiate(losingImagePrefab, Canvas, false);
        if (losingImage == null)
        {
            Debug.LogWarning("losingImage null yine");
        }
        losingImage.SetActive(false);
        backPanel = Instantiate(backPanelPrefab, Canvas, false);
        if (backPanel == null)
        {
            Debug.LogWarning("losingImage null yine");
        }
        button = backPanel.GetComponentInChildren<Button>();
        button.onClick.AddListener(OnClickButton);
        backPanel.SetActive(false);
    }

    private void OnClickButton()
    {
        replay = true;
        Destroy(losingImage);
        Destroy(backPanel);
    }

    public void StartCounter()
    {
        replay = false;
        countdownText.gameObject.SetActive(true);
        currentCountdownValue = startingCountdownValue;
        UpdateCountdownText();

        countdownSound.clip = countdownClip;
        countdownSound.Play();
        LeanTween.value(3, 0, 3f).setOnUpdate(UpdateCountdown).setOnComplete(OnCountdownComplete);
    }
    private void UpdateCountdown(float value)
    {
        // Geri sayım değerini zamanla azaltıyoruz ve ekrana yansıtıyoruz
        currentCountdownValue = Mathf.RoundToInt(value);
        UpdateCountdownText();
    }

    private void UpdateCountdownText()
    {
        countdownText.text = currentCountdownValue.ToString();
    }

    private IEnumerator OnCountdownCompleteCoroutine()
    {
        // Burada geri sayım tamamlandığında yapılacak işlemleri ekleyebilirsiniz.
        countdownText.text = "BAŞLA!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }

    private void OnCountdownComplete()
    {
        StartCoroutine(OnCountdownCompleteCoroutine());
    }


    public void GameLostPopUps(int score)
    {
        gameoverSound.clip = gameoverClip;
        gameoverSound.Play();

        if (losingImage != null)
        {
            YouLostPopUp();
            Text scoreText = GetChildTextByName("Score");
            if (scoreText != null)
            {
                scoreText.text = score + "!"; // Yeni metni atayın
            }
            else
            {
                Debug.LogWarning("Score isimli Text nesnesi bulunamadı!");
            }
        }
        else
        {
            Debug.LogWarning("losingImage nesnesi bulunamadı!");
        }
    }

    private void YouLostPopUp()
    {
        losingImage.SetActive(true);
        losingImage.transform.SetAsLastSibling();
        LeanTween.scale(losingImage, new Vector3(1.5f, 1.5f, 1.5f), 2f).setDelay(.5f).setEase(LeanTweenType.easeOutElastic).setOnComplete(BackPanelPopUp);
        LeanTween.moveLocal(losingImage, new Vector3(0f, 500f, -1f), 0.7f).setDelay(2f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(losingImage, new Vector3(1f, 1f, 1f), 2f).setDelay(1.7f).setEase(LeanTweenType.easeInOutCubic);

    }

    private void BackPanelPopUp()
    {
        backPanel.SetActive(true);
        backPanel.transform.SetAsLastSibling();
        losingImage.transform.SetAsLastSibling();
        LeanTween.moveLocal(backPanel, new Vector3(0f, 0f, 0f), 0.7f).setDelay(.5f).setEase(LeanTweenType.easeOutCirc);
    }

    private Text GetChildTextByName(string textObjectName)
    {
        Text[] textComponents = backPanel.GetComponentsInChildren<Text>(); // Tüm Text bileşenlerini al
        foreach (Text textComponent in textComponents)
        {
            if (textComponent.name == textObjectName)
            {
                return textComponent; // İstenilen isimdeki Text nesnesini döndür
            }
        }
        return null; // İstenilen isimde Text nesnesi bulunamadı
    }

}
