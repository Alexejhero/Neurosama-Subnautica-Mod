using SCHIZO.Unity.Materials;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [CreateAssetMenu(fileName = "PickupableCreatureData", menuName = "SCHIZO/Creatures/Pickupable Creature Data")]
    public class PickupableCreatureData : CustomCreatureData
    {
        [Header("Item Sprites")]
        public Sprite regularIcon;
        public Sprite cookedIcon;
        public Sprite curedIcon;

        [Header("Material Remapping")]
        public MaterialRemapOverride cookedRemap;
        public MaterialRemapOverride curedRemap;
    }
}
