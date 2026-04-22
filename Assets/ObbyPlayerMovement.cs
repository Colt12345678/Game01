using UnityEngine;
using TMPro; // We need this line to talk to your TextMeshPro UI!

public class ObbyPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform cam;

    [Header("UI Slots")]
    public GameObject winScreen;      // Slot for your "YOU WIN!" text
    public TextMeshProUGUI scoreText; // Slot for your "Score: 0" text

    [Header("Movement Stats")]
    public float speed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector3 velocity;
    private bool isGrounded;
    
    // Checkpoint & Game Memory
    private Vector3 respawnPoint;
    private int score = 0; // Your starting score

    private void Start()
    {
        // Hide the mouse cursor so it doesn't get in the way
        Cursor.lockState = CursorLockMode.Locked;
        
        // Save the very first spot you spawn as your first checkpoint
        respawnPoint = transform.position; 

        // Make sure the win screen is hidden when the game starts
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }

        // Set the score text to 0 when the game starts
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void Update()
    {
        // 1. Check if we are touching the ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Get WASD / Arrow Key Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 3. Move the player relative to where the camera is looking
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

    // --------------------------------------------------------
    // TRIGGER COLLISIONS (Checkpoints, Death Zones, Finish Line, Coins)
    // --------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // CHECKPOINT
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position + new Vector3(0, 1.5f, 0);
            Debug.Log("CHECKPOINT SAVED!"); 
        }
        
        // INVISIBLE DEATH ZONE
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("DEATH ZONE HIT! Teleporting...");
            controller.enabled = false; 
            transform.position = respawnPoint; 
            controller.enabled = true; 
        }

        // THE FINISH LINE
        if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("🏆 YOU WIN! COURSE COMPLETED! 🏆");
            controller.enabled = false; // Freeze the player
            
            if (winScreen != null)
            {
                winScreen.SetActive(true); // Turn on the victory text
            }
        }

        // COLLECTIBLE COINS
        if (other.gameObject.CompareTag("Coin"))
        {
            score += 1; // Add 1 point
            
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score; // Update the UI
            }

            Destroy(other.gameObject); // Make the coin disappear
            Debug.Log("Coin Collected!");
        }
    }

    // --------------------------------------------------------
    // PHYSICAL COLLISIONS (Bounce Pads and Lava Blocks)
    // --------------------------------------------------------
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // BOUNCE PAD
        if (hit.gameObject.CompareTag("BouncePad"))
        {
            velocity.y = Mathf.Sqrt(15f * -2f * gravity);
            Debug.Log("BOING!");
        }

        // DEADLY LAVA BLOCK
        if (hit.gameObject.CompareTag("Lava"))
        {
            Debug.Log("OUCH! Hit Lava! Teleporting...");
            controller.enabled = false; 
            transform.position = respawnPoint; 
            controller.enabled = true; 
        }
    }
}