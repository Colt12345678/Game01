using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -6f); // Slightly closer for better platforming
    public float smoothSpeed = 15f; // Faster smoothing reduces rubber-banding

    private void LateUpdate()
    {
        if (target != null)
        {
            // TransformPoint perfectly calculates the offset based on the player's current rotation
            Vector3 desiredPosition = target.TransformPoint(offset);

            // Smoothly glide to the spot
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Look slightly above the player's feet so we aren't staring at the ground
            Vector3 lookTarget = target.position + new Vector3(0, 1.5f, 0);
            transform.LookAt(lookTarget);
        }
    }
}