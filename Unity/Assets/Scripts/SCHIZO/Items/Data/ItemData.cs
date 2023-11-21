using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Interop.Subnautica.Enums.BelowZero;
using SCHIZO.Interop.Subnautica.Enums.Subnautica;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using SCHIZO.Sounds;
using SCHIZO.Spawns;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
    [DeclareBoxGroup("TechType")]
    [DeclareBoxGroup("common", Title = "Common Properties")]
    [DeclareBoxGroup("sn", Title = "Subnautica")]
    [DeclareBoxGroup("bz", Title = "Below Zero")]
    public partial class ItemData : ModRegistryItem
    {
        public GameObject prefab;

        [SerializeReference, ValidateInput(nameof(loader_Validate))]
        public ItemLoader loader = new();

        [Group("TechType"), Required, Careful]
        public string classId;

        [Group("TechType"), Required]
        public string displayName;

        [Group("TechType"), TextArea(1, 5), ShowIf(nameof(ShowPickupableProps))]
        public string tooltip;

        [CommonData, ShowIf(nameof(ShowPickupableProps)), Required]
        public Sprite icon;

        [CommonData, HideIf(nameof(HidePickupableProps)), HideIf(nameof(isBuildable)), Careful]
        public Vector2Int itemSize = new Vector2Int(1, 1);

        [CommonData, HideIf(nameof(HidePickupableProps)), HideIf(nameof(isBuildable)), Careful]
        public bool isCraftable;

        [CommonData, HideIf(nameof(HidePickupableProps)), HideIf(nameof(isCraftable)), Careful]
        public bool isBuildable;

        [CommonData, ShowIf(nameof(IsActuallyCraftable))]
        public float craftingTime = 2.5f;

        [CommonData, ShowIf(nameof(NonBuildableItemProperties_ShowIf))]
        public ItemSounds itemSounds;

        [CommonData]
        public PDAEncyclopediaInfo pdaEncyInfo;

        [CommonData, ShowIf(nameof(ShowPickupableProps)), FormerlySerializedAs("knownTechInfoSN")]
        public KnownTechInfo knownTechInfo;

        [CommonData]
        public BiomeSpawnData spawnData;

        [CommonData, UsedImplicitly, ShowIf(nameof(IsBuildableOrCraftable)), Careful, FormerlySerializedAs("unlockAtStartSN")]
        public bool unlockAtStart = true;

        #region Subnautica Data

        [SNData, LabelText("Register"), Careful, SerializeField]
        private bool registerInSN = true;

        [SNData, LabelText("Recipe"), SerializeField, ShowIf(nameof(registerInSN)), ShowIf(nameof(IsBuildableOrCraftable)), Careful]
        private Recipe recipeSN;

        [SNData, LabelText("Craft Tree"), ShowIf(nameof(registerInSN)), ShowIf(nameof(IsActuallyCraftable)), SerializeField]
        private CraftTree_Type_All craftTreeTypeSN = CraftTree_Type_All.None;

        [SNData, LabelText("Craft Tree Path"), ShowIf(nameof(registerInSN)), ShowIf(nameof(craftTreePathSN_ShowIf)), Dropdown(nameof(SNCraftTreePath)), SerializeField, UsedImplicitly]
        private string craftTreePathSN = "";

        [SNData, LabelText("Tech Group"), ValidateInput(nameof(techGroupSN_Validate)), SerializeField, ShowIf(nameof(registerInSN)), ShowIf(nameof(IsBuildableOrCraftable))]
        private TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;

        [SNData, LabelText("Tech Category"), SerializeField, ShowIf(nameof(registerInSN)), ShowIf(nameof(techCategorySN_ShowIf)), UsedImplicitly]
        private TechCategory_SN techCategorySN;

        [SNData, LabelText("Required For Unlock"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInSN)), ShowIf(nameof(requiredForUnlock_ShowIf)), Careful]
        private Item requiredForUnlockSN;

        [SNData, LabelText("Equipment Type"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInSN)), ShowIf(nameof(NonBuildableItemProperties_ShowIf)), HideIf(nameof(isBuildable)), Careful]
        private EquipmentType_All equipmentTypeSN;

        [SNData, LabelText("Quick Slot Type"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInSN)), ShowIf(nameof(NonBuildableItemProperties_ShowIf)), ShowIf(nameof(equipmentTypeSN), EquipmentType_All.Hand)]
        private QuickSlotType_All quickSlotTypeSN;

        #endregion

        #region Below Zero Data

        [BZData, LabelText("Register"), Careful, SerializeField]
        private bool registerInBZ = true;

        [BZData, LabelText("Recipe"), SerializeField, ShowIf(nameof(registerInBZ)), ShowIf(nameof(IsBuildableOrCraftable)), Careful]
        private Recipe recipeBZ;

        [BZData, LabelText("Can Be Recycled"), ShowIf(nameof(registerInBZ)), ShowIf(nameof(IsActuallyCraftable))]
        public bool canBeRecycledBZ = true;

        [BZData, LabelText("Craft Tree"), ShowIf(nameof(registerInBZ)), ShowIf(nameof(IsActuallyCraftable)), SerializeField]
        private CraftTree_Type_All craftTreeTypeBZ = CraftTree_Type_All.None;

        [BZData, LabelText("Craft Tree Path"), ShowIf(nameof(registerInBZ)), ShowIf(nameof(craftTreePathBZ_ShowIf)), SerializeField, UsedImplicitly, Dropdown(nameof(BZCraftTreePath))]
        private string craftTreePathBZ = "";

        [BZData, LabelText("Tech Group"), ValidateInput(nameof(techGroupBZ_Validate)), SerializeField, ShowIf(nameof(registerInBZ)), ShowIf(nameof(IsBuildableOrCraftable))]
        private TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;

        [BZData, LabelText("Tech Category"), SerializeField, ShowIf(nameof(registerInBZ)), ShowIf(nameof(techCategoryBZ_ShowIf)), UsedImplicitly]
        private TechCategory_BZ techCategoryBZ;

        [BZData, LabelText("Sound Type"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ)), ShowIf(nameof(NonBuildableItemProperties_ShowIf))]
        private TechData_SoundType_BZ soundTypeBZ;

        [BZData, LabelText("Required For Unlock"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ)), ShowIf(nameof(requiredForUnlock_ShowIf)), Careful]
        private Item requiredForUnlockBZ;

        [BZData, LabelText("Equipment Type"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ)), ShowIf(nameof(NonBuildableItemProperties_ShowIf)), Careful]
        private EquipmentType_All equipmentTypeBZ;

        [BZData, LabelText("Quick Slot Type"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ)), ShowIf(nameof(NonBuildableItemProperties_ShowIf)), ShowIf(nameof(equipmentTypeBZ), EquipmentType_All.Hand)]
        private QuickSlotType_All quickSlotTypeBZ;

        [BZData, ShowIf(nameof(registerInBZ)), ShowIf(nameof(NonBuildableItemProperties_ShowIf)), ShowIf(nameof(equipmentTypeBZ), EquipmentType_All.Hand), Range(0, 100)]
        public int coldResistanceBZ;

        #endregion

        #region TriInspector stuff

        private TriValidationResult loader_Validate()
        {
            if (loader == null) return TriValidationResult.Error("Loader is required!");
            return loader.AcceptsItem(this);
        }

        private TriDropdownList<string> SNCraftTreePath()
        {
            switch (craftTreeTypeSN)
            {
                case CraftTree_Type_All.Fabricator:
                    return new TriDropdownList<string>()
                        {
                            {"<root>", ""},
                            {"Resources/<self>", "Resources"},
                            {"Resources/Basic materials", "Resources/BasicMaterials"},
                            {"Resources/Advanced materials", "Resources/AdvancedMaterials"},
                            {"Resources/Electronics", "Resources/Electronics"},
                            {"Survival/<self>", "Survival"},
                            {"Survival/Water", "Survival/Water"},
                            {"Survival/Cooked food", "Survival/CookedFood"},
                            {"Survival/Cured food", "Survival/CuredFood"},
                            {"Personal/<self>", "Personal"},
                            {"Personal/Equipment", "Personal/Equipment"},
                            {"Personal/Tools", "Personal/Tools"},
                            {"Personal/Deployables", "Machines"},
                        };

                case CraftTree_Type_All.Constructor:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                        {"Vehicles", "Vehicles"},
                        {"Rocket", "Rocket"},
                    };

                case CraftTree_Type_All.SeamothUpgrades:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                        {"Common Modules", "CommonModules"},
                        {"Seamoth Modules", "SeamothModules"},
                        {"Prawn Suit Modules", "ExosuitModules"},
                        {"Torpedoes", "Torpedoes"},
                    };

                default:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                    };
            }
        }

        private TriDropdownList<string> BZCraftTreePath()
        {
            switch (craftTreeTypeBZ)
            {
                case CraftTree_Type_All.Fabricator:
                case CraftTree_Type_All.SeaTruckFabricator:
                    return new TriDropdownList<string>()
                        {
                            {"<root>", ""},
                            {"Resources/<self>", "Resources"},
                            {"Resources/Basic materials", "Resources/BasicMaterials"},
                            {"Resources/Advanced materials", "Resources/AdvancedMaterials"},
                            {"Resources/Electronics", "Resources/Electronics"},
                            {"Survival/<self>", "Survival"},
                            {"Survival/Water", "Survival/Water"},
                            {"Survival/Cooked food", "Survival/CookedFood"},
                            {"Survival/Cured food", "Survival/CuredFood"},
                            {"Personal/<self>", "Personal"},
                            {"Personal/Equipment", "Personal/Equipment"},
                            {"Personal/Tools", "Personal/Tools"},
                            {"Personal/Deployables", "Machines"},
                            {"Upgrades/<self>", "Upgrades"},
                            {"Upgrades/Prawn Suit Upgrades", "Upgrades/ExosuitUpgrades"},
                            {"Upgrades/Seatruck Upgrades", "Upgrades/SeatruckUpgrades"},
                        };

                case CraftTree_Type_All.Constructor:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                        {"Vehicles", "Vehicles"},
                        {"Modules", "Modules"},
                    };

                case CraftTree_Type_All.SeamothUpgrades:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                        {"Prawn Suit Upgrades", "ExosuitModules"},
                        {"Seatruck Upgrades", "SeaTruckUpgrade"},
                    };

                default:
                    return new TriDropdownList<string>()
                    {
                        {"<root>", ""},
                    };
            }
        }

        private void OnValidate()
        {
            if (!SNCraftTreePath().Select(i => i.Value).Contains(craftTreePathSN)) craftTreePathSN = "";
            if (!SNCraftTreePath().Select(i => i.Value).Contains(craftTreePathSN)) craftTreePathSN = "";
        }

        private TriValidationResult techGroupSN_Validate()
        {
            if (isBuildable && techGroupSN == TechGroup_SN.Uncategorized) return TriValidationResult.Error("Tech group cannot be Uncategorized if the item is buildable");
            return TriValidationResult.Valid;
        }

        private TriValidationResult techGroupBZ_Validate()
        {
            if (isBuildable && techGroupBZ == TechGroup_BZ.Uncategorized) return TriValidationResult.Error("Tech group cannot be Uncategorized if the item is buildable");
            return TriValidationResult.Valid;
        }

        private bool techCategorySN_ShowIf() => techGroupSN != TechGroup_SN.Uncategorized && IsBuildableOrCraftable();
        private bool techCategoryBZ_ShowIf() => techGroupBZ != TechGroup_BZ.Uncategorized && IsBuildableOrCraftable();

        private bool craftTreePathSN_ShowIf() => IsActuallyCraftable() && craftTreeTypeSN != CraftTree_Type_All.None;
        private bool craftTreePathBZ_ShowIf() => IsActuallyCraftable() && craftTreeTypeBZ != CraftTree_Type_All.None;

        private bool requiredForUnlock_ShowIf() => !unlockAtStart && IsBuildableOrCraftable();

        private bool NonBuildableItemProperties_ShowIf() => ShowPickupableProps() && !isBuildable;

        protected virtual bool ShowPickupableProps() => true;
        private bool HidePickupableProps() => !ShowPickupableProps();

        private bool IsActuallyCraftable() => isCraftable && ShowPickupableProps();

        private bool IsBuildableOrCraftable() => isBuildable || IsActuallyCraftable();

        private protected sealed class CommonData : GroupAttribute
        {
            public CommonData() : base("common")
            {
            }
        }

        private protected class SNData : GroupAttribute
        {
            public SNData() : base("sn")
            {
            }
        }

        private protected class BZData : GroupAttribute
        {
            public BZData() : base("bz")
            {
            }
        }

        #endregion
    }
}
