using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    private GameObject playerTankPrefab;
    private GameObject currentPlayerTank;
    private GameObject enemyTankPrefab;
    private GameObject currentEnenmyTank;

    public float minX = -10f; // Ersetzen Sie diese Werte mit den tatsächlichen Grenzen
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;

    private bool isInitialized = false;

    public void Initialize(GameObject playerTankPrefab, GameObject enemyTankPrefab)
    {
        this.playerTankPrefab = playerTankPrefab;
        this.enemyTankPrefab = enemyTankPrefab;
        isInitialized = true;

        Debug.Log("SpawnManager initialized.");

        StartCoroutine(SpawnEnemyRandomlyCoroutine());
    }

    private IEnumerator SpawnEnemyRandomlyCoroutine()
    {
        while (true)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0f);

            SpawnEnemy(randomPosition);

            yield return new WaitForSeconds(5f);
        }
    }


    private void SpawnEnemy(Vector3 position)
    {

        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && (colliderAtSpawnPoint.CompareTag("Brick") || (colliderAtSpawnPoint.CompareTag("Steel"))))
        {
            Debug.Log("Cannot spawn enemy on a Barrier!");
            return;
        }

        Instantiate(enemyTankPrefab, position, Quaternion.identity);
        Debug.Log("Enemy spawned.");
    }


      private void Update()
    {
        if (!isInitialized)
        {
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

    void SpawnPlayer(Vector3 position)
    {

        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && (colliderAtSpawnPoint.CompareTag("Brick") || (colliderAtSpawnPoint.CompareTag("Steel"))))
        {
            Debug.Log("Cannot spawn player on a Barrier!");
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
