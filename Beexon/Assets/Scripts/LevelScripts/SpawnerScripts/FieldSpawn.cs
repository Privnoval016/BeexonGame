using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    FallenTree,
    Flower,
    Frog,
    Wind,
    Turret,
    TreeWithWind,
    TripleFlower,
    DualTurret
}

[CreateAssetMenu(fileName = "FieldSpawn", menuName = "ScriptableObjects/FieldSpawn", order = 1)]
public class FieldSpawn : ScriptableObject
{
    public EntityType entityType;
    public int points;

    public GameObject prefab;
    public Vector3 spawnLocalPos;
    public Vector3 spawnLocalRot;
    public Vector3 spawnLocalScale;

    public int occupiedZTiles;
    
    public bool randomizeXPos;
    public int occupiedXTiles;

}
