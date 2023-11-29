using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyTankPrefab;

    private float minX = -5f; 
    private float maxX = 5f;
    private float minY = -5f;
    private float maxY = 5f;

    private bool isInitialized = false;

    private void Start()
    {
        isInitialized = true;
        StartCoroutine(SpawnEnemyRandomlyCoroutine());
        Debug.Log("EnemySpawnManager initialized.");
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
            break;
        }
    }

    private void SpawnEnemy(Vector3 position)
    {

        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null && (colliderAtSpawnPoint.CompareTag("Brick") || (colliderAtSpawnPoint.CompareTag("Steel") || (colliderAtSpawnPoint.CompareTag("Water")))))
        {
            Debug.Log("Cannot spawn enemy on a Barrier!");
            return;
        }

        Instantiate(enemyTankPrefab, position, Quaternion.identity);
        Debug.Log("Enemy spawned.");
    }
}
