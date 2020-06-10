using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationTrigger : MonoBehaviour
{
    Animator anime;

    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            anime.SetTrigger("beatDown");
        }
    }
}
