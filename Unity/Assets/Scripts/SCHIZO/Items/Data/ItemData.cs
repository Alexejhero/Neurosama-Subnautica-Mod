using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Visual;
using SCHIZO.Enums.BelowZero;
using SCHIZO.Enums.Subnautica;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEngine;
using UnityEngine.Serialization;
using static NaughtyAttributes.EConditionOperator;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
    public partial class ItemData : ModRegistryItem
    {
        public GameObject prefab;

        [BoxGroup("TechType"), ValidateInput(nameof(AutoRegister_Validate)), Careful]
        public string classId;

        [BoxGroup("TechType"), ValidateInput(nameof(AutoRegister_Validate))]
        public string displayName;

        [BoxGroup("TechType"), ResizableTextArea, ShowIf(nameof(ShowPickupableProps))]
        public string tooltip;

        [BoxGroup("Common Properties"), ShowIf(nameof(ShowPickupableProps)), Required]
        public Sprite icon;

        [BoxGroup("Common Properties"), HideIf(Or, nameof(HidePickupableProps), nameof(isBuildable))]
        public Vector2Int itemSize = new Vector2Int(1, 1);

        [BoxGroup("Common Properties"), HideIf(Or, nameof(HidePickupableProps), nameof(isBuildable))]
        public bool isCraftable;

        [BoxGroup("Common Properties"), HideIf(Or, nameof(HidePickupableProps), nameof(isCraftable))]
        public bool isBuildable;

        [BoxGroup("Common Properties"), ShowIf(nameof(IsActuallyCraftable))]
        public float craftingTime = 2.5f;

        #region Subnautica Data

        [BoxGroup("Subnautica Data"), Label("Register"), SerializeField]
        public bool registerInSN = true;

        [BoxGroup("Subnautica Data"), Label("Recipe"), SerializeField, ShowIf(And, nameof(registerInSN), nameof(IsBuildableOrCraftable))]
        private Recipe recipeSN;

        [BoxGroup("Subnautica Data"), Label("Craft Tree"), ShowIf(And, nameof(registerInSN), nameof(IsActuallyCraftable)), SerializeField]
        private CraftTree_Type_SN craftTreeTypeSN = CraftTree_Type_SN.None;

        [BoxGroup("Subnautica Data"), Label("Craft Tree Path"), ShowIf(And, nameof(registerInSN), nameof(craftTreePathSN_ShowIf)), Dropdown(nameof(craftTreePathsSN)), SerializeField, UsedImplicitly]
        private string craftTreePathSN = "";

        [BoxGroup("Subnautica Data"), Label("Tech Group"), ValidateInput(nameof(techGroupSN_Validate)), SerializeField, ShowIf(And, nameof(registerInSN), nameof(IsBuildableOrCraftable))]
        private TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;

        [BoxGroup("Subnautica Data"), Label("Tech Category"), SerializeField, ShowIf(And, nameof(registerInSN), nameof(techCategorySN_ShowIf)), UsedImplicitly]
        private TechCategory_SN techCategorySN;

        [FormerlySerializedAs("databankInfoSN"), BoxGroup("Subnautica Data"), Label("PDA Ency Info"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInSN))]
        private PDAEncyclopediaInfo pdaEncyclopediaInfoSN;

        [BoxGroup("Subnautica Data"), Label("Known Tech Info"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInSN), nameof(ShowPickupableProps))]
        private KnownTechInfo knownTechInfoSN;

        [BoxGroup("Subnautica Data"), Label("Unlock At Start"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInSN), nameof(IsBuildableOrCraftable))]
        private bool unlockAtStartSN = true;

        [BoxGroup("Subnautica Data"), Label("Required For Unlock"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInSN), nameof(requiredForUnlockSN_ShowIf))]
        private Item requiredForUnlockSN;

        #endregion

        #region Below Zero Data

        [BoxGroup("Below Zero Data"), Label("Register"), SerializeField]
        public bool registerInBZ = true;

        [BoxGroup("Below Zero Data"), Label("Recipe"), SerializeField, ShowIf(And, nameof(registerInBZ), nameof(IsBuildableOrCraftable))]
        private Recipe recipeBZ;

        [BoxGroup("Below Zero Data"), Label("Craft Tree"), ShowIf(And, nameof(registerInBZ), nameof(IsActuallyCraftable)), SerializeField]
        private CraftTree_Type_BZ craftTreeTypeBZ = CraftTree_Type_BZ.None;

        [BoxGroup("Below Zero Data"), Label("Craft Tree Path"), ShowIf(And, nameof(registerInBZ), nameof(craftTreePathBZ_ShowIf)), Dropdown(nameof(craftTreePathsBZ)), SerializeField, UsedImplicitly]
        private string craftTreePathBZ = "";

        [BoxGroup("Below Zero Data"), Label("Tech Group"), ValidateInput(nameof(techGroupBZ_Validate)), SerializeField, ShowIf(And, nameof(registerInBZ), nameof(IsBuildableOrCraftable))]
        private TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;

        [BoxGroup("Below Zero Data"), Label("Tech Category"), SerializeField, ShowIf(And, nameof(registerInBZ), nameof(techCategoryBZ_ShowIf)), UsedImplicitly]
        private TechCategory_BZ techCategoryBZ;

        [FormerlySerializedAs("databankInfoBZ")] [BoxGroup("Below Zero Data"), Label("PDA Ency Info"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ))]
        private PDAEncyclopediaInfo pdaEncyclopediaInfoBZ;

        [BoxGroup("Below Zero Data"), Label("Known Tech Info"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInBZ), nameof(ShowPickupableProps))]
        private KnownTechInfo knownTechInfoBZ;

        [BoxGroup("Below Zero Data"), Label("Unlock At Start"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInBZ), nameof(IsBuildableOrCraftable))]
        private bool unlockAtStartBZ = true;

        [BoxGroup("Below Zero Data"), Label("Required For Unlock"), SerializeField, UsedImplicitly, ShowIf(And, nameof(registerInBZ), nameof(requiredForUnlockBZ_ShowIf))]
        private Item requiredForUnlockBZ;

        #endregion

        #region NaughtyAttributes stuff

        private bool AutoRegister_Validate(string str) => !string.IsNullOrWhiteSpace(str);

        private bool techGroupSN_Validate(TechGroup_SN val) => !isBuildable || val != TechGroup_SN.Uncategorized;
        private bool techGroupBZ_Validate(TechGroup_BZ val) => !isBuildable || val != TechGroup_BZ.Uncategorized;

        private bool techCategorySN_ShowIf() => techGroupSN != TechGroup_SN.Uncategorized && IsBuildableOrCraftable();
        private bool techCategoryBZ_ShowIf() => techGroupBZ != TechGroup_BZ.Uncategorized && IsBuildableOrCraftable();

        private bool craftTreePathSN_ShowIf() => IsActuallyCraftable() && craftTreeTypeSN != CraftTree_Type_SN.None;
        private bool craftTreePathBZ_ShowIf() => IsActuallyCraftable() && craftTreeTypeBZ != CraftTree_Type_BZ.None;

        private bool requiredForUnlockSN_ShowIf() => !unlockAtStartSN && IsBuildableOrCraftable();
        private bool requiredForUnlockBZ_ShowIf() => !unlockAtStartBZ && IsBuildableOrCraftable();

        // TODO: be smart amout this
        private readonly DropdownList<string> craftTreePathsSN = new DropdownList<string>
        {
            { "Basic Materials (Fabricator)", "Resources/BasicMaterials" },
            { "Advanced Materials (Fabricator)", "Resources/AdvancedMaterials" },
            { "Electronics (Fabricator)", "Resources/Electronics" },
            { "Water (Fabricator)", "Survival/Water" },
            { "Cooked Food (Fabricator)", "Survival/CookedFood" },
            { "Cured Food (Fabricator)", "Survival/CuredFood" },
            { "Equipment (Fabricator)", "Personal/Equipment" },
            { "Tools (Fabricator)", "Personal/Tools" },
            { "Deployables (Fabricator)", "Machines" },
            { "Vehicles (Mobile Vehicle Bay)", "Vehicles" },
            { "Rocket (Mobile Vehicle Bay)", "Rocket" },
            { "Common Modules (Vehicle Upgrade Console)", "CommonModules" },
            { "Seamoth Modules (Vehicle Upgrade Console)", "SeamothModules" },
            { "Prawn Suit Modules (Vehicle Upgrade Console)", "ExosuitModules" },
            { "Torpedoes (Vehicle Upgrade Console)", "Torpedoes" },
        };

        private readonly DropdownList<string> craftTreePathsBZ = new DropdownList<string>
        {
            { "Basic Materials (Fabricator)", "Resources/BasicMaterials" },
            { "Advanced Materials (Fabricator)", "Resources/AdvancedMaterials" },
            { "Electronics (Fabricator)", "Resources/Electronics" },
            { "Water (Fabricator)", "Survival/Water" },
            { "Cooked Food (Fabricator)", "Survival/CookedFood" },
            { "Cured Food (Fabricator)", "Survival/CuredFood" },
            { "Equipment (Fabricator)", "Personal/Equipment" },
            { "Tools (Fabricator)", "Personal/Tools" },
            { "Deployables (Fabricator)", "Machines" },
            { "Prawn Suit Upgraades (Fabricator)", "Upgrades/ExosuitUpgrades" },
            { "Seatruck Upgrades (Fabricator)", "Upgrades/SeatruckUpgrades" },
            { "Vehicles (Mobile Vehicle Bay)", "Vehicles" },
            { "Seatruck Modules (Mobile Vehicle Bay)", "Modules" },
            { "Prawn Suit Upgrades (Vehicle Upgrade Console)", "ExosuitModules" },
            { "Seatruck Upgrades (Vehicle Upgrade Console)", "SeaTruckUpgrade" },
        };

        protected virtual bool ShowPickupableProps() => true;
        private bool HidePickupableProps() => !ShowPickupableProps();

        private bool IsActuallyCraftable() => isCraftable && ShowPickupableProps();

        private bool IsBuildableOrCraftable() => isBuildable || IsActuallyCraftable();

        #endregion
    }
}
