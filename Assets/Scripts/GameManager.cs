using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public SpawnManager spawnManager;
    public HealthManager healthManager;

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
            spawnManager.Initialize(playerTankPrefab); // Weitere Feind-Prefabs übergeben
        }
        else
        {
            Debug.LogError("Spawn Manager is not assigned!");
        }

        if (healthManager != null)
        {
            healthManager.Initialize(playerTankPrefab); // Weitere Feind-Prefabs übergeben
        }
        else
        {
            Debug.LogError("Health Manager is not assigned!");
        }
    }
}
