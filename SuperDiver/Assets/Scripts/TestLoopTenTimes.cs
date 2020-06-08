using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoopTenTimes : MonoBehaviour
{
    public int animLooper = 60;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("animLoop", animLooper);
    }

    void decrementAnimLooper()
    {
        animLooper--;
    }
}
