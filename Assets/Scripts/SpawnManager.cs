using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameObject playerTankPrefab;
    private GameObject currentPlayerTank;

    private bool isInitialized = false; 

    public void Initialize(GameObject playerTankPrefab)
    {
        this.playerTankPrefab = playerTankPrefab;
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
            if (currentPlayerTank == null)
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
        if (colliderAtSpawnPoint != null && (colliderAtSpawnPoint.CompareTag("Brick") || (colliderAtSpawnPoint.CompareTag("Steel"))))
        {
            Debug.Log("Cannot spawn player on a barrier!");
            return;
        }

        if (currentPlayerTank == null)
        {
            currentPlayerTank = Instantiate(playerTankPrefab, position, Quaternion.identity);

            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(currentPlayerTank.transform);
            }

            Debug.Log("Player spawned.");
        }
        else 
        {
            Debug.Log("playerTankPrefab:" + playerTankPrefab);
        }
    }

    public void DespawnPlayer()
    {
        Debug.Log("Despawning player...");

        if (currentPlayerTank != null)
        {
            Destroy(currentPlayerTank);

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
        //SpawnPlayerAtMousePosition();

    }

    private void SpawnPlayerAtMousePosition()
    {
        Debug.Log("Spawning player at mouse position...");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        SpawnPlayer(ray.origin);
        
    }

    public GameObject getCurrentPlayerTank(){
        return currentPlayerTank;
    }

}
