using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameObject currentPlayerCharacterInstance;
    private GameObject currentSpawnedTankPrefab;

    private GameObject playerPrefab; 

    private bool isInitialized = false; 

    public void Initialize(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
        isInitialized = true;

        Debug.Log("SpawnManager initialized.");
    }

    private void Update()
    {
        if (!isInitialized)
        {
            Debug.Log("SpawnManager not initialized yet.");
            return;
        }

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

        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && colliderAtSpawnPoint.CompareTag("Barrier"))
        {
            Debug.Log("Cannot spawn player on a barrier!");
            return;
        }

        if (currentPlayerCharacterInstance == null)
        {
            currentPlayerCharacterInstance = Instantiate(playerPrefab, position, Quaternion.identity);
            currentSpawnedTankPrefab = playerPrefab;

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
