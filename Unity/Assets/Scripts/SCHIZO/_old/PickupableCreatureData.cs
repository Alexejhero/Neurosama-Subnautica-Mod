using System;
using NaughtyAttributes;
using SCHIZO.Materials;
using UnityEngine;

namespace SCHIZO._old
{
    // [CreateAssetMenu(menuName = "SCHIZO/Creatures/Pickupable Creature Data")]
    [Obsolete]
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
