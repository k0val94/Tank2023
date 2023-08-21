using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera Instance;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float followSpeed = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.position = smoothPosition;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


}