using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Target")]
    public Transform target;           

    [Header("Position Settings (Zoomed In)")]
    public Vector3 offset = new Vector3(0f, 2f, -3.5f); 

    [Header("Mouse Control")]
    public float mouseSensitivity = 1.5f;
    public float minVerticalAngle = -20f; 
    public float maxVerticalAngle = 60f;  

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentX = transform.eulerAngles.y;
        currentY = transform.eulerAngles.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Automatically adapts whether you use old or new Unity input packages
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentX += mouseX * mouseSensitivity;
        currentY -= mouseY * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position + rotation * offset;

        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * (offset.y * 0.5f));
    }
}