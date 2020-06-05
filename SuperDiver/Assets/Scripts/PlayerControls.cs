﻿/* 
 * Player Controls Script
 * 
 * This script handles the player character interaction based on user input and
 * unit collision.
 */

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public enum JumpState { IDLE, JUMPUP, JUMPDOWN };

    public float moveSpeed = 10f;
    public float jumpForce = 6f;
    public Vector2 velocity;          // current velocity of the player
    public JumpState jumpState = JumpState.IDLE;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isFacingRight = true;


    void Awake()
    {
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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isFacingRight = true;
            spriteRenderer.flipX = !isFacingRight;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            animator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingRight = false;
            spriteRenderer.flipX = !isFacingRight;
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
        if (Input.GetButtonDown("Jump") && jumpState == JumpState.IDLE)
        {
            if (jumpState == JumpState.IDLE)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpState = JumpState.JUMPUP;
                animator.SetInteger("jumpState", 1);
            }
        }

        if (Input.GetButtonUp("Jump") && jumpState == JumpState.JUMPUP)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        if (rb.velocity.y < 0.001f && jumpState == JumpState.JUMPUP)
        {
            jumpState = JumpState.JUMPDOWN;
            animator.SetInteger("jumpState", 2);
        } 
        else if (rb.velocity.y > -0.001f && Mathf.Abs(rb.velocity.y) > 0 && jumpState == JumpState.JUMPDOWN)
        {
            Debug.Log("player has landed: " + rb.velocity.y);
            jumpState = JumpState.IDLE;
            animator.SetInteger("jumpState", 0);
        }
    }
}
