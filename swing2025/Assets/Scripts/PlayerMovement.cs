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
    private float wallStickTimer = 0.2f;   // Timer for wall sticking

    void Awake()
    {
        InitializeReferences();
    }

    void Update()
    {
        UpdateGroundedState();
        ProcessInput();
        UpdateWallInteraction();
        HandleWallClimbingVisuals();

        Debug.Log($"isGrounded: {isGrounded}, isTouchingWall: {isTouchingWall}, isClimbingWall: {isClimbingWall}");
        Debug.Log($"moveDirection: {moveDirection}, rb.velocity: {rb.linearVelocity}");
    }

    void FixedUpdate()
    {
        if (isClimbingWall)
        {
            ClimbWall();
        }
        else if (wallStickTimer > 0)
        {
            HandleWallStick();
        }
        else
        {
            Move();
        }

        Debug.Log($"FixedUpdate -> rb.velocity: {rb.linearVelocity}");
    }

    private void InitializeReferences()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
        Debug.Log("PlayerInput actions initialized correctly.");
    }

    private void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, GetComponent<Collider2D>().bounds.extents.y, 0), groundCheckRadius, groundLayer);
    }

    private void ProcessInput()
    {
        moveDirection = moveAction.ReadValue<Vector2>();

        if (isGrounded && jumpAction.triggered)
        {
            Jump();
        }
    }

    private void UpdateWallInteraction()
    {
        isTouchingWall = Physics2D.OverlapCircle(transform.position + new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius, wallLayer)
                         || Physics2D.OverlapCircle(transform.position - new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius, wallLayer);

        if (isTouchingWall && crouchAction.IsPressed())
        {
            isClimbingWall = true;
            Debug.Log("Wall climbing started.");
        }
        else if (!crouchAction.IsPressed())
        {
            isClimbingWall = false;
            wallStickTimer = wallStickTime;
            Debug.Log("Wall climbing stopped.");
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);



    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        Debug.Log("Player Jumped!");
    }

    private void ClimbWall()
    {
        rb.linearVelocity = new Vector2(0, moveDirection.y * wallClimbSpeed);
    }

    private void HandleWallStick()
    {
        wallStickTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -2f, 0));
    }

    private void HandleWallClimbingVisuals()
    {
        if (isClimbingWall)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, GetComponent<Collider2D>().bounds.extents.y, 0), groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position - new Vector3(GetComponent<Collider2D>().bounds.extents.x, 0), groundCheckRadius);
    }
}
