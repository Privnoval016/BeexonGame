using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using FMOD.Studio;
using UnityEngine;

public class PlayerBeeScript : MonoBehaviour
{

    [Header("Keybinds")]
    [SerializeField] public KeyCode upKey;
    [SerializeField] public KeyCode downKey;
    [SerializeField] public KeyCode leftKey;
    [SerializeField] public KeyCode rightKey;
    [SerializeField] public KeyCode shootKey;

    [Header("Player Attributes")]
    [SerializeField] public float MAX_BEE_TILT; // tilt angle limit when the player moves in a direction
    [SerializeField] public int speed;
    [SerializeField] public GameObject model;

    [Header("Projectile Attributes")]
    [SerializeField] public float projSpawnDist; // distance from player to spawn projectile
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Material projectileMaterial;

    [Header("Movement Caps")]
    [SerializeField] public float maxX;
    [SerializeField] public float maxY;
    [SerializeField] public float minX;
    [SerializeField] public float minY;

    [Header("Effects")]
    [SerializeField] public FMODUnity.EventReference shootSound;
    [SerializeField] public FMODUnity.EventReference hitSound;
    [SerializeField] public FMODUnity.EventReference buzzSound;
    [SerializeField] GameObject deathParticle;

    private EventInstance buzzingNoise;

    private Vector3 beeRotation;

    // Start is called before the first frame update
    void Start()
    {
        // player default. Please do not change
        beeRotation = new Vector3(90, 0, 0);

        buzzingNoise = FMODUnity.RuntimeManager.CreateInstance(buzzSound);
        buzzingNoise.start();
    }

    // Update is called once per frame
    void Update()
    {
        bool moved = false;

        if (GameManagerScript.S.isPaused) return;

        if (GameManagerScript.S.okToRespawn) model.SetActive(true);

        // player movement, as well as tilting
        Vector3 currentPosition = transform.position;

        if ((Input.GetKey(upKey) || Input.GetKey(KeyCode.W)) && transform.localPosition.y < maxY)
        {
            currentPosition.y += Time.deltaTime * speed;

            if (90 - beeRotation.x < MAX_BEE_TILT) { beeRotation.x--; }
            moved = true;
        }
        if ((Input.GetKey(downKey) || Input.GetKey(KeyCode.S)) && transform.localPosition.y > minY)
        {
            currentPosition.y -= Time.deltaTime * speed;
            if (beeRotation.x - 90 < MAX_BEE_TILT) { beeRotation.x++; }
            moved = true;
        }
        if ((Input.GetKey(leftKey) || Input.GetKey(KeyCode.A)) && transform.localPosition.x > minX)
        {
            currentPosition.x -= Time.deltaTime * speed;
            if (beeRotation.y * -1 < MAX_BEE_TILT) { beeRotation.y--; }
            moved = true;
        }
        if ((Input.GetKey(rightKey) || Input.GetKey(KeyCode.D)) && transform.localPosition.x < maxX)
        {
            currentPosition.x += Time.deltaTime * speed;
            if (beeRotation.y < MAX_BEE_TILT) { beeRotation.y++; }
            moved = true;
        }

        // reset the player's tilt if player is not moving
        if (!moved)
        {
            if (beeRotation.x < 90)
            {
                beeRotation.x++;
            }
            else if (beeRotation.x > 90)
            {
                beeRotation.x--;
            }
            if (beeRotation.y < 0)
            {
                beeRotation.y++;
            }
            else if (beeRotation.y > 0)
            {
                beeRotation.y--;
            }
        }

        transform.position = currentPosition;
        transform.eulerAngles = beeRotation;

        // shooting, spawn projectile
        Vector3 projectileSpawnLocation = transform.position;
        projectileSpawnLocation.z += projSpawnDist; // spawn 5 units infront of current player position
        if (Input.GetKeyDown(shootKey) || Input.GetKeyDown(KeyCode.Space))
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnLocation, projectilePrefab.transform.rotation);
            FMODUnity.RuntimeManager.PlayOneShot(shootSound, transform.position);
            projectile.GetComponent<MeshRenderer>().material = projectileMaterial;
            projectile.tag = "Projectile";
        }

        if (moved)
        {
            buzzingNoise.setPaused(false);
        }
        else
        {
            buzzingNoise.setPaused(true);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (!other.CompareTag("Projectile") && !other.CompareTag("Row"))
        {
            if (!GameManagerScript.S.isPaused)
            {
                FMODUnity.RuntimeManager.PlayOneShot(hitSound, transform.position);
                Instantiate(deathParticle, transform.position, Quaternion.identity);
                
                GameManagerScript.S.GetHit();

                model.SetActive(false);

            }   
        }
    }

    public void OutOfFuel()
    {
        FMODUnity.RuntimeManager.PlayOneShot(hitSound, transform.position);
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        model.SetActive(false);
    }

}
