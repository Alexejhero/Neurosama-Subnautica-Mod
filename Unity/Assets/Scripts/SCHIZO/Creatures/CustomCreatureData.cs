using UnityEngine;

namespace SCHIZO.Unity
{
    [CreateAssetMenu(fileName = "CustomCreatureData", menuName = "SCHIZO/Creatures/Custom Creature Data")]
    public class CustomCreatureData : ScriptableObject
    {
        [Header("Databank Info")]
        public Sprite unlockSprite;
        public Texture2D databankTexture;
        public TextAsset databankText;
    }
}
