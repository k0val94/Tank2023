using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 6;
    [SerializeField] private int currentLives;

    private SpawnManager spawnManager;
    private GameObject playerPrefab;

    public void Initialize(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;

        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();

        Debug.Log("HealthManager initialized.");
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
