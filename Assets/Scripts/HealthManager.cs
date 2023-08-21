using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 6;
    [SerializeField] private int currentLives;
    [SerializeField] private HealthDisplayUI healthDisplay;

    private SpawnManager spawnManager;
    private GameObject playerPrefab;

    public void Initialize(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
        
        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();

        healthDisplay.Initialize(maxLives);
        Debug.Log("HealthManager initialized.");
    }



    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;
        healthDisplay.UpdateHealthDisplay(currentLives);

        if (currentLives <= 0)
        {
            spawnManager.RespawnPlayer(playerPrefab);
        }
    }

}
