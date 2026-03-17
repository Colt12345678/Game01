using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables - Target Settings
    [Header("Targeting")]
    [Tooltip("Drag the Player object here.")]
    [SerializeField] private Transform target; 
    #endregion

    #region Variables - Offset Settings
    [Header("Positioning")]
    [Tooltip("The fixed distance and height from the player.")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);
    #endregion

    #region Variables - Movement Settings
    [Header("Smoothing")]
    [Tooltip("How smoothly the camera follows. Lower is faster.")]
    [SerializeField] private float smoothTime = 0.2f;

    [Tooltip("The fixed rotation of the camera. It will NOT change during gameplay.")]
    [SerializeField] private Vector3 fixedRotation = new Vector3(30, 0, 0);

    private Vector3 _currentVelocity = Vector3.zero;
    #endregion

    private void LateUpdate()
    {
        if (target == null) return;

        HandlePosition();
        HandleRotation();
    }

    private void HandlePosition()
    {
        // Calculate where the camera should be based on the player's position + the offset
        Vector3 targetPosition = target.position + offset;

        // Smoothly move to that position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }

    private void HandleRotation()
    {
        // Force the camera to stay at this exact angle forever
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}