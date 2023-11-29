using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfNoise : MonoBehaviour
{
    public float hearingRadius;
    public LayerMask targetMask;

    public List<Transform> audibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindAudibleTargets();
        }
    }

    void FindAudibleTargets()
    {
        audibleTargets.Clear();

        Collider2D[] targetsInHearingRadius = Physics2D.OverlapCircleAll(transform.position, hearingRadius, targetMask);

        foreach (Collider2D targetCollider in targetsInHearingRadius)
        {
            Transform target = targetCollider.transform;
            audibleTargets.Add(target);
            Debug.Log("Target " + target.name + " is audible.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
