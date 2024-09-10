using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; private set; }


    [Header("Background Sounds")]
    [SerializeField] private FMODUnity.EventReference entrySound;
    [SerializeField] private FMODUnity.EventReference[] backgroundMusics;
    [SerializeField] private FMODUnity.EventReference transition;
    [SerializeField] private FMODUnity.EventReference gameOverSound;
    [SerializeField] private FMODUnity.EventReference winSound;

    public EventInstance backgroundMusic;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(PlayEntry());

    }

    IEnumerator PlayEntry()
    {
        FMODUnity.RuntimeManager.PlayOneShot(entrySound, transform.position);
        yield return new WaitForSeconds(1f);
        backgroundMusic = FMODUnity.RuntimeManager.CreateInstance(backgroundMusics[0]);
        backgroundMusic.start();
    }

    // Update is called once per frame
    void Update()
    {
        backgroundMusic.setPaused(GameManagerScript.S.isPaused);
    }

    public void SetBackgroundMusic(int level)
    {
        if (level == 0) StartCoroutine(PlayTransition(0, 2.5f, 0));
        if (level == 1) return;
        if (level == 2) StartCoroutine(PlayTransition(0, 2.5f, 1));
        if (level == 3) StartCoroutine(PlayTransition(5, 2.5f, 2));
    }

    IEnumerator PlayTransition(float pause, float delay, int level)
    {
        yield return new WaitForSeconds(pause);
        backgroundMusic.stop(STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShot(transition, transform.position);
        yield return new WaitForSeconds(delay);
        backgroundMusic = FMODUnity.RuntimeManager.CreateInstance(backgroundMusics[level]);
        backgroundMusic.start();
    }

    public void PlayGameOver()
    {
        backgroundMusic.stop(STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShot(gameOverSound, transform.position);
    }

    public void PlayWin()
    {
        FMODUnity.RuntimeManager.PlayOneShot(winSound, transform.position);
    }
}
