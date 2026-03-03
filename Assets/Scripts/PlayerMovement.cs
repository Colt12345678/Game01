using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables: Movement Settings
    [Header("Movement Speeds")]
    [SerializeField, Tooltip("Base walking speed.")]
    private float walkSpeed = 5f;
    
    [SerializeField, Tooltip("Speed when moving faster (e.g., jog).")]
    private float briskWalkSpeed = 8f;
    
    [SerializeField, Tooltip("Top sprinting speed.")]
    private float sprintSpeed = 12f;

    [SerializeField, Tooltip("How quickly the player turns to face movement direction.")]
    private float rotationSpeed = 10f;
    #endregion

    #region Variables: Jump & Physics
    [Header("Jumping & Gravity")]
    [SerializeField, Tooltip("Force of the jump.")]
    private float jumpEventForce = 8f;
    
    [SerializeField, Tooltip("Gravity multiplier to make falling feel less floaty.")]
    private float gravityValue = -19.62f;

    [SerializeField, Tooltip("Time window (seconds) to jump after leaving a ledge.")]
    private float coyoteTime = 0.15f;
    
    [SerializeField, Tooltip("Layer mask for ground detection.")]
    private LayerMask groundLayer;
    #endregion

    #region Private Fields
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private int jumpCount;
    private float coyoteTimeCounter;
    private bool isSprinting;
    private bool isBriskWalking;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        // Reference the CharacterController attached to the GameObject
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGroundCheck();
        HandleMovement();
        ApplyGravity();
    }
    #endregion

    #region Input Methods (Called by Player Input Component)
    public void OnMove(InputValue value)
    {
        // Read vector from WASD or Thumbstick
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            AttemptJump();
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnBriskWalk(InputValue value)
    {
        isBriskWalking = value.isPressed;
    }
    #endregion

    #region Movement Logic
    private void ApplyGroundCheck()
    {
        // Check if the CharacterController is touching the ground
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            // Reset jump state when touching the floor
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
            
            // Stop accumulating downward velocity when grounded
            if (playerVelocity.y < 0)
                playerVelocity.y = -2f; 
        }
        else
        {
            // Count down the coyote time window when in the air
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        // Determine current speed based on input toggles
        float currentSpeed = walkSpeed;
        if (isSprinting) currentSpeed = sprintSpeed;
        else if (isBriskWalking) currentSpeed = briskWalkSpeed;

        // Convert 2D input into 3D movement (X and Z axes)
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        
        // Move the controller
        controller.Move(move * Time.deltaTime * currentSpeed);

        // Rotate the player to face the direction of movement
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void AttemptJump()
    {
        // First jump: Allowed if grounded OR within the Coyote Time window
        if (isGrounded || coyoteTimeCounter > 0f)
        {
            ExecuteJump();
        }
        // Second jump: Allowed if we haven't double-jumped yet
        else if (jumpCount < 2)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        // Apply upward velocity
        playerVelocity.y = Mathf.Sqrt(jumpEventForce * -2f * gravityValue);
        jumpCount++;
        coyoteTimeCounter = 0; // Reset coyote time so we can't spam it
    }

    private void ApplyGravity()
    {
        // Apply gravity over time
        playerVelocity.y += gravityValue * Time.deltaTime;
        // Move the controller based on vertical velocity (Y axis)
        controller.Move(playerVelocity * Time.deltaTime);
    }
    #endregion
}

/*
================================================================================
IMPLEMENTATION STEPS (Follow carefully to avoid errors):
================================================================================
1. SETUP LAYERS & TAGS:
   - Go to 'Edit' > 'Project Settings' > 'Tags and Layers'.
   - Add a Layer named "Player" and a Layer named "Ground".
   - Create a Tag named "Ground".

2. PREPARE THE GROUND:
   - Select your Floor/Ground objects in the Hierarchy.
   - Set their Layer to "Ground" and their Tag to "Ground".

3. PREPARE THE PLAYER:
   - Drag your FBX model into the scene. Name it "Player".
   - Set its Layer to "Player" and Tag to "Player".
   - Add a 'Character Controller' component to the Player.
   - Add the 'Player Input' component (from the Input System package).
   - Add this 'PlayerController' script to the Player.

4. SETUP INPUT ACTIONS:
   - Create a New Input Actions asset (Right-click in Project > Create > Input Actions).
   - Add an Action Map called "Player".
   - Create Actions: 
     * "Move" (Value, Vector2) -> Bind to WASD/Left Stick.
     * "Jump" (Button) -> Bind to Space/South Button.
     * "Sprint" (Button) -> Bind to Left Shift.
     * "BriskWalk" (Button) -> Bind to Ctrl or Right Trigger.
   - On the Player Input component in the inspector, drag this asset into the 'Actions' slot.
   - Change 'Behavior' to "Invoke Unity Events".
   - Expand 'Events' > 'Player' and link the script functions (OnMove, OnJump, etc.).

5. CONFIGURE THE SCRIPT:
   - In the Player's Inspector, find the 'Ground Layer' dropdown in this script.
   - Select the "Ground" layer you created. This is vital for jump detection!
================================================================================
*/