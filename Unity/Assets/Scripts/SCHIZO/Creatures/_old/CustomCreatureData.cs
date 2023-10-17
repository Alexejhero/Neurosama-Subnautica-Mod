using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Creatures._old
{
    // [CreateAssetMenu(menuName = "SCHIZO/Creatures/Custom Creature Data")]
    [Obsolete]
    public class CustomCreatureData : ScriptableObject
    {
        [BoxGroup("Creature Prefabs")][FormerlySerializedAs("prefab")] public GameObject regularPrefab;

        [BoxGroup("Databank Info")] public Sprite unlockSprite;
        [BoxGroup("Databank Info")] public Texture2D databankTexture;
        [BoxGroup("Databank Info")] public TextAsset databankText;

        [BoxGroup("Creature Sounds")][FormerlySerializedAs("sounds")] public CreatureSoundData soundData;

        [BoxGroup("Creature Data")] public bool AcidImmune = true;
        [BoxGroup("Creature Data")] public float BioReactorCharge = 0;
    }
}
