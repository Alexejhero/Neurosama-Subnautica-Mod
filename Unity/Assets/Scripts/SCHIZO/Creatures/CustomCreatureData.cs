using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Custom Creature Data")]
    public class CustomCreatureData : ScriptableObject
    {
        [Header("Databank Info")]
        public Sprite unlockSprite;
        public Texture2D databankTexture;
        public TextAsset databankText;

        [FormerlySerializedAs("sounds")] [Header("Creature Sounds")]
        public CreatureSoundData soundData;
    }
}
