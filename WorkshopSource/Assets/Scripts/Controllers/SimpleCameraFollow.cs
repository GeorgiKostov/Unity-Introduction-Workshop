using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;          // The target the camera will follow
    public float smoothSpeed = 0.125f; // How quickly the camera catches up to the target
    public Vector3 offset;            // Offset from the target position

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target set for the camera to follow.");
            return;
        }

        // Desired position is the target's position plus the offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
