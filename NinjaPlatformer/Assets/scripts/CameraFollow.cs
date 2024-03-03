using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target to follow
    [SerializeField] Transform target;

    // Offset between camera and target
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        // Calculate the initial offset between the camera and the target
        if (target != null)
        {
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogWarning("CameraFollow script is missing a target!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            // Set the camera's position to the target position plus the offset
            transform.position = target.position + offset;
        }
    }
}
