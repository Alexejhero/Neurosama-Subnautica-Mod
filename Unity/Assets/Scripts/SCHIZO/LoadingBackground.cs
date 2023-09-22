using UnityEngine;

namespace SCHIZO.Unity
{
    [CreateAssetMenu(fileName = "LoadingBackground", menuName = "SCHIZO/Loading Background")]
    public sealed class LoadingBackground : ScriptableObject
    {
        public Sprite art;
        public string credit;
        public string randomListId;
    }
}
