using UnityEngine;

public class EnemyTankManager : MonoBehaviour
{
    [SerializeField] private EnemyTankMovementController enemyTankMovementController;
    [SerializeField] private EnemyTankAnimationsController enemyTankAnimationsController;
    [SerializeField] private GameObject enemyTankTurretPrefab;
    private GameObject currentEnemyTankTurretInstance;

    private void Start()
    {
        if (enemyTankTurretPrefab != null)
        {
            currentEnemyTankTurretInstance = Instantiate(enemyTankTurretPrefab, transform); 
            currentEnemyTankTurretInstance.transform.localPosition = Vector3.zero; 
        }
        else
        {
            Debug.LogError("EnemyTankTurretPrefab is not assigned!");
        }

       if (enemyTankAnimationsController != null)
        {
            enemyTankAnimationsController.Initialize(enemyTankMovementController, currentEnemyTankTurretInstance);
        }
        else
        {
            Debug.LogError("EnemyTankAnimationsController is not assigned!");
        }

        if (enemyTankMovementController != null)
        {
            enemyTankMovementController.Initialize(currentEnemyTankTurretInstance);
        }
        else
        {
            Debug.LogError("EnemyTankMovementController is not assigned!");
        }
    }
}
