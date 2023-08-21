using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameObject currentPlayerCharacterInstance;
    private GameObject currentSpawnedTankPrefab;

    private GameObject playerPrefab; // Referenz auf das Spieler-Panzer-Prefab

    private bool isInitialized = false; // Variable, um zu überprüfen, ob der SpawnManager initialisiert wurde

    public void Initialize(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
        isInitialized = true;

        Debug.Log("SpawnManager initialized.");
    }

    private void Update()
    {
        // Warten, bis der SpawnManager initialisiert ist, bevor der Spieler gespawnt wird
        if (!isInitialized)
        {
            Debug.Log("SpawnManager not initialized yet.");
            return;
        }

        // Spieler spawnt, wenn die linke Maustaste gedrückt wird
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPlayerSpawned())
            {
                SpawnPlayerAtMousePosition();
            }
            else
            {
                Debug.Log("Player is already spawned.");
            }
        }
    }

    public void SpawnPlayer(Vector3 position)
    {
        Debug.Log("Spawning player...");

        // Überlappungs-Check
        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && colliderAtSpawnPoint.CompareTag("Barrier"))
        {
            Debug.Log("Cannot spawn player on a barrier!");
            return;
        }

        if (currentPlayerCharacterInstance == null)
        {
            currentPlayerCharacterInstance = Instantiate(playerPrefab, position, Quaternion.identity);
            currentSpawnedTankPrefab = playerPrefab; // Aktuelles Prefab speichern

            // Set camera target
            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(currentPlayerCharacterInstance.transform);
            }

            Debug.Log("Player spawned.");
        }
    }

    public void DespawnPlayer()
    {
        Debug.Log("Despawning player...");

        if (currentPlayerCharacterInstance != null)
        {
            Destroy(currentPlayerCharacterInstance);
            currentPlayerCharacterInstance = null;

            // Kamera-Ziel zurücksetzen
            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(null);
            }

            Debug.Log("Player despawned.");
        }
    }

    public bool IsPlayerSpawned()
    {
        return currentPlayerCharacterInstance != null;
    }

    public void RespawnPlayer(GameObject playerPrefab)
    {
        Debug.Log("Respawning player...");

        DespawnPlayer();
    }

    private void SpawnPlayerAtMousePosition()
    {
        Debug.Log("Spawning player at random position...");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider == null)
        {
            Debug.Log("Ray did not hit any object.");
            SpawnPlayer(ray.origin);
        }
        else if (!hit.collider.CompareTag("Barrier"))
        {
            SpawnPlayer(hit.point);
        }
    }

    public GameObject GetCurrentSpawnedTankPrefab()
    {
        return currentSpawnedTankPrefab;
    }
}
