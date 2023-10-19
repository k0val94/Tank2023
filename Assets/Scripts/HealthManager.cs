using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 6;
    private int currentLives;

    private SpawnManager spawnManager;

    public void Initialize()
    {
        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();

        Debug.Log("HealthManager initialized.");
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;

        if (currentLives <= 0)
        {
            spawnManager.RespawnPlayer();
        }
    }
}
