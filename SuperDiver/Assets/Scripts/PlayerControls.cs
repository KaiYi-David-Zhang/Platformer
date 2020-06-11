/* 
 * Player Controls Script
 * 
 * This script handles the player character interaction based on user input
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // enum class for the aerial state
    public enum JumpState { IDLE, JUMPUP, JUMPDOWN };

    // public variables
    public float groundSpeed = 10f;
    public float glideSpeed = 6f;
    public float jumpForce = 10f;
    public Vector2 velocity;          // current velocity of the player
    public JumpState jumpState = JumpState.IDLE;
    public int maxHealth = 1;
    public GameObject spawnPoint;
    public Cinemachine.CinemachineVirtualCamera vcam;

    // private variables
    Vector3 localScale; // for changing direction
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isFacingRight = true;
    bool isGrounded = false;
    float moveSpeed;
    bool isAlive = true;
    bool controlEnabled = true;
    int currHealth;



    // Unity engine basic functions
    void Awake()
    {
        currHealth = maxHealth;
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
        if (controlEnabled)
        {
            computeLRMovement();
            computeJump();
        }
        velocity = rb.velocity; // displays current velocity of player on unity
        testTeleport();
    }



    // movement related methods

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
        if (Input.GetButtonUp("Jump") && (jumpState == JumpState.INFLIGHT 
                                        || jumpState == JumpState.JUMPUP))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        jumpState = getCurrentJumpState();

        animator.SetInteger("jumpState", jumpState - JumpState.IDLE);
        if (jumpState == JumpState.IDLE)
        {
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

    /* 
     * teleport:
     *      teleports the player to a specific position on the map
     */
    void teleport(Vector3 position)
    {
        rb.position = position;
        rb.velocity *= 0;
    }

    void testTeleport()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            teleport(spawnPoint.transform.position);
        }
    }


    
    // health related methods
    /*
     * decrementHealth:
     *      decrements the player health by 1
     */
    public void decrementHealth()
    {
        currHealth--;
    }

    /* 
     * die:
     *      decrements the player's health until it reaches 0
     *      then procs playerDeath method
     */
    public void die()
    {
        controlEnabled = false;
        while (currHealth > 0)
        {
            decrementHealth();
        }
        playerDeath();
    }

    /*
     * playerDeath:
     *      called after a player's health reaches 0; player's behaviour
     *      after death and before respawn.
     */
    void playerDeath()
    {
        vcam.m_LookAt = null;
        vcam.m_Follow = null;
        Invoke("respawn", 0.75f);
    }

    /*
     * respawn:
     *      resets the character to spawn and teleports him back to spawn point
     */
    void respawn()
    {
        currHealth = maxHealth;
        teleport(spawnPoint.transform.position);
        vcam.m_LookAt = transform;
        vcam.m_Follow = transform;
        controlEnabled = true;
    }

}
