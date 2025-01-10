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

    [Header("Grappling Hook Settings")]
    public LayerMask grappleLayer;
    public float maxGrappleDistance = 15f;
    public float grappleSpeed = 10f;
    public Transform grappleOrigin;
    private LineRenderer grappleLine;
    private bool isGrappling = false;
    private Vector2 grapplePoint;
    private bool isWallHanging = false;
    private bool isWallClimbing = false;
    private bool isWallClimbPressed = false;
    private bool isWallSliding = false;  // Add this variable to track wall sliding state

    private Rigidbody2D rb;
    private Vector2 inputMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleLine = GetComponent<LineRenderer>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle horizontal movement
        float moveInput = inputMovement.x;
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        if (!isGrappling)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);  // Using velocity instead of linearVelocity
        }

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
        bool isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * transform.localScale.x, wallCheckDistance, wallLayer);

        // Handle wall sliding
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;  // Set wall sliding state
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;  // Set wall sliding state to false when not sliding
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
        else if (isGrounded && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * 2 * Time.deltaTime;  // Faster regen when grounded
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private void DrainStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * grappleSpeed;  // Using velocity instead of linearVelocity

            if (Vector2.Distance(transform.position, grapplePoint) < 0.5f)
            {
                StopGrapple();
            }
        }
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

    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.performed && !isGrappling)
        {
            StartGrapple();
        }
        else if (context.canceled)
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        Vector2 aimDirection = inputMovement;
        if (aimDirection == Vector2.zero) aimDirection = Vector2.up;

        RaycastHit2D hit = Physics2D.Raycast(grappleOrigin.position, aimDirection, maxGrappleDistance, grappleLayer);
        if (hit.collider != null)
        {
            isGrappling = true;
            grapplePoint = hit.point;
            grappleLine.enabled = true;
            grappleLine.SetPosition(0, grappleOrigin.position);
            grappleLine.SetPosition(1, grapplePoint);
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        grappleLine.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Visualize wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * transform.localScale.x * wallCheckDistance);

        // Visualize grapple range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(grappleOrigin.position, maxGrappleDistance);
    }
}
