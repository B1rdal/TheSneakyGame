using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    public FieldOfView fov;           // reference to your FOV script
    public int rayCount = 7;          // number of rays across the cone
    public float meshHeight = 0.1f;   // thickness above ground

    Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        if (!fov) return;

        // Fan shape points
        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // cone origin (local space)

        float halfAngle = fov.viewAngle * 0.5f;
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -halfAngle + (fov.viewAngle / rayCount) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * fov.transform.forward;

            Vector3 endpoint = fov.transform.position + dir * fov.viewRadius;

            // Raycast to check obstacles
            if (Physics.Raycast(fov.transform.position + Vector3.up * 1.6f, dir, out RaycastHit hit, fov.viewRadius, ~0))
            {
                if (((1 << hit.collider.gameObject.layer) & fov.obstacleMask.value) != 0)
                    endpoint = hit.point; // obstacle blocked
            }

            // convert to local space
            Vector3 localPoint = fov.transform.InverseTransformPoint(endpoint);
            localPoint.y = meshHeight;
            vertices[i + 1] = localPoint;
        }

        for (int i = 0; i < rayCount; i++)
        {
            int v0 = 0;
            int v1 = i + 1;
            int v2 = i + 2;
            triangles[i * 3] = v0;
            triangles[i * 3 + 1] = v1;
            triangles[i * 3 + 2] = v2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
