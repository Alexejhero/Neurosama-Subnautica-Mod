using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.HullPlates
{
    [CreateAssetMenu(fileName = "HullPlateCollection", menuName = "SCHIZO/Hull Plates/Hull Plate Collection")]
    public sealed class HullPlateCollection : ScriptableObject
    {
        public Texture2D hiddenIcon;
        public Texture2D deprecatedTexture;
        [ReorderableList]
        public HullPlate[] hullPlates;
        [ReorderableList]
        public HullPlate[] deprecatedHullPlates;
    }
}
