using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private BottomBar bottomBar; // Referenz zur BottomBar-Klasse

    [Header("Prefabs")]
    public GameObject playerTankPrefab;

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
        }
        else
        {
            Debug.LogError("Spawn Manager is not assigned!");
        }

        if (healthManager != null)
        {
            healthManager.Initialize(playerTankPrefab, bottomBar); // BottomBar-Referenz übergeben
        }
        else
        {
            Debug.LogError("Health Manager is not assigned!");
        }
    }
}
