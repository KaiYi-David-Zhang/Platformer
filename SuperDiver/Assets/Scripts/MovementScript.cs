using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed = 8f;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isFacingRight = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Movement Script has been loaded");
    }

    // Update is called every frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isFacingRight = true;
            spriteRenderer.flipX = !isFacingRight;
            transform.position += new Vector3(speed * Time.deltaTime, 0.0f);
            animator.SetBool("isRunning", true);
        } 
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingRight = false;
            spriteRenderer.flipX = !isFacingRight;
            transform.position += new Vector3(-speed * Time.deltaTime, 0.0f);
            animator.SetBool("isRunning", true);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            animator.SetBool("isRunning", false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            animator.SetBool("isRunning", false);
        }
    }
}
