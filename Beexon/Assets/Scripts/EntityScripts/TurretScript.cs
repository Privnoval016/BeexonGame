using System.Collections;
using UnityEngine;
using FMOD.Studio;
using System.Timers;

public class TurretScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Direction turretDirection; // set by user upon Instantiation. Default is forward.    
    public int projectileOffset; // how far away from the turret position should the projectile spawn
    public int shootDelay;
    public int projectileSpeed;
    public GameObject model;
    public GameObject flower;
    private float elapsedTimeStretch;
    public float elapsedTimeShrink;
    public float elapsedTimeGrow;
    public float shootSpeed = 10f;
    private int playingShoot;
    private Vector3 targetScale;

    public FMODUnity.EventReference shootSound;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Make turret model different based on turretDirection

        switch (turretDirection)
        {
            case Direction.Backward:
                model.transform.localRotation = Quaternion.Euler(90, 180, 0);
                break;
            case Direction.Right:
                model.transform.localRotation = Quaternion.Euler(90, 90, 0);
                break;
            case Direction.Left:
                model.transform.localRotation = Quaternion.Euler(90, 270, 0);
                break;
            case Direction.Forward:
                model.transform.localRotation = Quaternion.Euler(90, 0, 0);
                break;
        }

        targetScale = new Vector3(flower.transform.localScale.x, flower.transform.localScale.y * 1.4f, flower.transform.localScale.z);

        StartCoroutine(StartShootCycle());
    }

    // Update is called once per frame
    void Update()
    {
        if (playingShoot == 1)
        {
            elapsedTimeShrink = 0;
            elapsedTimeStretch += Time.deltaTime;
            flower.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, elapsedTimeStretch * shootSpeed);

            if (model.transform.localScale.y >= 1.3f)
            {
                playingShoot = 2;
            }
        }
        else if (playingShoot == 2)
        {
            elapsedTimeShrink += Time.deltaTime;
            elapsedTimeStretch = 0;
            flower.transform.localScale = Vector3.Lerp(targetScale, Vector3.one, shootSpeed * elapsedTimeShrink);

            if (model.transform.localScale.y <= 1.1f)
            {
                playingShoot = 0;
            }
        }
    }

    IEnumerator StartShootCycle() {
        yield return new WaitForSeconds(shootDelay * Random.Range(1, 1.5f));
        if (!GameManagerScript.S.isPaused) Shoot();
        StartCoroutine(StartShootCycle());
    }

    private void Shoot() {

        playingShoot = 1;

        GameObject projectile;
        if (turretDirection == Direction.Forward) {
            projectile = Instantiate(
                projectilePrefab, 
                new Vector3(transform.position.x, transform.position.y, transform.position.z + projectileOffset), 
                projectilePrefab.transform.rotation);
        }
        else if (turretDirection == Direction.Backward) {
            projectile = Instantiate(
                projectilePrefab,
                new Vector3(transform.position.x, transform.position.y, transform.position.z - projectileOffset),
                projectilePrefab.transform.rotation);
            
            projectile.GetComponent<ProjectileScript>().projectileDirection = Direction.Backward;
        }
        else if (turretDirection == Direction.Left) {
            projectile = Instantiate(
                projectilePrefab,
                new Vector3(transform.position.x - projectileOffset, transform.position.y, transform.position.z),
                projectilePrefab.transform.rotation);

            projectile.GetComponent<ProjectileScript>().projectileDirection = Direction.Left;
        }
        else {
            projectile = Instantiate(
                projectilePrefab,
                new Vector3(transform.position.x + projectileOffset, transform.position.y, transform.position.z),
                projectilePrefab.transform.rotation);

            projectile.GetComponent<ProjectileScript>().projectileDirection = Direction.Right;
        }

        if (!GameManagerScript.S.isPaused) FMODUnity.RuntimeManager.PlayOneShot(shootSound, transform.position);
        projectile.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile.GetComponent<ProjectileScript>().parent = gameObject;
        projectile.transform.localScale *= 0.8f;
        projectile.tag = "EnemyProjectile";
    }

}
