using System;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [Serializable]
    public partial struct SpawnLocation
    {
        public Vector3 position;
        public Vector3 rotation;

        [UsedImplicitly]
        [Button("Copy Warp Command")]
        private readonly void CopyWarpCommand()
        {
            GUIUtility.systemCopyBuffer = $"warp {position.x} {position.y} {position.z}";
        }
    }
}
