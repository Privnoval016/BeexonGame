using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class KillController : MonoBehaviour
{
    public FieldSpawn info;
    public FMODUnity.EventReference deathSound;
    public GameObject deathParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (!GameManagerScript.S.isPaused) FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);

            GameObject particle = Instantiate(deathParticles, transform.parent);
            particle.transform.localPosition = transform.localPosition;
            
            GameManagerScript.S.score += info.points;

            UIManagerScript.S.DisplayScorePopup(transform.position, info.points);

            if (info.entityType != EntityType.Flower) GameManagerScript.S.killedEnemies++;
            Destroy(gameObject);
        }
    }
}
