using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class HeadBallMovement : MonoBehaviour
{
    public Transform centerPoint;
    public float radius = 2.0f;
    public float moveSpeed = 10.0f;
    public float targetY = 5.0f;

    public enum MovementPhase { MovingUpRight, DrawingCircle, MovingDownRight }
    public MovementPhase currentPhase;
    private float angle = 0f;
    private Vector3 startPosition;

    public GameObject projectilePrefab;
    public int projectileSpeed = 5;
    public float projectileOffset = 0.5f;
    public float shootDelay = 1f;
    public bool isShooting = true;

    public int points = 700;

    private float elapsedTime = 0f;

    public FMODUnity.EventReference shootSound;
    public FMODUnity.EventReference deathSound;
    public GameObject deathParticle;

    void Start()
    {
        currentPhase = MovementPhase.MovingUpRight;
        startPosition = transform.position;
        isShooting = true;
        StartCoroutine(StartShootCycle());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        switch (currentPhase)
        {
            case MovementPhase.MovingUpRight:
                MoveUpRight();
                break;

            case MovementPhase.DrawingCircle:
                DrawCircle();
                break;

            case MovementPhase.MovingDownRight:
                MoveDownRight();
                break;
        }
    }

    public void MoveUpRight()
    {
        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.position.y + Mathf.Sin(angle) * radius;
        float z = centerPoint.position.z;

        transform.position = Vector3.Lerp(startPosition, new Vector3(x, y, z), moveSpeed * elapsedTime / 2f);

        if (transform.localPosition.y >= targetY - 0.1f)
        {
            currentPhase = MovementPhase.DrawingCircle;
        }
    }

    public void DrawCircle()
    {
        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.position.y + Mathf.Sin(angle) * radius;
        float z = centerPoint.position.z;

        transform.position = new Vector3(x, y, z);

        angle += moveSpeed / radius * Time.deltaTime;

        if (transform.position.y <= targetY)
        {
            angle -= Mathf.PI * 2;
            currentPhase = MovementPhase.MovingDownRight;
        }
    }

    public void MoveDownRight()
    {
        transform.position += new Vector3(moveSpeed * Time.deltaTime, -0.25f * moveSpeed * Time.deltaTime, 0);
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
            isShooting = false;
            UIManagerScript.S.DisplayScorePopup(transform.position, points);

            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
