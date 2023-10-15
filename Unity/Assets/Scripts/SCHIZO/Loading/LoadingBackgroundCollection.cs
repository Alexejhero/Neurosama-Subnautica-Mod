using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed class LoadingBackgroundCollection : ScriptableObject
    {
        [ReorderableList]
        public LoadingBackground[] backgrounds;
    }
}
