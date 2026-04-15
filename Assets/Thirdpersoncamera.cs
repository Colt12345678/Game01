using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    
    [Tooltip("Where the camera hovers relative to the world, NOT the player's back.")]
    public Vector3 offset = new Vector3(0f, 5f, -8f); 
    
    public float smoothSpeed = 10f;

    private void LateUpdate()
    {
        if (target != null)
        {
            // MAGIC FIX: We just use world position + offset. No rotation math!
            // The camera will no longer care which way your character spins.
            Vector3 desiredPosition = target.position + offset;

            // Smoothly glide to the position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Always look slightly above the player's feet
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }
}