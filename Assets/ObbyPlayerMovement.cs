using UnityEngine;
using TMPro;

public class ObbyPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform cam;

    [Header("UI Slots")]
    public GameObject winScreen;      
    public TextMeshProUGUI scoreText; 

    [Header("Effects")]
    public GameObject coinParticlePrefab; 
    public GameObject trailParticlePrefab; 
    private ParticleSystem currentTrail;   

    [Header("Movement Stats")]
    public float speed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 6f; 
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Double Jump Settings")]
    public int maxJumps = 2;       
    private int jumpCountRemaining; 

    private Vector3 velocity;
    private bool isGrounded;
    
    private Vector3 respawnPoint;
    private int score = 0; 

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        respawnPoint = transform.position; 

        if (winScreen != null) winScreen.SetActive(false);
        if (scoreText != null) scoreText.text = "Score: " + score;
        
        jumpCountRemaining = maxJumps; 
    }

    private void Update()
    {
        if (controller == null) return;

        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f; 
            jumpCountRemaining = maxJumps; 
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Double Jump Input Configuration
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || jumpCountRemaining > 0) 
            {
                if (!isGrounded)
                {
                    jumpCountRemaining--; 
                }
                else
                {
                    jumpCountRemaining = maxJumps - 1; 
                }

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position + new Vector3(0, 1.5f, 0);
            Debug.Log("CHECKPOINT SAVED!"); 
        }
        
        if (other.gameObject.CompareTag("DeathZone"))
        {
            StopTrail(); 
            controller.enabled = false; 
            transform.position = respawnPoint; 
            controller.enabled = true; 
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            StopTrail();
            controller.enabled = false; 
            if (winScreen != null) winScreen.SetActive(true);
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            score += 1; 
            if (scoreText != null) scoreText.text = "Score: " + score;

            if (coinParticlePrefab != null)
            {
                Instantiate(coinParticlePrefab, other.transform.position, Quaternion.identity);
            }

            Destroy(other.gameObject); 
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (controller == null) return;

        if (hit.gameObject.CompareTag("BouncePad"))
        {
            velocity.y = Mathf.Sqrt(15f * -2f * gravity);
            jumpCountRemaining = maxJumps - 1; 
        }

        if (hit.gameObject.CompareTag("Lava"))
        {
            StopTrail();
            controller.enabled = false; 
            transform.position = respawnPoint; 
            controller.enabled = true; 
        }

        if (hit.gameObject.CompareTag("EffectBlock"))
        {
            StartTrail();
        }
        else
        {
            StopTrail();
        }
    }

    private void StartTrail()
    {
        if (currentTrail == null && trailParticlePrefab != null)
        {
            GameObject trailObj = Instantiate(trailParticlePrefab, transform.position, Quaternion.identity, transform);
            currentTrail = trailObj.GetComponent<ParticleSystem>();
        }
    }

    private void StopTrail()
    {
        if (currentTrail != null)
        {
            currentTrail.transform.parent = null; 
            currentTrail.Stop();                  
            Destroy(currentTrail.gameObject, 2f); 
            currentTrail = null;
        }
    }
}