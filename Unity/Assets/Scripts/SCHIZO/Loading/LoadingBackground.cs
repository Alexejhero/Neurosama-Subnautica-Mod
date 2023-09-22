using UnityEngine;

namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(fileName = "LoadingBackground", menuName = "SCHIZO/Loading/Loading Background")]
    public sealed class LoadingBackground : ScriptableObject
    {
        public Sprite art;
        public string credit;
        public string randomListId;
    }
}
