using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerMovement : MonoBehaviour
{
    // References
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    // Movement Variables
    public float moveSpeed = 5f;       // Speed of player movement
    public float jumpForce = 10f;      // Force applied to jump
    public float groundCheckRadius = 0.2f;   // Radius to check for the ground
    public LayerMask groundLayer;     // Layer that represents ground

    private bool isGrounded = false;   // Whether the player is grounded

    // Input Variables
    private PlayerInput playerInput;  // Reference to the PlayerInput component
    private InputAction moveAction;   // Action for movement
    private InputAction jumpAction;   // Action for jump
    private InputAction crouchAction; // Action for crouch

    // Wall Climbing Variables
    public float wallClimbSpeed = 3f;  // Speed of climbing up or down
    public float wallStickTime = 0.2f; // Time the player sticks to the wall after letting go
    public LayerMask wallLayer;       // Layer that represents walls

    private bool isTouchingWall = false;  // Whether the player is near a wall
    private bool isClimbingWall = false; // Whether the player is climbing the wall
    private float wallStickTimer = 0.01f;   // Timer for wall sticking

    void Awake()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set up Input Actions
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];  // Input action for movement
        jumpAction = playerInput.actions["Jump"];  // Input action for jump
        crouchAction = playerInput.actions["Crouch"]; // Input action for crouch
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<Collider2D>().bounds.extents.y, 0), groundCheckRadius, groundLayer);

        // Get movement input from the new Input System
        moveDirection = moveAction.ReadValue<Vector2>();

        // Jump input (only allow jump if grounded)
        if (isGrounded && jumpAction.triggered)
        {
            Jump();
        }

        // Check if the player is touching a wall
        isTouchingWall = Physics2D.OverlapCircle(transform.position + new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius, wallLayer)
                         || Physics2D.OverlapCircle(transform.position - new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius, wallLayer);

        // Start wall climbing if touching a wall and crouch is held
        if (isTouchingWall && crouchAction.IsPressed())
        {
            isClimbingWall = true;
        }
        else if (!crouchAction.IsPressed())
        {
            isClimbingWall = false;
            wallStickTimer = wallStickTime; // Start stick timer when climbing ends
        }

        HandleWallClimbingVisuals();
    }

    void FixedUpdate()
    {
        if (isClimbingWall)
        {
            // Freeze horizontal movement while climbing
            rb.linearVelocity = new Vector2(0, moveDirection.y * wallClimbSpeed);
        }
        else if (wallStickTimer > 0)
        {
            // Reduce stick timer
            wallStickTimer -= Time.fixedDeltaTime;

            // Slightly reduce vertical velocity to simulate sticking
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -2f, 0));
        }
        else
        {
            // Normal movement
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        // Apply a force to the player for jumping
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void HandleWallClimbingVisuals()
    {
        if (isClimbingWall)
        {
            // Example: Change sprite color while climbing
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            // Revert to default color
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
