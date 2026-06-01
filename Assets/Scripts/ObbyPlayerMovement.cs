using UnityEngine;

public class ObbyPlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private int jumpCount = 0;

    [Header("Movement Settings")]
    public float playerSpeed = 7.0f;
    public float gravityValue = -25.0f;
    public float jumpHeight = 2.5f;
    public int maxJumps = 2;

    [Header("Camera Reference")]
    public Transform cam; 

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
            jumpCount = 0; 
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate directions based on where the camera is looking
        if (cam != null)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = (camForward * moveZ + camRight * moveX).normalized;
            controller.Move(move * Time.deltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }
        }

        // Jump & Double Jump
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumps))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            jumpCount++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}