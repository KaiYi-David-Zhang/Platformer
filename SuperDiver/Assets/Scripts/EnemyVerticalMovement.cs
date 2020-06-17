using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVerticalMovement : Enemy
{
    public float jumpTime = 4f;
    float jumpTimer = 0f; // in seconds
    public float jumpForce = 6f;
    Vector3 localScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        localScale = transform.localScale;
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

    protected override void modifyConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    public override void respawn()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.layer = 9;
        gameObject.SetActive(true);
        rb.position = originalLoc;
        rb.velocity *= 0;
        anime.SetTrigger("resetAnimation");
        modifyConstraints();
    }
}
