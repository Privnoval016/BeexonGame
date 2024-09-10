using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTankScript : MonoBehaviour
{
    public float fuelContent; // how much fuel to reward the player when destroyed

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            GameManagerScript.S.fuel += fuelContent;
            if (GameManagerScript.S.fuel > 1000) GameManagerScript.S.fuel = 1000;
            UIManagerScript.S.UpdateFuelSlider();
        }
    }
}
