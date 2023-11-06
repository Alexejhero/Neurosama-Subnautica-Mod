using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed partial class LoadingBackgroundCollection : ScriptableObject
    {
        [ListDrawerSettings, SerializeField, UsedImplicitly]
        private LoadingBackground[] backgrounds;
    }
}
