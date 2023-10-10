using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity
{
    [CreateAssetMenu(menuName = "SCHIZO/Mesh Combiner")]
    public sealed class MeshCombiner : ScriptableObject
    {
#if UNITY
        [SerializeField, NaughtyAttributes.Required] private GameObject model;

        [SerializeField, NaughtyAttributes.ReorderableList] private MeshCollider[] meshColliders;
#endif

#if UNITY_EDITOR
        [NaughtyAttributes.Button("Generate Mesh Collider List")]
        private void GenerateMeshColliderList()
        {
            meshColliders = model.GetComponentsInChildren<MeshCollider>();
        }

        [NaughtyAttributes.Button("Generate Combined Mesh")]
        private void GenerateCombinedMesh()
        {
            if (!model) return;

            Mesh mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(UnityEditor.AssetDatabase.GetAssetPath(this));
            if (!mesh) UnityEditor.AssetDatabase.AddObjectToAsset(mesh = new Mesh(), this);

            mesh.Clear();

            mesh.name = $"Combined Mesh ({model.name})";

            CombineInstance[] combine = new CombineInstance[meshColliders.Length];
            for (int i = 0; i < meshColliders.Length; i++)
            {
                combine[i].mesh = meshColliders[i].sharedMesh;
                combine[i].transform = meshColliders[i].transform.localToWorldMatrix;
            }

            mesh.CombineMeshes(combine);

            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}
