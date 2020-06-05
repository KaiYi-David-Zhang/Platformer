﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpossumMovement : MonoBehaviour
{
    public float leftDis, rightDis;
    public float moveSpeed = 3f;
    float startingPos;
    Vector3 localScale;
    bool moveL = true;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D> ();
        startingPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > startingPos + rightDis)
        {
            moveL = true;
        }
        if(transform.position.x < startingPos - leftDis) {
            moveL = false;
        }

        if (moveL) {
            moveLeft();
        }
        else {
            moveRight();
        }

    }

    // normall the local should be 1 for facing right but the image is facing left to start with
    void moveRight() {
        moveL = false;
        localScale.x = 1;
        rb.velocity = new Vector2(localScale.x * moveSpeed, rb.velocity.y);
        localScale.x = -1;  // here is the change since the picture is facing left
        transform.localScale = localScale;

    }


    void moveLeft() {
        moveL = true;
        localScale.x = -1;
        rb.velocity = new Vector2(localScale.x * moveSpeed, rb.velocity.y);
        localScale.x = 1;
        transform.localScale = localScale;
    }
}
