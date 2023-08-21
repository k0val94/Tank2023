using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;

    private SpawnManager spawnManager;
    private GameObject playerPrefab;

    public void Initialize(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;

        Debug.Log("HealthManager initialized.");
    }

    private void Start()
    {
        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;

        if (currentLives <= 0)
        {
            spawnManager.RespawnPlayer(playerPrefab);
        }
    }
}
