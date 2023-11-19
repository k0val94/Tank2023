using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBar healthBarPrefab;
    private HealthBar healthBarInstance;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No canvas found in the scene.");
            return;
        }
        
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
            healthBarInstance.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("HealthBar prefab not assigned.");
        }
    }

    public void TakeDamage(float damage)
    {
        int damageToTake = Mathf.RoundToInt(damage);
        currentHealth -= damageToTake;
        currentHealth = Mathf.Max(currentHealth, 0); 

        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died."); 
            // Handle player death here.
        }
    }
}