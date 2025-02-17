using UnityEngine;

public class PracticeMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpForce = 5f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Instant jump
            isGrounded = false; // Prevent double jumping
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Reset jump when touching the ground
        }
    }
}
