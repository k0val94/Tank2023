using UnityEngine;
using System.Collections;

public class PlayerTankMovementController : MonoBehaviour
{
    private float minZoom = 5f;
    private float maxZoom = 15f;
    private float zoomSpeed = 30f;

    private Rigidbody2D playerTankRigidbody2D;
    private TankPhysicsController tankPhysicsController;

    private void Start()
    {
        playerTankRigidbody2D = GetComponent<Rigidbody2D>();
        tankPhysicsController = GetComponent<TankPhysicsController>();
    }

    private void Update()
    {
        HandleCameraZoom();
    }

    private void FixedUpdate()
    {
         HandleTankMovement();
    }

    private void HandleCameraZoom()
    {
        if (Camera.main != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize -= scrollInput * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandleTankMovement()
    {
        float move = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");




        tankPhysicsController.MoveTank(move, rotate);
    }
}
