using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] public GameObject treeTile;
    [SerializeField] public int length = 7;
    [SerializeField] public int height = 5;

    [SerializeField] public GameObject[] models;

    private GameObject[,] treeTiles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTree(int holeLength, int holeHeight)
    {
        treeTiles = new GameObject[length, height];

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tree = Instantiate(treeTile, this.transform);
                tree.transform.localPosition = new Vector3(i, j, 0);
                treeTiles[i, j] = tree;
            }
        }

        // Add holes

        int holeLengthIndex = Random.Range(0, length - holeLength);
        int holeHeightIndex = Random.Range(0, height - holeHeight);
        for (int j = 0; j < holeLength; j++)
        {
            for (int k = 0; k < holeHeight; k++)
            {
                Destroy(treeTiles[holeLengthIndex + j, holeHeightIndex + k]);
                treeTiles[holeLengthIndex + j, holeHeightIndex + k] = null;
            }
        }

        RenderTiles();
    }

    private void RenderTiles()
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (treeTiles[i, j] != null)
                {
                    if (j + 1 >= height || treeTiles[i, j + 1] == null)
                    {
                        if (j - 1 >= 0 && treeTiles[i, j - 1] == null)
                        {
                            GameObject wallModel = Instantiate(models[6], treeTiles[i, j].transform);
                        }
                        else if (i + 1 >= length || treeTiles[i + 1, j] == null)
                        {
                            GameObject wallModel = Instantiate(models[2], treeTiles[i, j].transform);
                        }
                        else if (i - 1 < 0 || treeTiles[i - 1, j] == null)
                        {
                            GameObject wallModel = Instantiate(models[3], treeTiles[i, j].transform);
                        }
                        else
                        {
                            GameObject wallModel = Instantiate(models[1], treeTiles[i, j].transform);
                        }
                    }
                    else if (j - 1 >= 0 && treeTiles[i, j - 1] == null)
                    {
                        GameObject wallModel = Instantiate(models[1], treeTiles[i, j].transform);
                        wallModel.transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    else if (i + 1 >= length || treeTiles[i + 1, j] == null)
                    {
                        GameObject wallModel = Instantiate(models[4], treeTiles[i, j].transform);
                    }
                    else if (i - 1 < 0 || treeTiles[i - 1, j] == null)
                    {
                        GameObject wallModel = Instantiate(models[5], treeTiles[i, j].transform);
                    }
                    else
                    {
                        GameObject wallModel = Instantiate(models[0], treeTiles[i, j].transform);
                    }
                }
            }
        }
    }



}
