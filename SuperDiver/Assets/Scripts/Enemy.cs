using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anime;
    protected Rigidbody2D rb;


    protected virtual void Start()
    {
        anime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    // ReceivedHit runs when the player hits the enemy
    public void receivedHit()
    {
        UnityEngine.Debug.Log("I got hit");
        anime.SetTrigger("Death");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.layer = 10;
    }


    public void death()
    {
        Destroy(this.gameObject);
        
    }
}
