using SCHIZO.Unity.Materials;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Pickupable Creature Data")]
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
