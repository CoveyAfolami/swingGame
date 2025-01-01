using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRechargeRate = 5f; // Normal recharge rate
    public float staminaRechargeRateWhileSliding = 3f; // Recharge rate while sliding down
    public float staminaDrainClimb = 10f; // Drain rate while climbing
    public float staminaDrainWallJump = 20f;

    [Header("UI")]
    public Slider healthBar;
    public Slider staminaBar;

    private void Start()
    {
        // Initialize health and stamina values
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        // Initialize health and stamina bars
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
    }

    private void Update()
    {
        // Update the UI bars based on current values
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (staminaBar != null)
            staminaBar.value = currentStamina;
    }

    public void DrainStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0);
    }

    public void RechargeStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
