using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerTankPrefab;
    private GameObject playerTank;

    private bool isPlayerSpawned = false; 

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPlayerSpawned)
        {
            SpawnPlayerAtMousePosition();
        }
    }

    void SpawnPlayer(Vector3 position)
    {

        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && (colliderAtSpawnPoint.CompareTag("Brick") || (colliderAtSpawnPoint.CompareTag("Steel"))))
        {
            Debug.Log("Cannot spawn player on a Barrier!");
            return;
        }

        playerTank = Instantiate(playerTankPrefab, position, Quaternion.identity);
        isPlayerSpawned = true;

        if (FollowCamera.Instance != null)
        {
            FollowCamera.Instance.SetTarget(playerTank.transform);
        }

        Debug.Log("Player spawned.");

    }

    public void DespawnPlayer()
    {
        Debug.Log("Despawning player...");

        if (isPlayerSpawned)
        {
            Destroy(playerTank);
            isPlayerSpawned = false;

            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(null);
            }

            Debug.Log("Player despawned.");
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawning player...");
        DespawnPlayer();
        SpawnPlayerAtMousePosition();

    }

    private void SpawnPlayerAtMousePosition()
    {
        Debug.Log("Spawning player at mouse position...");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        SpawnPlayer(ray.origin);
        
    }
}
