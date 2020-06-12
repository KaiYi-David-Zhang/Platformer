using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    /* 
     * When the player enters this deathzone, this will trigger the
     * death and respawn methods from PlayerControls
     */
    void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.gameObject.GetComponent<PlayerControls>();
        var enemy = collider.gameObject.GetComponent<Enemy>();
        if (player != null)
        {
            UnityEngine.Debug.Log("Player entered Death Zone");
            player.die();
        }
        if(enemy != null)
        {
            UnityEngine.Debug.Log("An enemy entered Death Zone");
            enemy.death();
        }
    }
}
