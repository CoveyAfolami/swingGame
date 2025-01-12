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

    void Awake()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set up Input Actions
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];  // Input action for movement
        jumpAction = playerInput.actions["Jump"];  // Input action for jump
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
    }

    void FixedUpdate()
    {
        // Move the player based on input
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        // Apply a force to the player for jumping
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
