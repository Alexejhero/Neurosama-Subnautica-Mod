using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VFXFabricating : MonoBehaviour
{
    public float localMinY;
    public float localMaxY;
    public Vector3 posOffset;
    public Vector3 eulerOffset;
    public float scaleFactor = 1;

    [SerializeField, HideInInspector] private Mesh _fabricatorMesh;
    [SerializeField, HideInInspector] private Transform _fabricatorPrefab;
    [SerializeField, HideInInspector] private string _spawnPointPath = "submarine_fabricator_01/printBed/spawnPoint";

    // ReSharper disable once Unity.RedundantEventFunction
    private void OnEnable() {}

#if UNITY_EDITOR
    private Transform _spawnPoint => _fabricatorPrefab.Find(_spawnPointPath).transform;

    private void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        if (Selection.activeGameObject != gameObject) return;

        Transform spawnPoint = _spawnPoint;

        Vector3 position = TransformPoint(Vector3.zero - posOffset, Quaternion.Inverse(Quaternion.Euler(-eulerOffset)) * spawnPoint.rotation, Vector3.one / scaleFactor, spawnPoint.position);
        Vector3 rotation = Quaternion.Inverse(spawnPoint.rotation).eulerAngles - eulerOffset;
        Vector3 scale = Vector3.one / scaleFactor;

        Gizmos.DrawWireMesh(_fabricatorMesh, position, Quaternion.Euler(rotation), scale);

        Gizmos.color = Color.blue;
        DrawLine(-posOffset + new Vector3(0, localMinY), -posOffset + new Vector3(0, localMaxY), 5);

        if (transform.position == Vector3.zero && transform.rotation == Quaternion.identity && transform.lossyScale == Vector3.one) return;

        Gizmos.color = Color.red;

        foreach (MeshFilter meshRenderer in GetComponentsInChildren<MeshFilter>().Where(m => m.sharedMesh))
        {
            DrawMesh(meshRenderer.sharedMesh, meshRenderer.transform);
        }

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.sharedMesh))
        {
            DrawMesh(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.transform);
        }
    }

    private void DrawMesh(Mesh mesh, Transform meshTransform)
    {
        Vector3 meshPosition = transform.InverseTransformPoint(meshTransform.position);
        Quaternion meshRotation = Quaternion.Inverse(transform.rotation) * meshTransform.rotation;
        Vector3 meshScale = new Vector3(meshTransform.lossyScale.x / transform.lossyScale.x,
            meshTransform.lossyScale.y / transform.lossyScale.y,
            meshTransform.lossyScale.z / transform.lossyScale.z);

        Gizmos.DrawWireMesh(mesh, meshPosition, meshRotation, meshScale);
    }

    private static Vector3 TransformPoint(Vector3 transformPosition, Quaternion transformRotation, Vector3 transformScale, Vector3 point)
    {
        Vector3 diference = (point - transformPosition);
        return Quaternion.Inverse(transformRotation) * Vector3.Scale(diference, -1 * transformScale);
    }

    private static void DrawLine(Vector3 p1, Vector3 p2, float width)
    {
        int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
        {
            Gizmos.DrawLine(p1, p2);
        }
        else
        {
            Camera c = Camera.current;
            if (c == null)
            {
                Debug.LogError("Camera.current is null");
                return;
            }
            var scp1 = c.WorldToScreenPoint(p1);
            var scp2 = c.WorldToScreenPoint(p2);

            Vector3 v1 = (scp2 - scp1).normalized; // line direction
            Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector

            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                Gizmos.DrawLine(origin, destiny);
            }
        }
    }
#endif
}
