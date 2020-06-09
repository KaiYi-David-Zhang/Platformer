using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anime;

    protected virtual void Start()
    {
        anime = GetComponent<Animator>();
    }

    // ReceivedHit runs when the player hits the enemy
    public void ReceivedHit()
    {
        UnityEngine.Debug.Log("I got hit");
        anime.SetTrigger("Death");

        Death();
    }


    public void Death()
    {
        Destroy(this.gameObject);
    }
}
