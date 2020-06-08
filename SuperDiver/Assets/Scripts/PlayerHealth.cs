using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This script is left alive just in case we need to separate movement and health
 * This script is not called anywhere
 */

public class PlayerHealth : MonoBehaviour
{
    //public variables
    public int maxHealth = 1;

    // private variables
    bool isAlive = true;
    int healthRemaining;

    void decrementHealth()
    {
        healthRemaining--;
        if (healthRemaining == 0)
        {
            playerDeath();
        }
    }

    void playerDeath()
    {
        respawn();
    }

    void respawn()
    {
        healthRemaining = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthRemaining = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
