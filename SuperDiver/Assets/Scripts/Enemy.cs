using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 originalLoc;
    protected Animator anime;
    protected Rigidbody2D rb;


    protected virtual void Start()
    {
        anime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalLoc = rb.position;
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
        this.gameObject.SetActive(false);
    }

    public void respawn()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.layer = 9;
        gameObject.SetActive(true);
        rb.position = originalLoc;
        rb.velocity *= 0;
        modifyConstraints();
    }

    protected virtual void modifyConstraints()
    {

    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            rb.velocity *= 0;
        }
    }
}
