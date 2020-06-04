using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    enum FrogState { IDLE, JUMPUP, JUMPDOWN };
    public const float jumpTime = 4f;
    float jumpTimer = jumpTime; // in seconds
    public float jumpForce = 6f;
    Rigidbody2D rb;
    Vector3 localScale;
    Animator anime;
    FrogState currentState = FrogState.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
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


        if(rb.velocity.y > 0.01)
        {
            currentState = FrogState.JUMPUP;
            
        }
        else if(rb.velocity.y < -0.01)
        {
            currentState = FrogState.JUMPDOWN;
        }
        else
        {
            currentState = FrogState.IDLE;
        }

        setAnimator();
    }

    void frogJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }

    void setAnimator()
    {
        if(currentState == FrogState.JUMPUP)
        {
            anime.SetInteger("state", (int)FrogState.JUMPUP);
        }
        else if (currentState == FrogState.JUMPDOWN)
        {
            anime.SetInteger("state", (int)FrogState.JUMPDOWN);
        }
        else
        {
            anime.SetInteger("state", (int)FrogState.IDLE);
        }
    }

}
