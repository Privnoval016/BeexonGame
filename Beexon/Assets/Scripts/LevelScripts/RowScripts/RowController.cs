using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RowController : MonoBehaviour
{
    [SerializeField] public RowSegInfo segInfo;
    [SerializeField] public FieldSpawn spawnInfo;
    public GameObject rock;
    private List<GameObject> fieldObjects;
    public int rowIndex;
    private int numFrames;
    private int swayMult = 1;

    void Start()
    {
        fieldObjects = new List<GameObject>();

        rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameObject segmentContainer = transform.Find("Segments").gameObject;

        for (int i = 0; i < segmentContainer.transform.childCount; i++)
        {
            GameObject segment = segmentContainer.transform.GetChild(i).gameObject;

            if (segInfo.blockMaterial.Length == 0)
            {
                segment.GetComponent<MeshRenderer>().enabled = false;
            }
            else if (rowIndex % 2 == 0)
            {
                if (i % 2 == 0)
                {
                    segment.GetComponent<MeshRenderer>().material = segInfo.blockMaterial[0];
                }
                else
                {
                    segment.GetComponent<MeshRenderer>().material = segInfo.blockMaterial[1];
                }
            }
            else
            {
                if (i % 2 == 0)
                {
                    segment.GetComponent<MeshRenderer>().material = segInfo.blockMaterial[2];
                }
                else
                {
                    segment.GetComponent<MeshRenderer>().material = segInfo.blockMaterial[3];
                }
            }

            if (segInfo.designPrefab != null)
            {
                int numSpawn = Random.Range(0, 100);

                if (numSpawn > segInfo.spawnChance)
                {
                    for (int j = 0; j < numSpawn / 2; j++)
                    { // Spawn more than one object
                        GameObject grass = Instantiate(segInfo.designPrefab, segment.transform);
                        fieldObjects.Add(grass);
                        grass.transform.localScale *= Random.Range(0.3f, 0.7f);
                        grass.transform.localPosition += new Vector3(Random.Range(-5f, 5f), Random.Range(-1.5f, 0.5f), Random.Range(-5f, 5f));
                    }
                }
            }
        }

        GameObject wall = transform.Find("Wall").gameObject;
        if (segInfo.wallMaterial != null)
        {
            wall.GetComponent<MeshRenderer>().material = segInfo.wallMaterial;
        }
        else
        {
            wall.GetComponent<MeshRenderer>().enabled = false;
            rock.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        numFrames++;
        foreach (GameObject g in fieldObjects)
        {
            g.transform.Translate(Vector3.forward * 0.2f * Time.fixedDeltaTime * swayMult);
            //g.transform.Rotate(Vector3.forward * swayMult * Time.fixedDeltaTime);

            if (numFrames >= 60)
            {
                swayMult *= -1;
                numFrames = 0;
            }

        }
    }
}
