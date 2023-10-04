using NaughtyAttributes;
using SCHIZO.Unity.Materials;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Pickupable Creature Data")]
    public class PickupableCreatureData : CustomCreatureData
    {
        [BoxGroup("Creature Prefabs")] public GameObject cookedPrefab;
        [BoxGroup("Creature Prefabs")] public GameObject curedPrefab;

        [BoxGroup("Item Sprites")] public Sprite regularIcon;
        [BoxGroup("Item Sprites")] public Sprite cookedIcon;
        [BoxGroup("Item Sprites")] public Sprite curedIcon;

        [BoxGroup("Material Remapping")] public MaterialRemapOverride cookedRemap;
        [BoxGroup("Material Remapping")] public MaterialRemapOverride curedRemap;
    }
}
