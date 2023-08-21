using UnityEngine;

public class PlayerTankManager : MonoBehaviour
{
    [SerializeField] private PlayerTankMovementController playerTankMovementController;
    [SerializeField] private PlayerTankAnimationsController playerTankAnimationsController;
    [SerializeField] private GameObject playerTankTurretPrefab;
    private GameObject currentPlayerTankTurretInstance;

    private void Start()
    {
        if (playerTankTurretPrefab != null)
        {
            currentPlayerTankTurretInstance = Instantiate(playerTankTurretPrefab, transform); 
            currentPlayerTankTurretInstance.transform.localPosition = Vector3.zero; 
        }
        else
        {
            Debug.LogError("PlayerTankTurretPrefab is not assigned!");
        }

        if (playerTankAnimationsController != null)
        {
            //playerTankAnimationsController.Initialize(playerTankMovementController);
        }
        else
        {
            Debug.LogError("PlayerTankAnimationsController is not assigned!");
        }

        if (playerTankMovementController != null)
        {
            playerTankMovementController.Initialize(currentPlayerTankTurretInstance);
        }
        else
        {
            Debug.LogError("PlayerTankMovementController is not assigned!");
        }
    }
}
