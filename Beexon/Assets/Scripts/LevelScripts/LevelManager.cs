using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum LevelArea
{
    FlowerField,
    WallParkour,
    Sky,
    Boss,
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    [Header("Area Information")]
    public LevelArea currentArea;
    [SerializeField] public int[] levelSwapRows;

    [Header("Row Information")]
    public int numRows;

    public int initialRows = 34;

    public float rowSpeed;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private GameObject rowContainer;
    [SerializeField] private Vector3 spawnPosition;

    private List<GameObject> rows = new List<GameObject>();

    private List<FieldSpawn> rowEntities = new List<FieldSpawn>();
    private List<RowSegInfo> rowSegInfos = new List<RowSegInfo>();

    [SerializeField] private int[] checkpoints;
    public int currentCheckpoint;

    [SerializeField] private RowSegInfo[] rowSegObjs;
    [SerializeField] private int[] textureSwapIndices;


    [Header("Entity Spawn Information")]

    [SerializeField] private float spawnRate;
    [SerializeField] private int wallInterval;

    [SerializeField] private FieldSpawn[] walls;
    [SerializeField] private float[] wallSpawnRates;
    [SerializeField] private FieldSpawn[] entityInfos;
    [SerializeField] private float[] entitySpawnRates;

    [Header("Flying Enemy Information")]
    [SerializeField] private GameObject seekingWasp;
    [SerializeField] private float seekingWaspSpawnRate;
    [SerializeField] private GameObject waspCircle;
    [SerializeField] private float waspCircleSpawnRate;
    [SerializeField] private GameObject waspStraight;
    [SerializeField] private float waspStraightSpawnRate;
    [SerializeField] private int maxStraightWasps;

    [Header("Boss Information")]
    [SerializeField] private GameObject bossPrefab;

    [Header("Background Information")]
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] public float cloudMaxX;
    [SerializeField] public float cloudMaxY;
    [SerializeField] public float cloudMinY;
    [SerializeField] public float cloudMinX;
    [SerializeField] public float cloudMinZ;
    [SerializeField] public float cloudMaxZ;

    [SerializeField] public Vector3 cloudWallPos;
    [SerializeField] public GameObject cloudWall;

    [SerializeField] private int maxClouds;

    [SerializeField] private GameObject speedLines;

    [SerializeField] private GameObject fireflies;

    [HideInInspector]
    public enum BossSpawnState
    {
        NeedToSpawn,
        Spawned,
        NeedToReset
    }

    [HideInInspector]
    public BossSpawnState bossState;

    [HideInInspector]
    public int numStraightWasps;

    [HideInInspector]
    public GameObject bossInstance;

    [HideInInspector]
    public List<GameObject> currentFlyingEnemies;

    [HideInInspector]
    public bool waspCircleSpawned = false;

    private float rowZ;

    private int spawnCooldown;


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

        RestartCycle();
    }

    private void SpawnInitialRows(int n)
    {
        for (int i = 0; i < n; i++)
        {
            numRows++;
            GameObject row = Instantiate(rowPrefab, rowContainer.transform);

            row.GetComponent<RowController>().segInfo = rowSegObjs[GetTextureIndex(numRows)];
            row.GetComponent<RowController>().rowIndex = numRows;

            row.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, i);

            rows.Add(row);
            rowEntities.Add(null);
        }
    }

    private void GenerateNRows(int n)
    {
        spawnCooldown = 0;
        for (int i = 0; i < n; i++)
        {
            rowEntities.Add(null);
        }
    }

    private void RandomizeLayout(int totalRows, int wallInterval, float spawnRate)
    {
        //rowEntities.Add(walls[0]);

        for (int i = 0; i < totalRows; i++)
        {
            if (spawnCooldown == 0)
            {
                FieldSpawn entity = null;

                if (i % wallInterval == 0)
                {
                    entity = GetEntityFromProbability(walls, wallSpawnRates);
                    spawnCooldown += 6;
                }
                else
                {
                    int spawnProbability = Random.Range(0, 100);

                    if (spawnProbability < spawnRate)
                    {
                        entity = GetEntityFromProbability(entityInfos, entitySpawnRates);
                    }
                    else
                    {
                        entity = null;
                    }

                }

                rowEntities.Add(entity);
                if (entity != null) spawnCooldown += entity.occupiedZTiles;

            }
            else
            {
                rowEntities.Add(null);
                spawnCooldown--;
            }
        }
    }

    private FieldSpawn GetEntityFromProbability(FieldSpawn[] entities, float[] spawnRates)
    {
        float totalRate = spawnRates.Sum();

        float randomValue = Random.Range(0, totalRate);

        for (int i = 0; i < entities.Length; i++)
        {
            if (randomValue < spawnRates[i])
            {
                return entities[i];
            }
            else
            {
                randomValue -= spawnRates[i];
            }
        }

        return entities[entities.Length - 1];
    }

    private void SpawnBossFight()
    {
        switch (bossState)
        {
            case BossSpawnState.NeedToSpawn:
                StartCoroutine(DelayedBossSpawn(8));
                break;
            case BossSpawnState.Spawned:
                break;
            case BossSpawnState.NeedToReset:
                break;

        }

    }

    public IEnumerator DelayedBossSpawn(int seconds)
    {
        bossState = BossSpawnState.Spawned;
        yield return new WaitForSeconds(seconds);
        bossInstance = Instantiate(bossPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.S.isPaused) return;
         
        switch (currentArea)
        {
            case LevelArea.Sky:
                SpawnFlyingEnemies();
                break;
            case LevelArea.Boss:
                SpawnBossFight();
                break;
        }

        UpdateRows();

        for (int i = checkpoints.Length - 1; i >= 0; i--)
        {
            if (numRows - 24 >= checkpoints[i])
            {
                currentCheckpoint = i;
                break;
            }
        }

        for (int i = levelSwapRows.Length - 1; i >= 0; i--)
        {
            LevelArea currentState = currentArea;

            if (numRows - 24 >= levelSwapRows[i])
            {
                currentArea = (LevelArea)i;

                if (currentState != currentArea)
                {
                    SoundManager.instance.SetBackgroundMusic((int)currentArea);

                    if (currentArea == LevelArea.Sky)
                    {
                        foreach (Transform t in speedLines.transform)
                        {
                            t.gameObject.SetActive(true);
                            t.GetComponent<ParticleSystem>().Play();
                        }
                    }
                    else
                    {
                        foreach (Transform t in speedLines.transform)
                        {
                            t.GetComponent<ParticleSystem>().Pause();
                            t.gameObject.SetActive(false);
                        }
                    }

                    if (currentArea == LevelArea.FlowerField || currentArea == LevelArea.WallParkour)
                    {
                        fireflies.SetActive(true);
                    }
                    else
                    {
                        fireflies.SetActive(false);
                    }
                }

                break;
            }
        }

        if (currentArea == LevelArea.Sky)
        {
            foreach (Transform t in speedLines.transform)
            {
                t.gameObject.SetActive(true);
                t.GetComponent<ParticleSystem>().Play();
            }
        }
        else
        {
            foreach (Transform t in speedLines.transform)
            {
                t.GetComponent<ParticleSystem>().Pause();
                t.gameObject.SetActive(false);
            }
        }
        
    }

    private void UpdateRows()
    {
        if (rows.Count <= 0) return;

        rows[0].transform.position -= Vector3.forward * rowSpeed * Time.deltaTime;

        if (rows[0].transform.position.z <= -5f)
        {
            Destroy(rows[0]);
            rows.Remove(rows[0]);
            if (numRows < rowEntities.Count) LoadRow(rowEntities[numRows]);
        }

        for (int i = 1; i < rows.Count; i++)
        {
            rows[i].transform.position = rows[i - 1].transform.position + Vector3.forward;
        }
    }

    public void LoadRow(FieldSpawn entity)
    {
        //Debug.Log("Loading row");
        numRows++;
        GameObject row = Instantiate(rowPrefab, rowContainer.transform);

        if (numRows == levelSwapRows[2])
        {
            GameObject wall = Instantiate(cloudWall, row.transform);
            wall.transform.localPosition = cloudWallPos;
        }

        row.GetComponent<RowController>().segInfo = rowSegObjs[GetTextureIndex(numRows)];
        row.GetComponent<RowController>().rowIndex = numRows;

        row.transform.localPosition = spawnPosition;

        if (entity != null)
            SpawnEntity(entity, row);

        rows.Add(row);
    }

    public void SpawnEntity(FieldSpawn info, GameObject row)
    {
        GameObject fieldObject = Instantiate(info.prefab, row.transform);
        fieldObject.transform.localPosition = new Vector3(info.spawnLocalPos.x, info.spawnLocalPos.y, info.spawnLocalPos.z + (info.occupiedZTiles - 1) * 0.5f);

        if (info.randomizeXPos)
        {
            fieldObject.transform.localPosition += new Vector3(Random.Range((info.occupiedXTiles - 1) * 0.5f - 4.5f, 1.5f), 0, 0);
        }

        if (info.spawnLocalRot != Vector3.zero)
            fieldObject.transform.localRotation = Quaternion.Euler(info.spawnLocalRot);
        if (info.spawnLocalScale != Vector3.zero)
            fieldObject.transform.localScale = info.spawnLocalScale;

        switch (info.entityType)
        {
            case EntityType.FallenTree:
                fieldObject.GetComponent<TreeController>().SpawnTree(3, 2);
                if (currentArea != LevelArea.WallParkour && Random.Range(0, 100) < seekingWaspSpawnRate)
                {
                    GameObject wasp = Instantiate(seekingWasp, row.transform);               
                }
                break;

            case EntityType.TreeWithWind:
                fieldObject.GetComponent<TreeController>().SpawnTree(3, 2);
                if (currentArea != LevelArea.WallParkour && Random.Range(0, 100) < seekingWaspSpawnRate)
                {
                    GameObject wasp = Instantiate(seekingWasp, row.transform);
                }
                break;
            case EntityType.Wind:
                if (currentArea != LevelArea.WallParkour && Random.Range(0, 100) < seekingWaspSpawnRate)
                {
                    GameObject wasp = Instantiate(seekingWasp, row.transform);
                }
                break;
            case EntityType.Turret:
                int direction = Random.Range(0, 3);
                if (direction <= 1)
                {
                    fieldObject.GetComponent<TurretScript>().turretDirection = Direction.Backward;
                }
                else
                {
                    if (fieldObject.transform.position.x < 0)
                    {
                        fieldObject.GetComponent<TurretScript>().turretDirection = Direction.Right;
                    }
                    else
                    {
                        fieldObject.GetComponent<TurretScript>().turretDirection = Direction.Left;
                    }
                }
                break;
        }
    }

    public void SpawnFlyingEnemies()
    {
        print(numStraightWasps);
        if (!waspCircleSpawned)
        {
            if (Random.Range(0, 100) < waspCircleSpawnRate)
            {
                GameObject wasp = Instantiate(waspCircle);
                waspCircleSpawned = true;
            }
        }

        if (Random.Range(0, 100f) < seekingWaspSpawnRate / (wallInterval * rowSpeed))
        {
            GameObject wasp = Instantiate(seekingWasp);
        }

        if (numStraightWasps < maxStraightWasps && Random.Range(0, 100f) < waspStraightSpawnRate / (wallInterval * rowSpeed))
        {
            GameObject wasp = Instantiate(waspStraight);
        }
    }

    public void AddToFlyingEnemies(GameObject enemy)
    {
        currentFlyingEnemies.Add(enemy);
    }

    public void RemoveFromFlyingEnemies(GameObject enemy)
    {
        if (currentFlyingEnemies.Contains(enemy)) currentFlyingEnemies.Remove(enemy);
    }

    public void ResetToCheckpoint(int checkpoint, int bufferLength)
    {

        foreach (Transform row in rowContainer.transform)
        {
            Destroy(row.gameObject);
        }

        rows.Clear();

        numRows = checkpoints[checkpoint];

        for (int i = 0; i < initialRows; i++)
        {
            numRows++;
            GameObject row = Instantiate(rowPrefab, rowContainer.transform);
            row.GetComponent<RowController>().segInfo = rowSegObjs[GetTextureIndex(numRows)];
            row.GetComponent<RowController>().rowIndex = numRows;
            row.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, i);
            rows.Add(row);

            if (numRows == levelSwapRows[2])
            {
                GameObject wall = Instantiate(cloudWall, row.transform);
                wall.transform.localPosition = cloudWallPos;
            }

            if (i >= bufferLength && rowEntities[numRows] != null)
            {
                SpawnEntity(rowEntities[numRows], row);
            }
        }
    }

    private int GetTextureIndex(int row)
    {
        for (int i = textureSwapIndices.Length - 1; i >= 0; i--)
        {
            if (row >= textureSwapIndices[i])
            {
                return i;
            }
        }
        return 0;
    }

    public void RestartCycle()
    {
        currentCheckpoint = 0;

        rowEntities.Clear();
        numRows = 0;

        foreach (GameObject row in rows)
        {
            Destroy(row);
        }
        rows.Clear();

        SpawnClouds();

        SpawnInitialRows(initialRows);
        RandomizeLayout(levelSwapRows[1] - initialRows, wallInterval, spawnRate);
        RandomizeLayout(levelSwapRows[2] - levelSwapRows[1], wallInterval / 2, spawnRate * 0.25f);
        GenerateNRows(levelSwapRows[3] - levelSwapRows[2]);
        GenerateNRows(35);

        currentArea = LevelArea.FlowerField;
        bossState = BossSpawnState.NeedToSpawn;
    }

    private void SpawnClouds()
    {
        for (int i = 0; i < maxClouds; i++)
        {
            Instantiate(cloudPrefab);
        }
    }
}
