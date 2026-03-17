using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables: Movement Settings
    [Header("Speeds")]
    [SerializeField, Tooltip("Base walking speed")] 
    private float walkSpeed = 5f;
    
    [SerializeField, Tooltip("Speed when walking faster (Alt/Shift)")] 
    private float fastWalkSpeed = 8f;
    
    [SerializeField, Tooltip("Maximum sprinting speed")] 
    private float sprintSpeed = 12f;
    
    [SerializeField, Tooltip("Speed while crouching")] 
    private float crouchSpeed = 2.5f;

    [Header("Jump Settings")]
    [SerializeField, Tooltip("How high the player jumps")] 
    private float jumpForce = 6f;
    
    [SerializeField, Tooltip("Strength of gravity (Try -19.62)")] 
    private float gravity = -19.62f;
    
    [SerializeField, Tooltip("Time window to jump after leaving a ledge")] 
    private float coyoteTime = 0.2f;
    #endregion

    #region Variables: Internal State
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteCounter;
    private float currentSpeed;
    
    // Input States
    private bool isSprinting;
    private bool isCrouching;
    private bool isFastWalking;
    #endregion

    private void Awake()
    {
        // Setup the controller component
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGround();
        HandleMovement();
        HandleGravity();
    }

    #region Movement Logic
    private void CheckGround()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDoubleJump = true;
            if (velocity.y < 0) velocity.y = -2f;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        // Logic to choose which speed to use
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isSprinting) currentSpeed = sprintSpeed;
        else if (isFastWalking) currentSpeed = fastWalkSpeed;
        else currentSpeed = walkSpeed;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Face the direction of travel
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
    }

    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Input System Handlers
    // These must match your Input Action names
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded || coyoteCounter > 0)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                coyoteCounter = 0;
            }
            else if (canDoubleJump)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                canDoubleJump = false;
            }
        }
    }

    public void OnSprint(InputValue value) => isSprinting = value.isPressed;
    public void OnCrouch(InputValue value) => isCrouching = value.isPressed;
    public void OnFastWalk(InputValue value) => isFastWalking = value.isPressed;
    #endregion
}

/* ================================================================================
FINAL RE-IMPLEMENTATION STEPS (To avoid errors):
1. SAVE: Make sure you press Ctrl+S in your code editor.
2. CHECK CONSOLE: If there are red errors, the code won't run. Ensure your 
   filename is exactly "PlayerMovement.cs".
3. CENTER Y: In the Inspector, set Center Y to 1 and Height to 2.
4. INPUT SYSTEM: Ensure your Input Actions (Move, Jump, Sprint, Crouch, FastWalk) 
   are mapped to the correct keys in the Input Actions Asset.
5. EVENTS: Drag Player1 into the "Player Input" event slots and select the 
   matching functions (OnMove, OnJump, etc.).
================================================================================
*/