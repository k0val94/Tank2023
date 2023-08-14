using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // Singleton instance

    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;

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

            // Reset camera target
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
}
