using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleMovement : MonoBehaviour
{
    public float leftDis, RightDis;
    float startingPos;
    public float moveSpeed = 3f;
    Vector3 localScale;
    bool moveL = true;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        startingPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > startingPos + RightDis)
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

    void moveRight()
    {
        moveL = false;
        localScale.x = 1;
        rb.velocity = new Vector2(localScale.x * moveSpeed, 0);
        localScale.x = -1;  // here is the change since the picture is facing left
        transform.localScale = localScale;

    }


    void moveLeft()
    {
        moveL = true;
        localScale.x = -1;
        rb.velocity = new Vector2(localScale.x * moveSpeed, 0);
        localScale.x = 1;
        transform.localScale = localScale;
    }

}
