using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalMovement : Enemy
{
    public float leftDis, rightDis;
    public float moveSpeed = 3f;
    float startingPos;
    Vector3 localScale;
    SpriteRenderer spriteRenderer;
    bool moveL = true;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        localScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftDis != 0 || rightDis != 0)
        {

            if (transform.position.x > startingPos + rightDis)
            {
                moveL = true;
            }
            if (transform.position.x < startingPos - leftDis)
            {
                moveL = false;
            }

            if (moveL)
            {
                moveLeft();
            }
            else
            {
                moveRight();
            }
        }

    }

    // normall the local should be 1 for facing right but the image is facing left to start with
    void moveRight() {
        moveL = false;
        localScale.x = -1;  // since the sprite is facing left initially

        transform.localScale = localScale;
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

    }


    void moveLeft() {
        moveL = true;
        localScale.x = 1;
        transform.localScale = localScale;
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);

    }


    protected override void modifyConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public override void respawn()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.layer = 9;
        gameObject.SetActive(true);
        rb.position = originalLoc;
        rb.velocity *= 0;
        localScale.x = 1;
        transform.localScale = localScale;
        moveL = true;
        modifyConstraints();
    }
}
