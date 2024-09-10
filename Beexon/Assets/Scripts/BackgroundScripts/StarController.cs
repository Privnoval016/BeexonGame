using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarController : MonoBehaviour
{
    private List<GameObject> starChilds;
    public GameObject starPrefab;

    private float zOffset = 21.18f;

    // Start is called before the first frame update
    void Start()
    {
        starChilds = new List<GameObject>();

        foreach (Transform child in transform)
        {
            starChilds.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        starChilds[0].transform.localPosition -= Vector3.forward * Time.deltaTime * 0.5f;

        if (starChilds[0].transform.localPosition.z < -1 * zOffset)
        {
            Destroy(starChilds[0]);
            starChilds.Remove(starChilds[0]);
            GameObject newStar = Instantiate(starPrefab, transform);
            newStar.transform.localPosition = new Vector3(0, 0, starChilds[starChilds.Count - 1].transform.localPosition.z + zOffset);
            starChilds.Add(newStar);
        }

        for (int i = 1; i < starChilds.Count; i++)
        {
            starChilds[i].transform.localPosition = starChilds[i - 1].transform.localPosition + Vector3.forward * zOffset;
        }
    }
}
