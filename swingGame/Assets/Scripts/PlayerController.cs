using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    public float staminaDrainRate = 10f; // Drain per second while hanging or climbing
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

    private Rigidbody2D rb;
    private Vector2 inputMovement;
    private Vector3 velocity; // Store velocity for gravity and jump

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleLine = GetComponent<LineRenderer>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle wall mechanics
        HandleWallMechanics();

        // Apply movement
        Vector2 move = new Vector2(inputMovement.x, rb.linearVelocity.y); // Keep vertical velocity for gravity/jumping
        ApplyMovement(move);

        // Handle stamina regeneration
        RegenerateStamina();

        // Handle crouching visuals (if needed)
        AdjustCrouchHeight();
    }

    private void HandleWallMechanics()
    {
        bool isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);

        if (isTouchingWall && !isGrounded && !isGrappling)
        {
            // Wall Slide
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);

            // Drain stamina while climbing or sliding on the wall
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            // Wall Jump
            if (inputMovement.x != 0 && currentStamina >= wallJumpStaminaCost)
            {
                currentStamina -= wallJumpStaminaCost;
                rb.linearVelocity = new Vector2(wallJumpForceX * inputMovement.x, wallJumpForceY);
            }
        }
    }

    private void ApplyMovement(Vector2 moveDirection)
    {
        if (isGrounded)
        {
            // Reset vertical velocity when grounded
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Apply movement while airborne
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private void AdjustCrouchHeight()
    {
        // Adjust the player's collider height when crouching (optional)
        float targetHeight = isCrouching ? 1.0f : 2.0f;
        Vector3 newScale = transform.localScale;
        newScale.y = Mathf.Lerp(transform.localScale.y, targetHeight, Time.deltaTime * 10f);
        transform.localScale = newScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(grappleOrigin.position, maxGrappleDistance);
    }
}
