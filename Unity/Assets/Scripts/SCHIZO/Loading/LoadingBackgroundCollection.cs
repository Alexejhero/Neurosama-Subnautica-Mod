using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed partial class LoadingBackgroundCollection : ScriptableObject
    {
        [ReorderableList, SerializeField, UsedImplicitly]
        private LoadingBackground[] backgrounds;
    }
}
