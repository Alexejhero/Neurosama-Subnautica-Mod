using UnityEngine;

namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(fileName = "LoadingBackgroundCollection", menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed class LoadingBackgroundCollection : ScriptableObject
    {
        public LoadingBackground[] backgrounds;
    }
}
