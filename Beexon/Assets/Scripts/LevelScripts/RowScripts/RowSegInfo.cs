using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RowSegInfo", menuName = "ScriptableObjects/RowSegInfo", order = 1)]
public class RowSegInfo : ScriptableObject
{
    public Material[] blockMaterial;
    public Material wallMaterial;

    public GameObject designPrefab;
    public int spawnChance;

}
