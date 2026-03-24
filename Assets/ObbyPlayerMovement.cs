using UnityEngine;

public class ObbyPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Drag your Character Controller here!")]
    public CharacterController controller;

    [Header("Movement Stats")]
    public float speed = 8f;
    public float gravity = -20f; // High gravity makes jumping feel snappy and responsive!
    public float jumpHeight = 3f;

    [Header("Mouse Camera Controls")]
    public float mouseSensitivity = 300f;

    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        // This hides your mouse cursor and locks it to the center of the screen
        // (Press ESC on your keyboard while playing to get your mouse back!)
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 1. Check if we are touching the floor
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keeps the player firmly snapped to the ground
        }

        // 2. Rotate the Player using the Mouse (Left and Right)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 3. Move Forward, Backward, and Strafe using WASD
        float x = Input.GetAxis("Horizontal"); // A & D keys
        float z = Input.GetAxis("Vertical");   // W & S keys

        // This math makes sure we always move relative to the direction we are facing
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 4. Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Apply Gravity so we fall back down
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}