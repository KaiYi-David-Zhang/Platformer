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
        if (player != null)
        {
            UnityEngine.Debug.Log("Player entered Death Zone");
            player.die();
        }
    }
}
