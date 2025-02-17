using UnityEngine;
using UnityEngine.InputSystem;  // Import the InputSystem namespace

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f; // Adjustable jump force
    public float groundCheckDistance = 0.1f; // Distance for Raycast to check ground
    public LayerMask groundLayer; // Layer to detect the ground

    private Rigidbody2D rb;
    private bool isGrounded;

    // Input action reference for jumping
    private InputAction jumpAction;

    void Awake()
    {
        // Create an instance of the InputAction for jump
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space"); // Default to space bar
        jumpAction.AddBinding("<Gamepad>/buttonSouth"); // Gamepad button (e.g., A on Xbox)
    }

    void OnEnable()
    {
        // Enable the action when the player is active
        jumpAction.Enable();
    }

    void OnDisable()
    {
        // Disable the action when the player is disabled
        jumpAction.Disable();
    }

    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Perform the Raycast to check if the player is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // Check for jump input using the Input System
        if (jumpAction.triggered && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        // Apply upward force while keeping the horizontal velocity unchanged
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void OnDrawGizmosSelected()
    {
        // Draw the Raycast path in the Scene view for debugging
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
