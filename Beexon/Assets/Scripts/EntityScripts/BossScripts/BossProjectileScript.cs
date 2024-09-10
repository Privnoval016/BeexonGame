using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BossProjectileScript : MonoBehaviour
{
    public int speed;
    public GameObject projectileDestroyParticle; // prefab for the particle system object that spawns in whenever the projectile destroys something
    public Vector3 target; // position of the projectile's target
    public bool telegraph; // if (telegraph == true), the projectile spawns but doesn't move. the projectile only starts moving after telegraphWaittelegraphWaitTime
    public float telegraphWaitTime;
    private Vector3 projectileStartingPosition;
    private float startTime, journeyLength;
    private bool moving, fading;
    private float currentDuration;

    Vector3 finalScale;
    

    // Start is called before the first frame update
    void Start()
    {
        finalScale = transform.localScale;
        currentDuration = 0;
        moving = false;
        projectileStartingPosition = transform.position;
        journeyLength = Vector3.Distance(projectileStartingPosition, target);
        StartCoroutine(Initiate());
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            float distanceCovered = (Time.time - startTime) * speed;
            transform.position = Vector3.Lerp(projectileStartingPosition, target, distanceCovered / journeyLength);
            if (Math.Abs(distanceCovered - journeyLength) <= 0.2f)
            { // reached the end of its travel distance
                Destroy(gameObject);
            }
        }
        if (fading)
        {
            // fade in
            currentDuration += Time.deltaTime;
            float factor = Mathf.Lerp(0.2f, 1f, currentDuration / (telegraphWaitTime / 4));
            transform.localScale = finalScale * factor;
        }
    }

    IEnumerator Initiate()
    {
        transform.LookAt(target);
        if (telegraph)
        {
            fading = true;
            yield return new WaitForSeconds(telegraphWaitTime);
            fading = false;
        }
        
        moving = true;
        startTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}