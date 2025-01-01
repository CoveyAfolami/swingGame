using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 5f;
    public float maxSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float maxJumpHoldTime = 0.2f;
    public float holdJumpForce = 5f;

    [Header("Wall Mechanics")]
    public LayerMask wallLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public float climbSpeed = 3f;
    public float slideSpeed = -2f;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Advanced Jump Settings")]
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private float horizontalInput;
    private bool isJumping;
    private float jumpHoldTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gather input
        horizontalInput = Input.GetAxis("Horizontal");

        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);

        // Update coyote time counter
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update jump buffer counter
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Check for jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            StartJump();
            jumpBufferCounter = 0;
        }

        // Variable jump height
        if (isJumping && Input.GetButton("Jump"))
        {
            HoldJump();
        }
        else
        {
            isJumping = false;
        }

        // Wall climb
        if (isTouchingWall && Input.GetButton("Climb"))
        {
            ClimbWall();
        }

        // Flip sprite based on movement direction
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        // Apply movement force
        Move();

        // Wall sliding
        if (isTouchingWall && !Input.GetButton("Climb"))
        {
            WallSlide();
        }
    }

    void Move()
    {
        // Apply horizontal force for acceleration
        float targetSpeed = horizontalInput * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelerationRate = Mathf.Abs(targetSpeed) > 0.1f ? acceleration : acceleration * 2;

        rb.AddForce(Vector2.right * speedDifference * accelerationRate);

        // Clamp max speed
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
        }
    }

    void StartJump()
    {
        isJumping = true;
        jumpHoldTimer = 0; // Reset jump hold timer
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset vertical velocity for consistent jumps
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0; // Prevent multiple jumps during coyote time
    }

    void HoldJump()
    {
        if (jumpHoldTimer < maxJumpHoldTime)
        {
            rb.AddForce(Vector2.up * holdJumpForce * Time.deltaTime, ForceMode2D.Force);
            jumpHoldTimer += Time.deltaTime;
        }
    }

    void ClimbWall()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeed);
    }

    void WallSlide()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
