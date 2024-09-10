using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private GameObject[] layerPrefabs;
    [SerializeField] private GameObject[] layerContainers;
    [SerializeField] private float[] parallaxSpeeds;
    [SerializeField] private float[] layersStartZCoords;
    [SerializeField] private float layerOffset;
    [SerializeField] private int numberOfRepeats;
    [SerializeField] private float exitSpeed;
    private List<GameObject>[] layersList;

    // Start is called before the first frame update
    void Start()
    {
        layersList = new List<GameObject>[layerPrefabs.Length];
        
        for (int i = 0; i < layerPrefabs.Length; i++)
        {
            layersList[i] = new List<GameObject>();
            for (int j = 0; j < numberOfRepeats; j++)
            {
                GameObject newLayer = Instantiate(layerPrefabs[i], layerContainers[i].transform);
                newLayer.transform.localPosition = new Vector3(layerOffset * j, 0, layersStartZCoords[i]);
                layersList[i].Add(newLayer);
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.S.isPaused) return;

        if (LevelManager.instance.numRows >= LevelManager.instance.levelSwapRows[2] - 4)
        {
            foreach (List<GameObject> layers in layersList)
            {
                foreach (GameObject layer in layers)
                {
                    if (layer.transform.position.z >= 37)
                    {
                        layer.SetActive(false);
                    }
                    layer.transform.localPosition -= Vector3.right * Time.deltaTime * exitSpeed;
                }
            }
        }
        else
        {

            for (int i = 0; i < layersList.Length; i++)
            {
                List<GameObject> layers = layersList[i];

                layers[0].SetActive(true);
                layers[0].transform.localPosition -= Vector3.right * Time.deltaTime * parallaxSpeeds[i];

                if (layers[0].transform.localPosition.x < -1 * layerOffset)
                {
                    Destroy(layers[0]);
                    layers.Remove(layers[0]);
                    GameObject newLayer = Instantiate(layerPrefabs[i], layerContainers[i].transform);
                    newLayer.transform.localPosition = new Vector3(layers[layers.Count - 1].transform.localPosition.x + layerOffset, 0, layersStartZCoords[i]);
                    layers.Add(newLayer);
                }

                for (int j = 1; j < layers.Count; j++)
                {
                    layers[j].SetActive(true);
                    layers[j].transform.localPosition = layers[j - 1].transform.localPosition + Vector3.right * layerOffset;
                }
            }
        }
        
    }
}
