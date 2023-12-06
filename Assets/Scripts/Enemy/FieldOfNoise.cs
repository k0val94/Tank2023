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
        List<Transform> previouslyAudibleTargets = new List<Transform>(audibleTargets);
        audibleTargets.Clear();

        Collider2D[] targetsInHearingRadius = Physics2D.OverlapCircleAll(transform.position, hearingRadius, targetMask);

        foreach (Collider2D targetCollider in targetsInHearingRadius)
        {
            Transform target = targetCollider.transform;
            // Beispiel für zusätzliche Logik zur Bestimmung, ob das Ziel 'gehört' werden kann
            // Hier nur das Hinzufügen zum audibleTargets

            audibleTargets.Add(target);
            if (!previouslyAudibleTargets.Contains(target))
            {
                Debug.Log("Panzer hört ein Ziel: " + target.name);
            }
        }

        // Überprüfen, ob zuvor hörbare Ziele nicht mehr hörbar sind
        foreach (var previousTarget in previouslyAudibleTargets)
        {
            if (!audibleTargets.Contains(previousTarget))
            {
                Debug.Log("Panzer hört Ziel nicht mehr: " + previousTarget.name);
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
