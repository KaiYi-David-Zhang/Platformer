using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlinking : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float blinkCycles = 0.1f;
    float blinkTimer = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            blinkTimer = blinkCycles;
        }
    }
}
