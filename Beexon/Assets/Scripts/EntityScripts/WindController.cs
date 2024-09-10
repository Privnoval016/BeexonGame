using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD.Studio;

public class WindController : MonoBehaviour
{
    public FMODUnity.EventReference gustSound;
    private EventInstance gustNoise;

    // Start is called before the first frame update
    void Start()
    {
        gustNoise = FMODUnity.RuntimeManager.CreateInstance(gustSound);
        gustNoise.start();
    }

    // Update is called once per frame
    void Update()
    {
        gustNoise.setPaused(GameManagerScript.S.isPaused);

        if (transform.position.z < GameManagerScript.S.playerBee.transform.position.z - 2f)
        {
            gustNoise.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
