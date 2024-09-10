using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopupController : MonoBehaviour
{
    RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();   
    }

    // Update is called once per frame
    void Update()
    {
        //rt.anchoredPosition += new Vector2(0.614f, -0.789f) * Time.deltaTime * 100f;
        if (GameManagerScript.S.isPaused) return;

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - 0.789f * Time.deltaTime * 250f, 
            rt.anchoredPosition.y - 0.614f * Time.deltaTime * 250f);
    }
}
