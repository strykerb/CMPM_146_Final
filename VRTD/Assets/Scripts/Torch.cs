using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public Light lightSource;
    float health;
    float damagePerHit = 0.2f;
    float regen_per_tick = 0.0002f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Default Values
        health = 1f;
        lightSource.intensity = 1.0f;

        // Assign Refrences
        //lightSource = GetComponentInChildren<Light>();
    }

    public void TakeDamage()
    {
        health -= damagePerHit;
    }

    // Regenerates health every tick while health is not full
    void Update()
    {
        if (health < 1.0f)
        {
            health += regen_per_tick;
        }
        lightSource.intensity = health;

        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
