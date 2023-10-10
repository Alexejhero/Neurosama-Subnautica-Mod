using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background")]
    public sealed class LoadingBackground : ScriptableObject
    {
        public Sprite art;
        public string credit;
        public string randomListId;
        public Game game = Game.Subnautica | Game.BelowZero;
    }
}
