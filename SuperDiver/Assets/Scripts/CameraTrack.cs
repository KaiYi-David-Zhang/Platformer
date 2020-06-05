using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public Transform trackedObject;
    public float updateSpeed = 3f;
    public Vector2 trackingOffset;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = (Vector3)trackingOffset;
        offset.z = transform.position.z - trackedObject.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, trackedObject.position + offset,
                updateSpeed * Time.deltaTime);
    }
}
