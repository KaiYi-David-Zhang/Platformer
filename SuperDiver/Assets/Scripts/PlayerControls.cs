﻿/* 
 * Player Controls Script
 * 
 * This script handles the player character interaction based on user input and
 * unit collision.
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public enum JumpState { IDLE, JUMPUP, JUMPDOWN };

    public float groundSpeed = 10f;
    public float glideSpeed = 6f;
    public float jumpForce = 10f;
    public Vector2 velocity;          // current velocity of the player
    public JumpState jumpState = JumpState.IDLE;

    Vector3 localScale; // for changing direction

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isFacingRight = true;
    bool isGrounded = false;
    bool isInControl = true;    // determinds if the player can have left and right movement 
    float moveSpeed;

    void Awake()
    {
        localScale = transform.localScale;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Movement Script has been loaded");
    }

    // Update is called every frame
    void Update()
    {
        computeLRMovement();
        computeJump();
        velocity = rb.velocity; // displays current velocity of player on unity
    }


    /* 
     * computeLRMovement:
     *      takes user key input and computes left and right movement
     */
    void computeLRMovement()
    {
        if (isGrounded)
        {
            moveSpeed = groundSpeed;
        } else
        {
            moveSpeed = glideSpeed;
        }

        if (!isInControl)
        {
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isFacingRight = true;
            localScale.x = 1;
            transform.localScale = localScale;
            //spriteRenderer.flipX = !isFacingRight;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            animator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingRight = false;
            localScale.x = -1;
            transform.localScale = localScale;
            //spriteRenderer.flipX = !isFacingRight;
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            animator.SetBool("isRunning", true);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetBool("isRunning", false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetBool("isRunning", false);
        }
    }


    /*
     * computeJump:
     *      takes user key input and computes jump movement
     */
    void computeJump()
    {
        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && jumpState == JumpState.JUMPUP)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        jumpState = getCurrentJumpState();

        animator.SetInteger("jumpState", jumpState - JumpState.IDLE);
        if (jumpState == JumpState.IDLE)
        {
            if (!isInControl)
            {
                isInControl = true; // player regains control when landed
                gameObject.layer = 8;   // change back to the player layer so it can be hit again
                rb.velocity = new Vector2(0, rb.velocity.y);    // resets velocity
                animator.SetBool("isRunning", false);   // resets animation

            }

            isGrounded = true;

        } else
        {
            isGrounded = false;
        }
    }


    /* 
     * getCurrentJumpState:
     *      returns the current jump state based on vertical velocity
     */
    JumpState getCurrentJumpState()
    {
        float verticalVel = rb.velocity.y;
        if (verticalVel > 0.001f)
        {
            return JumpState.JUMPUP;
        }
        else if (rb.velocity.y < -0.001f)
        {
            return JumpState.JUMPDOWN;
        }
        else {
            return JumpState.IDLE;
        }
    }


    // unity function that runs everytime a collision happends
    // used for collsion check for enemy
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy" && isInControl == true)
        {
            rb.velocity = new Vector2(-20, 20);
            isInControl = false;
            gameObject.layer = 10;  // change to unHitable layer to avoid repeated damage

        }
    }
}
