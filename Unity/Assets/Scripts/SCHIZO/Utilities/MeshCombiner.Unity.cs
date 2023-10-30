using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

namespace SCHIZO.Utilities
{
    partial class MeshCombiner
    {
        [SerializeField, Required] private GameObject model;
        [SerializeField, ReorderableList] private MeshCollider[] meshColliders;
        [SerializeField] private SimplificationOptions simplificationOptions = SimplificationOptions.Default;
        [SerializeField, Range(0, 1)] private float quality = 0.1f;

        [SerializeField, HideInNormalInspector] private Mesh _combinedMesh;

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

            if (!_combinedMesh)
            {
                _combinedMesh = new Mesh();
                _combinedMesh.name = $"Combined Mesh ({model.name})";
                AssetDatabase.AddObjectToAsset(_combinedMesh, this);
            }

            Mesh mergeMesh = new Mesh();
            CombineInstance[] combine = new CombineInstance[meshColliders.Length];
            for (int i = 0; i < meshColliders.Length; i++)
            {
                combine[i].mesh = meshColliders[i].sharedMesh;
                combine[i].transform = meshColliders[i].transform.localToWorldMatrix;
            }
            mergeMesh.CombineMeshes(combine);

            MeshSimplifier simplifier = new MeshSimplifier(mergeMesh);
            simplifier.SimplificationOptions = simplificationOptions;
            simplifier.SimplifyMesh(quality);

            Mesh resultMesh = simplifier.ToMesh();
            CombineInstance[] move = new CombineInstance[]
            {
                new CombineInstance()
                {
                    mesh = resultMesh,
                    transform = Matrix4x4.identity
                }
            };
            _combinedMesh.Clear();
            _combinedMesh.CombineMeshes(move);

            EditorUtility.SetDirty(this);
        }
#endif
    }
}
