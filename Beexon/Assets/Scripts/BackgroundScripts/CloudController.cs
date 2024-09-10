using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private bool gameBegun;
    private float speedMult = 1;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(
            Random.Range(LevelManager.instance.cloudMinX, LevelManager.instance.cloudMaxX),
            Random.Range(LevelManager.instance.cloudMinY, LevelManager.instance.cloudMaxY),
            Random.Range(LevelManager.instance.cloudMinZ, LevelManager.instance.cloudMaxZ));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.S.isPaused) speedMult = 0.5f;
        else speedMult = 1f;

        gameBegun = true;

        transform.Translate(Vector3.back * Time.deltaTime * 1f * speedMult);
        if (transform.position.z < -5f)
        {
            transform.position = new Vector3(Random.Range(LevelManager.instance.cloudMinX, LevelManager.instance.cloudMaxX),
                Random.Range(LevelManager.instance.cloudMinY, LevelManager.instance.cloudMaxY),
                45f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cloud") && gameBegun)
        {
            transform.position = new Vector3(
                Random.Range(LevelManager.instance.cloudMinX, LevelManager.instance.cloudMaxX),
                Random.Range(LevelManager.instance.cloudMinY, LevelManager.instance.cloudMaxY),
                45f);
        }
    }
}
