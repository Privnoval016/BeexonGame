using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMOD.Studio;

public class TitleScreenManagerScript : MonoBehaviour
{
    public TextMeshProUGUI blinkingStartText;
    public Image title;
    private Vector2 imgStart;
    public GameObject infoPanel;
    public Image credits;

    public FMODUnity.EventReference startSound;

    private bool openInfo = false;
    private bool openCredits = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blinkClock());
        StartCoroutine(PlayEntry());
        imgStart = title.GetComponent<RectTransform>().anchoredPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            FMODUnity.RuntimeManager.PlayOneShot(startSound, transform.position);
            SceneManager.LoadScene("MasterScene");
        }

        float newY = 15 * Mathf.Sin(Time.time * 0.7f) + imgStart.y;

        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(imgStart.x, newY);

        if (Input.GetKeyDown(KeyCode.I))
        {
            openInfo = !openInfo;
            FMODUnity.RuntimeManager.PlayOneShot(startSound, transform.position);
        }

        if (Input.GetKeyDown(KeyCode.C) && openInfo)
        {
            openCredits = !openCredits;
        }  
        
        Color color = credits.color;

        float targetAlpha = openCredits ? 1 : 0;

        if (Mathf.Abs(color.a - targetAlpha) > 0.0001f)
        {
            color.a = Mathf.Lerp(color.a, targetAlpha, 5f * Time.deltaTime);
            credits.color = color;
        }

        if (openInfo)
        {
            if (infoPanel.transform.localScale.x < 1)
            {
                infoPanel.transform.localScale += new Vector3(1f, 1f, 0) * Time.deltaTime * 5f;
            }
        }
        else
        {
            if (infoPanel.transform.localScale.x > 0)
            {
                infoPanel.transform.localScale -= new Vector3(1f, 1f, 0) * Time.deltaTime * 5f;
            }
        }
    }

    IEnumerator blinkClock() {
        blinkingStartText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        blinkingStartText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(blinkClock());
    }

    IEnumerator PlayEntry()
    {
        title.color = new Color(title.color.r, title.color.g, title.color.b, 0);
        yield return new WaitForSeconds(1f);
        float targetAlpha = 1;
        Color curColor = title.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, 1f * Time.deltaTime);
            title.color = curColor;
            yield return null;
        }
    }
}
