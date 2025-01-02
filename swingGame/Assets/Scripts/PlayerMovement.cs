using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float crouchSpeed = 2.5f;
    private bool isCrouching = false;

    [Header("Wall Mechanics")]
    public LayerMask wallLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public float slideSpeed = -2f;
    private bool isWallSliding = false;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Vector2 inputMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle horizontal movement
        float moveInput = inputMovement.x;
        float adjustedSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * adjustedSpeed, rb.linearVelocity.y);

        // Flip sprite based on movement direction
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Wall sliding logic
        bool isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * transform.localScale.x, wallCheckDistance, wallLayer);
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Handle wall sliding and hanging
        if (isWallSliding)
        {
            if (isCrouching) // Wall hang
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Stop vertical movement
            }
            else // Wall slide
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideSpeed);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Visualize wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDistance);
    }
}
