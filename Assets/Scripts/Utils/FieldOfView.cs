using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FieldOfView : MonoBehaviour
{
    [Header("Targets & Layers")]
    public Transform player;
    public LayerMask obstacleMask;   // assign Obstacles layer here

    [Header("FOV")]
    public float viewRadius = 10f;
    [Range(0, 360f)] public float viewAngle = 90f;

    [Header("Debug")]
    public bool drawGizmos = true;
    public bool canSeePlayer { get; private set; }

    void Update()
    {
        canSeePlayer = CanSee(player);
    }

    public bool CanSee(Transform target)
    {
        if (!target) return false;

        Vector3 eye = transform.position + Vector3.up * 1.6f;
        Vector3 tgt = target.position + Vector3.up * 1.6f;

        // 1) FOV checks
        Vector3 toTarget = tgt - eye;
        if (toTarget.sqrMagnitude > viewRadius * viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > viewAngle * 0.5f) return false;

        // 2) LOS blocked by obstacles?
        // Only cast against obstacles; ignore ground and characters entirely.
        if (Physics.Linecast(eye, tgt, out RaycastHit hit, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            // Hit an obstacle -> blocked
            return false;
        }

        // Nothing in the way -> visible
        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.cyan;

        // edges of FOV
        Vector3 left = DirFromAngle(-viewAngle * 0.5f);
        Vector3 right = DirFromAngle(viewAngle * 0.5f);
        Gizmos.DrawRay(transform.position, left * viewRadius);
        Gizmos.DrawRay(transform.position, right * viewRadius);

        // LOS line when player is assigned
        if (player)
        {
            Gizmos.color = canSeePlayer ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * 1.6f,
                            player.position + Vector3.up * 1.6f);
        }
    }

    Vector3 DirFromAngle(float angleDeg)
    {
        return Quaternion.Euler(0f, angleDeg, 0f) * transform.forward;
    }
}