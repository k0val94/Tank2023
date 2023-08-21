using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 6;
    private int currentLives;

    private SpawnManager spawnManager;
    private GameObject playerPrefab;
    private BottomBar bottomBar; // Referenz zur BottomBar-Klasse

    public void Initialize(GameObject playerPrefab, BottomBar bar)
    {
        this.playerPrefab = playerPrefab;
        bottomBar = bar; // BottomBar-Referenz setzen

        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();

        if (bottomBar != null)
        {
            bottomBar.UpdateHeartsState(currentLives); // Initialer Zustand der Herzen im UI
        }

        Debug.Log("HealthManager initialized.");
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;

        if (bottomBar != null)
        {
            bottomBar.UpdateHeartsState(currentLives); // Aktualisiere Herzen im UI
        }

        if (currentLives <= 0)
        {
            spawnManager.RespawnPlayer(playerPrefab);
        }
    }
}
