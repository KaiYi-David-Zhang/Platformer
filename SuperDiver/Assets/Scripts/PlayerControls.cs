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

    // useful constants
    const int PLAYER_LAYER = 8;
    const int UNHITABLE_LAYER = 10;
    const int LOCALSCALE_RIGHT = 1;
    const int LOCALSCALE_LEFT = -1;

    // public variables
    public float groundSpeed = 6f;
    public float glideSpeed = 6f;
    public float jumpForce = 12f;
    public float hurtVelocity = 4f;
    public float bounceVelocity = 6f;

    public JumpState jumpState = JumpState.IDLE;
    public int maxHealth = 1;
    public float hitStunTime = 0.5f;
    public float iframesTime = 1.0f;

    public Vector2 velocity;          // current velocity of the player
    
    public GameObject spawnPoint;
    public Cinemachine.CinemachineVirtualCamera vcam;
    public Collider2D collider2D;
    

    // private variables
    Vector3 localScale; // for changing direction
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isFacingRight = true;
    bool isGrounded = false;
    //bool isInControl = true;    // determinds if the player can have left and right movement used during interaction with enemies
    float moveSpeed;
    bool isAlive = true;
    bool controlEnabled = true; // determinds if the player have controls at all used during death and respawns
    int currHealth;



    // Unity engine basic functions
    // Awake is called when the script is loaded, regardless of it being enabled or not
    void Awake()
    {
        currHealth = maxHealth;
        localScale = transform.localScale;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
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
        //testTeleport();

        // check if player is alive
        if (!isAlive)
        {
            playerDeath();
        }
    }

    // unity function that runs everytime a collision happends
    // used for collsion check for enemy
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            // When player jumps on the enemy

            if (jumpState == JumpState.JUMPDOWN && transform.position.y > col.gameObject.transform.position.y)
            {
                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                enemy.ReceivedHit();
                bounce(bounceVelocity);
                // TODO: Add jump here
            }
            else
            {
                // When an Enemy hits the player
                //UnityEngine.Debug.Log("Enemy collided with player");

                playerHurt(col);
                if (isAlive)
                {
                    Invoke("exitHurt", hitStunTime);
                    Invoke("makeHitable", iframesTime);
                }
            }
        }
    }



    // helper functions
    /*
     * giveControl:
     *      player regains control over their character.
     *      Implemented so that it can be "invoked"
     */
    void giveControl()
    {
        controlEnabled = true;
    }

    /*
     * makeHitable:
     *      player's layer is set back to player layer, and becomes hitable again
     */
    void makeHitable()
    {
        gameObject.layer = PLAYER_LAYER;
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
            localScale.x = LOCALSCALE_RIGHT;
            transform.localScale = localScale;
            //spriteRenderer.flipX = !isFacingRight;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            animator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingRight = false;
            localScale.x = LOCALSCALE_LEFT;
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
            // stop jump if spacebar is lifted during upwards motion

            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        jumpState = getCurrentJumpState();

        animator.SetInteger("jumpState", jumpState - JumpState.IDLE);
        if (jumpState == JumpState.IDLE)
        {
            //if (!controlEnabled)
            //{
            //    controlEnabled = true; // player regains control when landed
            //    makeHitable();   // change back to the player layer so it can be hit again
            //    rb.velocity = new Vector2(0, rb.velocity.y);    // resets velocity
            //    animator.SetBool("isRunning", false);   // resets animation
            //    animator.SetBool("isHurt", false);
            //}

            isGrounded = true;
        } 
        else
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

    void bounce(float bounceVelocity)
    {
        rb.velocity = new Vector2(rb.velocity.x, bounceVelocity);
    }


    
    // health related methods


    /*
     * decrementHealth:
     *      decrements the player health by 1
     */
    public void decrementHealth()
    {
        currHealth--;
        if (currHealth == 0)
        {
            isAlive = false;
        }
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
        isAlive = true;
        Invoke("respawn", 0.75f);
    }

    /*
     * respawn:
     *      resets the character to spawn and teleports him back to spawn point
     */
    void respawn()
    {
        currHealth = maxHealth;                // reset health

        animator.SetBool("isHurt", false);     // reset the animation
        animator.SetBool("isRunning", false);  // reset the animation

        teleport(spawnPoint.transform.position);

        vcam.m_LookAt = transform;
        vcam.m_Follow = transform;

        // enable control after a small fraction of a second to disable jumping midair when respawning
        Invoke("giveControl", 0.025f);
        makeHitable();                         // reset to player layer
    }

    /*
     * playerHurt:
     *      enter player hurt animation, disable control for a set time, and
     *      make unhitable for set time
     */
    void playerHurt(Collision2D enemy)
    {
        if (gameObject.transform.position.x < enemy.gameObject.transform.position.x)
        {
            // player is to the left of enemy   
            rb.velocity = new Vector2(-hurtVelocity, hurtVelocity);
        }
        else
        {
            // player is to the right of enemy
            rb.velocity = new Vector2(hurtVelocity, hurtVelocity);
        }

        animator.SetBool("isHurt", true);
        animator.SetInteger("jumpState", 0);
        controlEnabled = false;
        gameObject.layer = UNHITABLE_LAYER;  // change to unHitable layer to avoid repeated damage

        decrementHealth();
    }

    /*
     * exitHurt:
     *      exit out of the hurt state:
     *          reset the animator
     *          reset the velocity
     *          regain control
     */
    void exitHurt()
    {
        controlEnabled = true; 
        animator.SetBool("isHurt", false);              // resets animation
        animator.SetBool("isRunning", false);           // resets animation
        
        rb.velocity = new Vector2(0, rb.velocity.y);    // resets velocity
    }
}
