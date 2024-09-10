using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspParticleController : MonoBehaviour
{
    public bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position -= Vector3.forward * LevelManager.instance.rowSpeed * Time.deltaTime;
        }
    }
}
