using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SINGLETON
public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript S;
    public GameObject playerBee;
    public int lives;
    public float fuel;
    public float maxFuel = 1000;
    public float fuelTickSpeed = 20;
    public float altutude; // y axis
    public float score;
    public float killedEnemies;

    public bool inBoss;
    public bool isPaused;
    public bool okToRespawn = true;

    private KeyCode[] konamiCode;
    private int konamiIndex;

    private bool allowCheats = false;
    void Awake()
    {

        if (S) Destroy(gameObject);
        else S = this;

        playerBee = GameObject.FindGameObjectWithTag("Player");

        konamiCode = new KeyCode[] { KeyCode.UpArrow, KeyCode.UpArrow, 
                                     KeyCode.DownArrow, KeyCode.DownArrow, 
                                     KeyCode.LeftArrow, KeyCode.RightArrow, 
                                     KeyCode.LeftArrow, KeyCode.RightArrow, 
                                     KeyCode.B, KeyCode.A, KeyCode.Space};
    }

    // Start is called before the first frame update
    void Start()
    {
        lives = 3; fuel = maxFuel; 
        altutude = (playerBee.transform.position.y + 4f) * 0.25f - 0.4375f;
        score = 0;
        killedEnemies = 0;
        StartCoroutine(FuelClock());
    }

    // Update is called once per frame
    void Update()
    {
        altutude = (playerBee.transform.position.y + 4f) * 0.25f - 0.4375f;

        if (fuel <= 0)
        {
            playerBee.GetComponent<PlayerBeeScript>().OutOfFuel();
            GetHit();
        }

        KonamiCode();
        RunCheats();
    }

    void RunCheats()
    {
        if (!allowCheats) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            playerBee.GetComponent<CapsuleCollider>().enabled = !playerBee.GetComponent<CapsuleCollider>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            fuel = 1000;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(WaitToRestart(4));
        }
    }

    void KonamiCode()
    {
        if (konamiIndex <= konamiCode.Length - 1)
        {
            if (Input.GetKeyDown(konamiCode[konamiIndex])) konamiIndex++;
            else if (Input.anyKeyDown) konamiIndex = 0;
        }
        else
        {
            Debug.Log("Konami Code Entered");
            allowCheats = !allowCheats;
        }
    }

    IEnumerator FuelClock()
    { // decrease the fuel gradually
        yield return new WaitForSeconds(1);
        fuel -= fuelTickSpeed;
        UIManagerScript.S.UpdateFuelSlider();
        StartCoroutine(FuelClock());
    }

    public void GetHit()
    {
        lives--;
        okToRespawn = false;
        
        UIManagerScript.S.UpdateLives();

        if (lives == 0) Die();
        else Respawn();
    }

    public void Respawn()
    {
        if (isPaused) return;

        isPaused = true;

        StartCoroutine(WaitToRespawn(2));
    }

    IEnumerator WaitToRespawn(int seconds)
    {
        LevelManager.instance.StopAllCoroutines();

        yield return new WaitForSeconds(seconds - 0.5f);
        UIManagerScript.S.FadeToBlack(true);
        yield return new WaitForSeconds(0.5f);

        fuel = maxFuel;

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");

        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }

        LevelManager.instance.ResetToCheckpoint(LevelManager.instance.currentCheckpoint, 15);
        foreach (GameObject enemy in LevelManager.instance.currentFlyingEnemies)
        {
            Destroy(enemy);
        }
        LevelManager.instance.waspCircleSpawned = false;
        LevelManager.instance.currentFlyingEnemies.Clear();

        if (LevelManager.instance.bossInstance != null) Destroy(LevelManager.instance.bossInstance);
        LevelManager.instance.bossInstance = null;

        yield return new WaitForSeconds(1);

        if (LevelManager.instance.currentArea == LevelArea.Boss)
            LevelManager.instance.StartCoroutine(LevelManager.instance.DelayedBossSpawn(1));

        UIManagerScript.S.FadeToBlack(false);
        yield return new WaitForSeconds(0.2f);
        isPaused = false;
        okToRespawn = true;
    }

    IEnumerator WaitToRestart(int seconds)
    {
        LevelManager.instance.StopAllCoroutines();
        UIManagerScript.S.PlayReset(1);
        SoundManager.instance.PlayWin();
        yield return new WaitForSeconds(seconds);

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }
        fuel = maxFuel;

        SoundManager.instance.SetBackgroundMusic(0);

        foreach (GameObject enemy in LevelManager.instance.currentFlyingEnemies)
        {
            Destroy(enemy);
        }
        LevelManager.instance.waspCircleSpawned = false;
        LevelManager.instance.currentFlyingEnemies.Clear();
        Destroy(LevelManager.instance.bossInstance);
        LevelManager.instance.bossInstance = null;

        LevelManager.instance.RestartCycle();

        UIManagerScript.S.PlayReset(-1);
        isPaused = false;
        okToRespawn = true;
    }

    private void Die()
    {
        isPaused = true;

        StartCoroutine(StartDie(2));
    }

    IEnumerator StartDie(float seconds)
    {
        LevelManager.instance.StopAllCoroutines();
        yield return new WaitForSeconds(seconds);

        UIManagerScript.S.FadeRestart(true);

        SoundManager.instance.PlayGameOver();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        if (isPaused) return;

        isPaused = true;

        StartCoroutine(WaitToRestart(4)); 
    }

}
