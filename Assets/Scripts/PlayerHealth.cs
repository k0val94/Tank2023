using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = FindObjectOfType<HealthBar>(); // Find the HealthBar in the scene.
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

    }

    public void TakeDamage(float damage)
    {
        int damageToTake = Mathf.RoundToInt(damage);
        currentHealth -= damageToTake;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevents health from going negative.

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died."); 
            // Handle player death here.
        }
    }
}
