using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Forward, Backward, Left, Right
}

public class ProjectileScript : MonoBehaviour
{
    public Direction projectileDirection; // set by user manually upon Instantiation. Default is forward.
    public int speed;
    private Vector3 projectileRotation;
    public GameObject[] deathParticles;
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        if (projectileDirection == Direction.Forward) {
            projectileRotation = new Vector3(90, 0, 0);
        }
        else if (projectileDirection == Direction.Backward) {
            projectileRotation = new Vector3(90, 0, 180);
        }
        else if (projectileDirection == Direction.Left) {
            projectileRotation = new Vector3(90, 0, 90);
        }
        else if (projectileDirection == Direction.Right) {
            projectileRotation = new Vector3(90, 0, -90);
        }
        transform.eulerAngles = projectileRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        if (projectileDirection == Direction.Forward) {
            currentPosition.z += Time.deltaTime * speed;
        }
        else if (projectileDirection == Direction.Backward) {
            currentPosition.z -= Time.deltaTime * speed;
        }
        else if (projectileDirection == Direction.Left) {
            currentPosition.x -= Time.deltaTime * speed;
        }
        else if (projectileDirection == Direction.Right) {
            currentPosition.x += Time.deltaTime * speed;
        }
        
        transform.position = currentPosition;

        if (transform.position.x > 15f || transform.position.x < -15f || transform.position.z > 60f || transform.position.z < -20f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        // for when the projectile hits something.
        // the object that the projectile hits HAS to have a rigidbody and check isTrigger tickbox under collider
        // remmeber to turn off gravity on rigidbody

        if (other.gameObject == parent) return;

        if (CompareTag("EnemyProjectile"))
        {
            if (other.CompareTag("Player") || other.CompareTag("Wall"))
            {
                Vector3 particleRotation = transform.rotation.eulerAngles * -1;
                if (other.CompareTag("Wall"))
                {
                    GameObject particle = Instantiate(deathParticles[0], transform.position, Quaternion.Euler(particleRotation));
                    particle.transform.localScale *= 0.3f;
                }
                Destroy(gameObject);
            }
        }
        else if (CompareTag("Projectile"))
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Wall") || other.CompareTag("Boss"))
            { 
                Vector3 particleRotation = transform.rotation.eulerAngles * -1 - new Vector3(90, 0, 0);
                if (!other.CompareTag("Enemy"))
                {
                    GameObject particle = Instantiate(deathParticles[1], transform.position, Quaternion.Euler(particleRotation));
                    particle.transform.localScale *= 0.3f;
                }

                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("DeathTrigger"))
        {
            Destroy(gameObject);
        }
    }
}
