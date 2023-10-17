using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Utilities
{
    partial class MeshCombiner
    {
        [SerializeField, Required] private GameObject model;
        [SerializeField, ReorderableList] private MeshCollider[] meshColliders;

#if UNITY_EDITOR
        [Button("Generate Mesh Collider List"), UsedImplicitly]
        private void GenerateMeshColliderList()
        {
            meshColliders = model.GetComponentsInChildren<MeshCollider>();
        }

        [Button("Generate Combined Mesh"), UsedImplicitly]
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

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
