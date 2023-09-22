using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(fileName = "LoadingBackgroundCollection", menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed class LoadingBackgroundCollection : ScriptableObject
    {
        [ReorderableList]
        public LoadingBackground[] backgrounds;
    }
}
