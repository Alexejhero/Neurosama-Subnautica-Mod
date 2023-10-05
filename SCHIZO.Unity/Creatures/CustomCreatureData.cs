using UnityEngine.Serialization;

namespace SCHIZO.Unity.Creatures;

[CreateAssetMenu(menuName = "SCHIZO/Creatures/Custom Creature Data")]
public class CustomCreatureData : ScriptableObject
{
    [BoxGroup("Creature Prefabs")][FormerlySerializedAs("prefab")] public GameObject regularPrefab;

    [BoxGroup("Databank Info")] public Sprite unlockSprite;
    [BoxGroup("Databank Info")] public Texture2D databankTexture;
    [BoxGroup("Databank Info")] public TextAsset databankText;

    [BoxGroup("Creature Sounds")][FormerlySerializedAs("sounds")] public CreatureSoundData soundData;
}
