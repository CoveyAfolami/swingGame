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
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = -2f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 15f;
    public float wallClimbSpeed = 3f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f; // Drain per second while hanging
    public float wallJumpStaminaCost = 20f; // Cost for wall jumping
    public float staminaRegenRate = 15f; // Regen per second when not hanging or climbing
    private float currentStamina;

    [Header("Ground Mechanics")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Vector2 inputMovement;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool isWallHanging = false;
    private bool isWallClimbing = false;
    private bool isWallClimbPressed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle horizontal movement
        float moveInput = inputMovement.x;
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Flip sprite based on movement direction
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Handle stamina regeneration
        RegenerateStamina();

        // Wall mechanics
        HandleWallMechanics();
    }

    private void HandleWallMechanics()
    {
        // Check if the player is touching a wall
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * transform.localScale.x, wallCheckDistance, wallLayer);

        // Handle wall sliding
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }

        // Handle wall hanging
        if (isTouchingWall && isWallClimbPressed && currentStamina > 0)
        {
            isWallHanging = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Stop vertical movement
            DrainStamina(staminaDrainRate * Time.deltaTime);
        }
        else
        {
            isWallHanging = false;
        }

        // Handle wall climbing
        if (isWallHanging && inputMovement.y > 0 && currentStamina > 0)
        {
            isWallClimbing = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallClimbSpeed);
            DrainStamina(staminaDrainRate * Time.deltaTime);
        }
        else
        {
            isWallClimbing = false;
        }
    }

    private void RegenerateStamina()
    {
        if (!isWallHanging && !isWallClimbing && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private void DrainStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isWallSliding && currentStamina >= wallJumpStaminaCost)
            {
                // Wall jump
                rb.linearVelocity = new Vector2(-transform.localScale.x * wallJumpForceX, wallJumpForceY);
                DrainStamina(wallJumpStaminaCost);
            }
            else if (isGrounded)
            {
                // Ground jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    public void OnWallClimb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isWallClimbPressed = true;
        }
        else if (context.canceled)
        {
            isWallClimbPressed = false;
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
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * transform.localScale.x * wallCheckDistance);
    }
}
