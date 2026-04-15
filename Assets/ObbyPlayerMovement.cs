using UnityEngine;

public class ObbyPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform cam;

    [Header("Movement Stats")]
    public float speed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector3 velocity;
    private bool isGrounded;
    
    // Checkpoint memory
    private Vector3 respawnPoint;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Save the starting position as the first checkpoint
        respawnPoint = transform.position; 
    }

    private void Update()
    {
        // 1. Gravity and Ground Check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Get WASD Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 3. Move Relative to the Camera
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // 4. Normal Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // TRIGGER COLLISIONS (For Checkpoints and the Death Zone)
    private void OnTriggerEnter(Collider other)
    {
        // The lie detector code
        Debug.Log("I just touched a trigger with the tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position + new Vector3(0, 1.5f, 0);
            Debug.Log("CHECKPOINT SAVED!"); 
        }
        
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("DEATH ZONE HIT! Teleporting...");
            controller.enabled = false; 
            transform.position = respawnPoint; 
            controller.enabled = true; 
        }
    }

    // PHYSICAL COLLISIONS (For the Bounce Pad)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If our feet physically bump into a BouncePad...
        if (hit.gameObject.CompareTag("BouncePad"))
        {
            // Launch the player! (Change 15f to jump higher or lower)
            velocity.y = Mathf.Sqrt(15f * -2f * gravity);
            
            Debug.Log("BOING!");
        }
    }
}