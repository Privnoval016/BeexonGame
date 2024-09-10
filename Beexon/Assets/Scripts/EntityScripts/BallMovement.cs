using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class BallMovement : MonoBehaviour
{
    public GameObject target;
    private Vector3 delayedTargetLocation;
    public float moveSpeed = 2f;

    

    public GameObject projectilePrefab;
    public int projectileSpeed = 5;
    public float projectileOffset = 0.5f;
    public float shootDelay = 1f;
    public bool isShooting = true;

    public float delay = 0.25f;
    private float delayTimer = 0f;

    public int points = 500;

    public FMODUnity.EventReference shootSound;
    public FMODUnity.EventReference deathSound;
    public GameObject deathParticle;

    private void Start()
    {
        delayedTargetLocation = target.transform.position;
        isShooting = true;
        StartCoroutine(StartShootCycle());

    }

    void Update()
    {
        if (delayTimer < delay)
        {
            delayTimer += Time.deltaTime;
        }
        else
        {
            delayTimer = 0f;
            delayedTargetLocation = target.transform.position;
        }

        transform.position = Vector3.Lerp(transform.position, delayedTargetLocation, moveSpeed * Time.deltaTime);
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
            GameManagerScript.S.score += points;
            GameManagerScript.S.killedEnemies++;
            UIManagerScript.S.DisplayScorePopup(transform.position, points);
            isShooting = false;
            transform.GetChild(0).gameObject.SetActive(false);
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
