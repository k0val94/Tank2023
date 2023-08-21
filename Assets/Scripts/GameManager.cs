using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private BottomBar bottomBar; 

    [Header("Prefabs")]
    [SerializeField] private GameObject playerTankPrefab;
    private GameObject currentPlayerTank;

    private void Start()
    {
        if (mapGenerator != null)
        {
            mapGenerator.Generate();
        }
        else
        {
            Debug.LogError("Map Generator is not assigned!");
        }

        if (spawnManager != null)
        {
            spawnManager.Initialize(playerTankPrefab); 
            currentPlayerTank = spawnManager.getCurrentPlayerTank();
        }
        else
        {
            Debug.LogError("Spawn Manager is not assigned!");
        }

        if (healthManager != null)
        {
            healthManager.Initialize(bottomBar); 
        }
        else
        {
            Debug.LogError("Health Manager is not assigned!");
        }
    }
}
