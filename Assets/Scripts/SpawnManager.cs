using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    private GameObject playerTankPrefab;
    private GameObject playerTank;
    private GameObject enemyTankPrefab;
    private GameObject currentEnenmyTank;

    private float minX = -5f; 
    private float maxX = 5f;
    private float minY = -5f;
    private float maxY = 5f;

    private bool isInitialized = false;
    private bool isPlayerSpawned = false; 

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
            if (isPlayerSpawned)
            {
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                Vector3 randomPosition = new Vector3(randomX, randomY, 0f);

                SpawnEnemy(randomPosition);
            }

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
            if (!isPlayerSpawned)
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
