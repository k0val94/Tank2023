using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera Instance;

    [SerializeField] private Transform target;
    private Vector3 offset = new Vector3(0, 0, -10);
    private float followSpeed = 2f;
    private float mouseFollowSpeed = 0.05f;
    private float edgeMargin = 0.01f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (target != null)
        {
            FollowTarget();
        }
        else
        {
            FollowMouse();
        }
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothPosition;
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector3 moveDir = new Vector3(0, 0, 0);

        if (mousePosition.x < edgeMargin)
        {
            moveDir.x = -1;
        }
        else if (mousePosition.x > 1 - edgeMargin)
        {
            moveDir.x = 1;
        }

        if (mousePosition.y < edgeMargin)
        {
            moveDir.y = -1;
        }
        else if (mousePosition.y > 1 - edgeMargin)
        {
            moveDir.y = 1;
        }

        transform.position += moveDir * mouseFollowSpeed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}