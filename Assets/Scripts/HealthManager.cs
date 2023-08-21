using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 6;
    private int currentLives;

    private SpawnManager spawnManager;
    private BottomBar bottomBar; 

    public void Initialize(BottomBar bottomBar)
    {
        this.bottomBar = bottomBar; 
        currentLives = maxLives;
        spawnManager = GetComponent<SpawnManager>();

        if (bottomBar != null)
        {
            bottomBar.UpdateHeartsState(currentLives);
        }

        Debug.Log("HealthManager initialized.");
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;

        if (bottomBar != null)
        {
            bottomBar.UpdateHeartsState(currentLives); 
        }

        if (currentLives <= 0)
        {
            spawnManager.RespawnPlayer();
        }
    }
}
