using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    public float jumpTime = 4f;
    float jumpTimer = 0f; // in seconds
    public float jumpForce = 6f;
    Rigidbody2D rb;
    Vector3 localScale;
    Animator anime;

    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        jumpTimer = jumpTime;
    }

    // Update is called once per frame
    void Update()
    {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0.0f)
        {
            frogJump();
            jumpTimer = jumpTime;
        }

        anime.SetFloat("jumpVelocity", rb.velocity.y);


    }

    void frogJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }

}
