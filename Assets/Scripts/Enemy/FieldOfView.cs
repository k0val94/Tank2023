using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.4f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        foreach (Collider2D targetCollider in targetsInViewRadius)
        {
            Transform target = targetCollider.transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (!Physics2D.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                visibleTargets.Add(target);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Define the number of rays to cast (more rays mean a more detailed field of view)
        int rayCount = 60;
        float angleStep = 360f / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.z + (angleStep * i);
            Vector2 dir = AngleToDirection(angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
            if (hit)
            {
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Vector2 endPosition = transform.position + (Vector3)(dir * viewRadius);
                Gizmos.DrawLine(transform.position, endPosition);
            }
        }
    }

    Vector2 AngleToDirection(float angleInDegrees)
    {

        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }

}
