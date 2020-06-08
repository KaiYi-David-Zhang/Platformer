using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This script is left alive solely for the purpose of referencing.
 * In the "hurt" folder, a test animation has been implemented to adjust the animation
 * when the player takes damage.
 * Repeating 5 times with the animation rate set there seems pretty good.
 * 
 * See the rest of the script to see how animation can be repeated.
 * Further note that an event calling "decrementAnimLooper" must be attached
 * to the animation.
 */
public class TestLoopTenTimes : MonoBehaviour
{
    public int animLooper = 5;

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
