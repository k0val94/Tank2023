using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera Instance;

    public Transform target;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}
