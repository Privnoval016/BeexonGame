using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class StraightWaspController : MonoBehaviour
{
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public float speed = 5f;

    private float elapsedTime;

    private int moveDirectionX = 1;
    private int moveDirectionY = 1;

    private Vector3 target;

    public GameObject projectilePrefab;
    public int projectileSpeed = 5;
    public float projectileOffset = 0.5f;
    public float shootDelay = 1f;

    public float minX, minY, maxX, maxY;

    public FMODUnity.EventReference shootSound;
    public FMODUnity.EventReference deathSound;
    public GameObject deathParticle;

    public float backAndForthTime = 5f;
    private float timer = 0f;

    public int points = 500;

    private bool isShooting = false;

    public enum WaspState
    {
        Straight,
        BackAndForth,
        LeaveScreen
    }

    public WaspState waspState;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.instance.numStraightWasps++;
        LevelManager.instance.AddToFlyingEnemies(gameObject);
        elapsedTime = 0f;
        transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), spawnPosition.z);
        endPosition = new Vector3(transform.position.x, transform.position.y, endPosition.z);

        StartCoroutine(StartShootCycle());

        moveDirectionX = Random.Range(0, 2) - 1;
        moveDirectionY = Random.Range(0, 2) - 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > backAndForthTime)
        {
            waspState = WaspState.LeaveScreen;
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.x >= maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
        if (transform.position.y >= maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
        else if (transform.position.y <= minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }

        switch (waspState)
        {
            case WaspState.Straight:
                isShooting = false;
                elapsedTime += Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(spawnPosition, endPosition, speed * elapsedTime * 0.5f);

                if (transform.position.z <= endPosition.z + 0.1f)
                {
                    waspState = WaspState.BackAndForth;
                }
                break;
            case WaspState.BackAndForth:
                
                isShooting = true;

                transform.Translate(Vector3.right * moveDirectionX * speed * Time.fixedDeltaTime);
                int changeXDir = Random.Range(0, 100);
                if (changeXDir == 1)
                {
                    moveDirectionX = 1;
                }
                else if (changeXDir > 90)
                {
                    moveDirectionX = 0;
                }
                else if (changeXDir == 3)
                {
                    moveDirectionX = -1;
                }

                transform.Translate(Vector3.up * moveDirectionY * speed * Time.fixedDeltaTime);
                int changeYDir = Random.Range(0, 100);
                if (changeYDir == 1)
                {
                    moveDirectionY = 1;
                }
                else if (changeYDir > 90)
                {
                    moveDirectionY = 0;
                }
                else if (changeYDir == 3)
                {
                    moveDirectionY = -1;
                }

                break;
            case WaspState.LeaveScreen:
                isShooting = false;
                transform.Translate(Vector3.back * speed * Time.fixedDeltaTime);
                break;
        }
    }

    IEnumerator StartShootCycle()
    {
        yield return new WaitForSeconds(shootDelay + Random.Range(0, shootDelay * 0.5f));
        if (isShooting && !GameManagerScript.S.isPaused) Shoot();
        StartCoroutine(StartShootCycle());
    }

    private void Shoot()
    {

        GameObject projectile;

        projectile = Instantiate(
            projectilePrefab,
            new Vector3(transform.position.x, transform.position.y, transform.position.z - projectileOffset),
            projectilePrefab.transform.rotation);

        projectile.GetComponent<ProjectileScript>().projectileDirection = Direction.Backward;

        FMODUnity.RuntimeManager.PlayOneShot(shootSound, transform.position);

        projectile.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile.GetComponent<ProjectileScript>().parent = gameObject;
        projectile.transform.localScale *= 0.7f;
        projectile.tag = "EnemyProjectile";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (!GameManagerScript.S.isPaused)
            {
                FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);
                GameObject explosion = Instantiate(deathParticle, transform.position, Quaternion.identity);
                explosion.transform.localScale *= 0.7f;
            }

            LevelManager.instance.numStraightWasps--;
            GameManagerScript.S.score += points;
            GameManagerScript.S.killedEnemies++;
            LevelManager.instance.RemoveFromFlyingEnemies(gameObject);
            UIManagerScript.S.DisplayScorePopup(transform.position, points);

            Destroy(gameObject);
        }
    }
}
