

// ReSharper disable once CheckNamespace

using UnityEngine;

namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background")]
    public sealed class LoadingBackground : ScriptableObject
    {
        public Sprite art;
        public string credit;
        public string randomListId;
    }
}
