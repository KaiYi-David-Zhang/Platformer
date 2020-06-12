using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * respawnEnemies:
     *      reset the enemies when the player dies
     */
    public void respawnEnemies()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Enemy enemy = transform.GetChild(i).gameObject.GetComponent<Enemy> ();
            enemy.respawn();
        }
        
    }
}
