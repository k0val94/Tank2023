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

        // Umwandeln der Mausposition in eine Weltposition in 2D
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // Sicherstellen, dass die Z-Achse auf einen gültigen Wert gesetzt ist
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0; // Z-Wert auf 0 setzen, um in der 2D-Ebene zu bleiben

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider == null || (!hit.collider.CompareTag("Brick") && !hit.collider.CompareTag("Steel")))
        {
            SpawnPlayer(worldPosition);
        }
        else
        {
            Debug.Log("Cannot spawn player on a Barrier!");
        }
    }

}
