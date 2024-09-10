using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWaspController : MonoBehaviour
{
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public float speed = 5f;

    private float elapsedTime;

    public GameObject tailWasp;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.instance.AddToFlyingEnemies(gameObject);
        elapsedTime = 0f;
        LevelManager.instance.waspCircleSpawned = true;
        transform.position = spawnPosition;

        foreach (Transform child in transform)
        {
            Debug.Log(child.name);
            if (child.GetComponent<HeadBallMovement>() != null)
            {
                child.GetComponent<HeadBallMovement>().enabled = false;
            }
            else
            {
                child.GetComponent<BallMovement>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(spawnPosition, endPosition, speed * elapsedTime * 0.5f);

        if (transform.position.z <= endPosition.z + 0.1f)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<HeadBallMovement>() != null)
                {
                    child.GetComponent<HeadBallMovement>().enabled = true;
                }
                else
                {
                    child.GetComponent<BallMovement>().enabled = true;
                }
            }
        }

        if (tailWasp.transform.localPosition.x >= 17f)
        {
            LevelManager.instance.waspCircleSpawned = false;
            LevelManager.instance.RemoveFromFlyingEnemies(this.gameObject);
            Destroy(gameObject);
        }
    }
}
