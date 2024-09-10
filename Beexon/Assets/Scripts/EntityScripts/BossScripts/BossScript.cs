using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.EventSystems;
using System.Net.Sockets;

// map is 8 units wide
// 5 tall 

public class BossScript : MonoBehaviour
{
    public enum BossState
    {
        Entering,
        Attacking,
        Retreating,
        Death
    }

    public BossState bossState;

    public GameObject bossProjectile;
    public GameObject homingMissile; // they rhyme. 
    public int bossHealth;
    public int hits;
    public int points;
    public int numProjectiles = 6;

    public float maxX, maxY, minX, minY;
    public float xRange, yRange;

    public FMODUnity.EventReference deathSound;
    public FMODUnity.EventReference explosionSound;
    public FMODUnity.EventReference projectileSound;
    public GameObject deathParticle;
    public GameObject model;

    private bool startedAttacking;

    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public Vector3 approachLimit;
    public float speed;

    private float elapsedTime;

    private List<GameObject> projectiles = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        // horizontalSweep(5f, 5f, 5f, 6);
        // verticalSweep(5, 1, 10, 5);
        // targetPlayerPositionShots(new Vector3(6.5f, 0.65f, 54f));
        

        xRange = maxX - minX;
        yRange = maxY - minY;

        elapsedTime = 0;

        bossState = BossState.Entering;
        LevelManager.instance.bossState = LevelManager.BossSpawnState.Spawned;
        transform.position = spawnPosition;

    }

    // Update is called once per frame
    void Update()
    {
        switch (bossState)
        {
            case BossState.Entering:
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(spawnPosition, endPosition, speed * elapsedTime * 0.5f);

                if (transform.position.z <= endPosition.z + 0.1f)
                {
                    bossState = BossState.Attacking;
                }
                break;
            case BossState.Attacking:
                if (!startedAttacking)
                {
                    StartCoroutine(attackCycle());
                    startedAttacking = true;
                }
                
                transform.Translate(Vector3.back * speed * Time.deltaTime * 0.25f);

                if (transform.position.z < approachLimit.z)
                {
                    elapsedTime = 0;
                    bossState = BossState.Retreating;
                }

                break;

            case BossState.Retreating:
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(approachLimit, endPosition, speed * elapsedTime * 0.7f);

                if (transform.position.z >= endPosition.z - 0.1f)
                {
                    bossState = BossState.Attacking;
                }
                break;

            case BossState.Death:
                StopCoroutine(attackCycle());
                foreach (GameObject projectile in projectiles)
                {
                    Destroy(projectile);
                }
                break;
        }
    }

    IEnumerator attackCycle() {

        float sweep1 = Random.Range(minY, maxY);
        float sweep2 = Random.Range(minY, maxY);
        while (sweep2 <= sweep1 + 0.35f && sweep2 >= sweep1 - 0.35f) sweep2 = Random.Range(minY, maxY);

        StartCoroutine(PlayAttackSound(2));
        
        horizontalSweep(2f, sweep1, 1, numProjectiles);
        horizontalSweep(2f, sweep2, 1, numProjectiles);

        yield return new WaitForSeconds(2);

        if (GameManagerScript.S.isPaused) yield break;

        sweep1 = Random.Range(minX, maxX);
        sweep2 = Random.Range(minX, maxX);
        while (sweep2 <= sweep1 + 0.35f && sweep2 >= sweep1 - 0.35f) sweep2 = Random.Range(minX, maxX);

        StartCoroutine(PlayAttackSound(2));

        verticalSweep(2f, sweep1, 1, numProjectiles);
        verticalSweep(2f, sweep2, 1, numProjectiles);

        yield return new WaitForSeconds(2);

        if (GameManagerScript.S.isPaused) yield break;

        StartCoroutine(PlayAttackSound(0.75f));

        targetPlayerPositionShots(new Vector3(transform.position.x - 1, transform.position.y - 1, transform.position.z));
        targetPlayerPositionShots(new Vector3(transform.position.x + 1, transform.position.y - 1, transform.position.z));

        yield return new WaitForSeconds(2);

        if (GameManagerScript.S.isPaused) yield break;

        StartCoroutine(attackCycle());
    }

    IEnumerator PlayAttackSound(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (GameManagerScript.S.isPaused) yield break;

        FMODUnity.RuntimeManager.PlayOneShot(projectileSound, transform.position);
    }

    void targetPlayerPositionShots(Vector3 spawnLocation) { // projectiles that aim at the player's current position. 
        Vector3 playerPosition = GameManagerScript.S.playerBee.transform.position;
        GameObject projectile = Instantiate(bossProjectile, spawnLocation, Quaternion.identity);
        projectile.GetComponent<BossProjectileScript>().target = playerPosition;
        projectile.GetComponent<BossProjectileScript>().telegraphWaitTime = 0.75f;
        projectiles.Add(projectile);
        projectile.transform.LookAt(playerPosition);
    }

    // frontOffset: how forward the sweep spawns relative to the location of the boss.
    // height: how high the current sweep will spawn
    // gap: how spaced out the projectiles are
    // quantity: how many projectiles to spawn
    void horizontalSweep(float frontOffset, float height, float gap, int quantity) { 
        float z = transform.position.z - frontOffset;
        float y = height;
        if (quantity % 2 == 0) { // even
            Vector3 firstSpawn = new Vector3(transform.position.x - gap/2, y, z);
            Vector3 secondSpawn = new Vector3(transform.position.x + gap/2, y, z);
            for (int i = 0; i < quantity / 2; i++)
            {
                GameObject projectile = Instantiate(bossProjectile, firstSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(firstSpawn.x, firstSpawn.y, firstSpawn.z - 50);
                projectiles.Add(projectile);
                
                projectile = Instantiate(bossProjectile, secondSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(secondSpawn.x, secondSpawn.y, secondSpawn.z - 50);
                projectiles.Add(projectile);

                firstSpawn.x -= gap; secondSpawn.x += gap;
            }
        }
        else {
            GameObject projectile = Instantiate(bossProjectile, new Vector3(transform.position.x, y, z), Quaternion.identity);
            projectile.GetComponent<BossProjectileScript>().target = new Vector3(transform.position.x, y, z - 50);
            projectiles.Add(projectile);

            Vector3 firstSpawn = new Vector3(transform.position.x - gap, y, z);
            Vector3 secondSpawn = new Vector3(transform.position.x + gap, y, z);
            for (int i = 0; i < (quantity - 1) / 2; i++) 
            {
                projectile = Instantiate(bossProjectile, firstSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(firstSpawn.x, firstSpawn.y, firstSpawn.z - 50);
                projectiles.Add(projectile);

                projectile = Instantiate(bossProjectile, secondSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(secondSpawn.x, secondSpawn.y, secondSpawn.z - 50);
                projectiles.Add(projectile);
                firstSpawn.x -= gap; secondSpawn.x += gap;
            }
        }
    }
    
    void verticalSweep(float frontOffset, float x, float gap, int quantity) {
        float z = transform.position.z - frontOffset;
        if (quantity % 2 == 0) {
            Vector3 firstSpawn = new Vector3(x, transform.position.y - gap/2, z);
            Vector3 secondSpawn = new Vector3(x, transform.position.y + gap/2, z);
            for (int i = 0; i < quantity/2; i++) {
                GameObject projectile = Instantiate(bossProjectile, firstSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(firstSpawn.x, firstSpawn.y, firstSpawn.z - 50);
                projectiles.Add(projectile);

                projectile = Instantiate(bossProjectile, secondSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(secondSpawn.x, secondSpawn.y, secondSpawn.z - 50);
                projectiles.Add(projectile);

                firstSpawn.y -= gap; secondSpawn.y += gap;
            }
        }
        else {
            GameObject projectile = Instantiate(bossProjectile, new Vector3(x, transform.position.y, z), Quaternion.identity);
            projectile.GetComponent<BossProjectileScript>().target = new Vector3(x, transform.position.y, z - 50);
            projectiles.Add(projectile);

            Vector3 firstSpawn = new Vector3(x, transform.position.y - gap, z);
            Vector3 secondSpawn = new Vector3(x, transform.position.y + gap, z);
            for (int i = 0; i < (quantity - 1) / 2; i++) 
            {
                projectile = Instantiate(bossProjectile, firstSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(firstSpawn.x, firstSpawn.y, firstSpawn.z - 50);
                projectiles.Add(projectile);

                projectile = Instantiate(bossProjectile, secondSpawn, Quaternion.identity);
                projectile.GetComponent<BossProjectileScript>().target = new Vector3(secondSpawn.x, secondSpawn.y, secondSpawn.z - 50);
                projectiles.Add(projectile);

                firstSpawn.y -= gap; secondSpawn.y += gap;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Projectile")
        {
            hits++;
            if (hits >= bossHealth)
            {
                StartCoroutine(PlayDeathSound());
            }
        }
    }

    IEnumerator PlayDeathSound()
    {
        bossState = BossState.Death;
        GameManagerScript.S.score += points;
        GameManagerScript.S.isPaused = true;
        FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);
        yield return new WaitForSeconds(3f);
        GameObject explosion = Instantiate(deathParticle, transform.position, Quaternion.identity);
        //Hide model when in
        GetComponent<MeshRenderer>().enabled = false;
        model.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot(explosionSound, transform.position);
        yield return new WaitForSeconds(1f);
        LevelManager.instance.bossState = LevelManager.BossSpawnState.NeedToReset;
        GameManagerScript.S.isPaused = false;
        GameManagerScript.S.RestartGame();
        Destroy(gameObject);
    }
}
