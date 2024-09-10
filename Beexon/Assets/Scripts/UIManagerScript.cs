using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// SINGLETON
public class UIManagerScript : MonoBehaviour
{
    public static UIManagerScript S;
    public Slider fuelSlider;
    public Slider altitudeSlider;
    public Image[] livesIcons;
    public TextMeshProUGUI scoreText;
    public Image loadScreen;
    public TextMeshProUGUI killedEnemiesText;

    public Image[] restartScreen;
    public TextMeshProUGUI progressText;

    private int cloudMove = 0;
    public RectTransform cloudLeft;
    public RectTransform cloudRight;
    public float cloudLeftStart = -724f;
    public float cloudLeftEnd = -124f;
    public float cloudMoveSpeed = 50f;

    public Camera playerCamera;
    public GameObject scorePopupText;
    public float shrinkTime; // time it takes for the text to fade out
    public Color colorTwo; // the color the text blinks to
    public float blinkInterval; // the cooldown time between each blink

    private void Awake()
    {
        if (S) Destroy(gameObject);
        else { S = this; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFuelSlider();
        UpdateAltitudeSlider();
        UpdateScore();
        UpdateKilledEnemies();
        UpdateClouds();
    }

    public void UpdateFuelSlider()
    {
        fuelSlider.value = GameManagerScript.S.fuel;
    }

    public void UpdateKilledEnemies()
    {
        killedEnemiesText.text = GameManagerScript.S.killedEnemies + "";
    }

    public void UpdateAltitudeSlider()
    {
        altitudeSlider.value = GameManagerScript.S.altutude;
    }

    public void UpdateLives()
    {
        int livesLeft = GameManagerScript.S.lives;
        if (livesLeft < 0) return;
        livesIcons[livesLeft].gameObject.SetActive(false);
    }

    public void UpdateScore()
    {
        string text = GameManagerScript.S.score + "";

        while (text.Length < 6) text = "0" + text;


        scoreText.text = text;
    }

    public void UpdateClouds()
    {

        cloudLeft.anchoredPosition = new Vector2(cloudLeft.anchoredPosition.x + cloudMoveSpeed * cloudMove * Time.deltaTime,
            cloudLeft.anchoredPosition.y);
        cloudRight.anchoredPosition = new Vector2(cloudRight.anchoredPosition.x - cloudMoveSpeed * cloudMove * Time.deltaTime,
            cloudRight.anchoredPosition.y);

        if (cloudMove == 1)
        {
            if (cloudLeft.anchoredPosition.x >= cloudLeftEnd)
            {
                cloudMove = 0;
            }
        }
        else if (cloudMove == -1)
        {
            if (cloudLeft.anchoredPosition.x <= cloudLeftStart)
            {
                cloudMove = 0;
            }
        }

    }

    public void FadeToBlack(bool on)
    {
        StartCoroutine(Fade(loadScreen, on, 7f));
    }

    public void FadeRestart(bool on)
    {
        StartCoroutine(Fade(restartScreen[0], on, 10f));
        StartCoroutine(Fade(restartScreen[1], on, 10f));
        progressText.text = "FINAL SCORE: " + GameManagerScript.S.score;
        StartCoroutine(Fade(progressText, on, 5f));
    }

    public void PlayReset(int state)
    {
        cloudMove = state;
    }

    IEnumerator Fade(Image image, bool isFadingIn, float fadeRate)
    {
        float targetAlpha = isFadingIn ? 1 : 0;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
    }

    IEnumerator Fade(TextMeshProUGUI image, bool isFadingIn, float fadeRate)
    {
        float targetAlpha = isFadingIn ? 1 : 0;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
    }

    public void DisplayScorePopup(Vector3 enemyDeathPosition, int scoreReward)
    {
        var text = Instantiate(scorePopupText);
        text.transform.parent = GameObject.Find("Canvas").transform;
        text.GetComponent<TextMeshProUGUI>().text = scoreReward.ToString();
        Vector2 screenLocation = playerCamera.WorldToScreenPoint(enemyDeathPosition);
        screenLocation.x -= 768 / 2;
        screenLocation.y -= 1024 / 2;
        text.GetComponent<TextMeshProUGUI>().rectTransform.anchoredPosition = screenLocation;

        StartCoroutine(ShrinkBlink(text));
    }

    private IEnumerator ShrinkBlink(GameObject text)
    {
        float curShrinkTime = shrinkTime;
        yield return new WaitForSeconds(0.75f * curShrinkTime);
        curShrinkTime *= 0.25f;
        float elapsedTime = 0;
        float timeSinceLastBlink = 0;
        bool isOnFirstColor = true;
        Color colorOne = text.GetComponent<TextMeshProUGUI>().color;
        while (elapsedTime < curShrinkTime)
        {
            elapsedTime += Time.deltaTime;
            timeSinceLastBlink += Time.deltaTime;
            // update font size
            float fontSize = Mathf.Lerp(46f, 0f, elapsedTime / curShrinkTime);
            text.GetComponent<TextMeshProUGUI>().fontSize = fontSize;

            // blink 
            if (timeSinceLastBlink >= blinkInterval)
            {
                if (isOnFirstColor)
                {
                    isOnFirstColor = false;
                    text.GetComponent<TextMeshProUGUI>().color = colorTwo;
                }
                else
                {
                    isOnFirstColor = true;
                    text.GetComponent<TextMeshProUGUI>().color = colorOne;
                }
                timeSinceLastBlink = 0;
            }
            yield return 0;
        }
        Destroy(text);
    }
}
