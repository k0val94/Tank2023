using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // Singleton instance

    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;

    [Header("Camera Settings")]
    public float cameraFollowSpeed = 2f; // Geschwindigkeit, mit der die Kamera dem Spieler folgt

    [Header("Spawn Settings")]
    public float spawnDuration = 1.0f; // Dauer des Spawn-Prozesses

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayer(Vector3 position)
    {
        // Überlappungs-Check
        Collider2D colliderAtSpawnPoint = Physics2D.OverlapPoint(position);
        if (colliderAtSpawnPoint != null)
        {
            // Überprüfen, ob das Objekt an dieser Position ein "Brick" oder ein anderes Hindernis ist
            if (colliderAtSpawnPoint.CompareTag("Brick") /* oder andere Tags für Hindernisse */)
            {
                Debug.Log("Cannot spawn player on a brick or obstacle!");
                return; // Beendet die Methode frühzeitig, sodass der Spieler nicht gespawnt wird.
            }
        }

        if (currentPlayerInstance == null)
        {
            currentPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);

            // Set camera target
            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(currentPlayerInstance.transform);
            }
        }
    }


    public void DespawnPlayer()
    {
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
            currentPlayerInstance = null;

            // Kamera-Ziel zurücksetzen
            if (FollowCamera.Instance != null)
            {
                FollowCamera.Instance.SetTarget(null);
            }
        }
    }

    public bool IsPlayerSpawned()
    {
        return currentPlayerInstance != null;
    }

    private IEnumerator FadeIn(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float elapsedTime = 0;
            Color color = sr.color;
            while (elapsedTime < spawnDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, elapsedTime / spawnDuration);
                sr.color = color;
                yield return null;
            }
            color.a = 1;
            sr.color = color;
        }
    }
}
