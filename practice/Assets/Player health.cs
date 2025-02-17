using UnityEngine;
using UnityEngine.SceneManagement; // For restarting the level

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    public int damage = 1;
    private bool isTakingDamage = false; // Prevents multiple triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && !isTakingDamage)
        {
            TakeDamage(damage);
            isTakingDamage = true; // Prevent multiple damage triggers
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            isTakingDamage = false; // Reset when player leaves obstacle
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Player Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        gameObject.SetActive(false); // Hide player
        Invoke(nameof(RestartGame), 2f); // Restart after 2 seconds
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload level
    }
}
