/* 
 * Player Controls Script
 * 
 * This script handles the player character interaction based on user input
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    /*
     * JumpState:
     *      enum class to denote character's aerial state:
     *          -IDLE: the character is on the ground
     *          -JUMPUP: the character is jumping upwards while on ground
     *          -INFLIGHT: character is in the air in an upwards motion
     *                     prevents character from getting stuck on JUMPUP
     *                     animation when the velocity changes quickly to 0
     *                     from a positive value
     *          -JUMPDOWN: character is in a downwards motion while in the air
     */
    public enum JumpState { IDLE, JUMPUP, INFLIGHT, JUMPDOWN };

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
    public float blinkCycles = 0.1f;

    public JumpState jumpState = JumpState.IDLE;
    public int maxHealth = 1;
    public float hitStunTime = 0.5f;
    public float iframesTime = 1.0f;

    public Vector2 velocity;          // current velocity of the player
    
    public GameObject spawnPoint;
    public Cinemachine.CinemachineVirtualCamera vcam;
    public Collider2D collider2D;
    public Text lifeNum;

    // private variables
    Vector3 localScale; // for changing direction
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isFacingRight = true;
    bool isGrounded = false;
    bool isAlive = true;
    bool controlEnabled = true; // determines if the player have controls at all
                                // used during death and respawns
    //bool isInControl = true;  // determines if the player can have left and right movement
                                //used during interaction with enemies

    int currHealth;

    float moveSpeed;
    float blinkTimer;



    // Unity engine basic functions
    // Awake is called when the script is loaded, regardless of it being enabled or not
    void Awake()
    {
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Movement Script has been loaded");
        currHealth = maxHealth;
        localScale = transform.localScale;
        blinkTimer = blinkCycles;
    }

    // Update is called every frame
    void Update()
    {
        // controls
        if (controlEnabled)
        {
            computeLRMovement();
            computeJump();
        }


        // player state updates
        if (gameObject.layer == UNHITABLE_LAYER && isAlive)
        {
            spriteBlink();
        }


        // analytics and testing
        velocity = rb.velocity; // displays current velocity of player on unity
        //testTeleport();
    }

    // unity function that runs everytime a collision happends
    // used for collsion check for enemy
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (jumpState == JumpState.JUMPDOWN && transform.position.y > col.gameObject.transform.position.y)
            {
                // player is above the enemy

                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                enemy.receivedHit();
                bounce(bounceVelocity);
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

        lifeNum.text = currHealth.ToString();   // update health to player
    }



    // helper functions

    /* 
     * takeControl:
     *      strip player of any control over their character
     */
    public void takeControl()
    {
        controlEnabled = false;
    }

    /*
     * giveControl:
     *      player regains control over their character.
     *      Implemented so that it can be "invoked"
     */
    public void giveControl()
    {
        controlEnabled = true;
    }

    /*
     * makeHitable:
     *      player's layer is set back to player layer, and becomes hitable again
     */
    public void makeHitable()
    {
        gameObject.layer = PLAYER_LAYER;
        spriteRenderer.enabled = true;
    }

    /*
     * makeUnhitable:
     *      player's layer is set to unHitable
     */
    public void makeUnhitable()
    {
        gameObject.layer = UNHITABLE_LAYER;
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

        if (Input.GetAxis("Horizontal") == 0)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetBool("isRunning", false); // reset running animation if there is no LR input
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
        if (verticalVel > jumpForce - 1.5f)
        {
            return JumpState.JUMPUP;
        } 
        else if (verticalVel > 0.055f)
        {
            return JumpState.INFLIGHT;
        }
        else if (verticalVel < -0.055f)
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

    /*
     * bounce:
     *      player bounces after stepping on the enemy
     */
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
            playerDeath();
        }
    }

    /* 
     * die:
     *      decrements the player's health until it reaches 0
     *      then procs playerDeath method
     */
    public void die()
    {
        currHealth = 0;
        playerDeath();
    }

    /*
     * playerDeath:
     *      called after a player's health reaches 0; player's behaviour
     *      after death and before respawn.
     */
    void playerDeath()
    {
        controlEnabled = false;
        isAlive = false;

        rb.velocity *= 0;                     // stop motion
        rb.gravityScale = 0f;                 // stop motion

        animator.SetBool("isHurt", false);    // stop hurt animation
        animator.SetTrigger("isDead");        // start dead animation

        vcam.m_LookAt = null;                 // detach camera
        vcam.m_Follow = null;                 // detach camera

        Invoke("respawn", 0.75f);
    }

    /*
     * respawn:
     *      resets the character to spawn and teleports him back to spawn point
     */
    void respawn()
    {
        currHealth = maxHealth;                // reset health

        rb.gravityScale = 2f;
        animator.SetBool("isHurt", false);     // reset the animation
        animator.SetBool("isRunning", false);  // reset the animation

        var enemies = GameObject.Find("Enemies");
        if (enemies != null)
        {
            var allEnemies = enemies.GetComponent<Enemies>();
            allEnemies.respawnEnemies();
        }

        teleport(spawnPoint.transform.position);
        
        vcam.m_LookAt = transform;             // attach camera
        vcam.m_Follow = transform;             // attach camera

        // enable control after a small fraction of a second to disable jumping midair when respawning
        isAlive = true;
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
        animator.SetInteger("jumpState", 0); // resets animation
        jumpState = JumpState.IDLE;          // reset jump state
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
        isGrounded = false;

        rb.velocity = new Vector2(0, rb.velocity.y);    // resets velocity
    }

    /*
     * spriteBlink:
     *      blink the sprite while the player is unhittable
     */
    void spriteBlink()
    {
        bool spriteCurrentlyEnabled = spriteRenderer.enabled;
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            blinkTimer = blinkCycles;
            spriteRenderer.enabled = !spriteCurrentlyEnabled;
        }
    }
}
