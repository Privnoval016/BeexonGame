using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class SeekingWaspController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject player;
    private Vector3 delayedPlayerLocation;

    public Vector3 spawnPosition;

    public int maxHits = 2;
    private int hits = 0;

    public float delay = 0.25f;
    private float delayTimer = 0f;

    public int points = 1000;

    private bool isTracking = true;

    public FMODUnity.EventReference deathSound;
    public FMODUnity.EventReference startSound;
    public GameObject deathParticle;

    // Start is called before the first frame update
    void Start()
    {
        speed = 7f;
        Debug.Log(transform.localScale);
        LevelManager.instance.AddToFlyingEnemies(gameObject);
        player = GameManagerScript.S.playerBee;
        transform.position = spawnPosition;

        delayedPlayerLocation = player.transform.position;

        FMODUnity.RuntimeManager.PlayOneShot(startSound, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking)
        {
            if (delayTimer < delay)
            {
                delayTimer += Time.deltaTime;
            }
            else
            {
                delayTimer = 0f;
                delayedPlayerLocation = player.transform.position;
            }

            Vector3 targetPos = new Vector3(delayedPlayerLocation.x, delayedPlayerLocation.y, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);

        if (transform.position.z < player.transform.position.z)
        {
            isTracking = false;
        }

        if (transform.position.z < -5f)
        {
            LevelManager.instance.RemoveFromFlyingEnemies(this.gameObject);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            hits++;
            if (hits >= maxHits)
            {
                GameManagerScript.S.score += points;
                if (!GameManagerScript.S.isPaused)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);
                    GameObject explosion = Instantiate(deathParticle, transform.position, Quaternion.identity);
                    explosion.transform.localScale *= 0.7f;
                }
                LevelManager.instance.RemoveFromFlyingEnemies(this.gameObject);

                UIManagerScript.S.DisplayScorePopup(transform.position, points);
                Destroy(gameObject);
            }
        }
    }
}
