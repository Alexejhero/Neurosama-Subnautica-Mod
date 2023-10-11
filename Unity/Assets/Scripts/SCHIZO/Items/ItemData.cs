using NaughtyAttributes;
using SCHIZO.Unity.Retargeting.BelowZero;
using SCHIZO.Unity.Retargeting.Subnautica;
using SCHIZO.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
    public sealed class ItemData : ScriptableObject
    {
        public bool autoRegister = false;

        #region Tomfoolery to work around HorizontalLine limitations
#if UNITY
        [HorizontalLine(2, EColor.Green), ShowIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab1;

        [HorizontalLine(2, EColor.Red), HideIf(nameof(autoRegister))]
        [SerializeField, Label("Prefab")] private GameObject _prefab2;

        private void OnValidate_prefab()
        {
            if (_prefab1 == _prefab2 && prefab != _prefab1) _prefab1 = _prefab2 = prefab;
            if (prefab == _prefab2 && _prefab1 != prefab) prefab = _prefab2 = _prefab1;
            if (prefab == _prefab1 && _prefab2 != prefab) prefab = _prefab1 = _prefab2;
        }
#endif
        #endregion

        [HideInInspector] public GameObject prefab;
        public BaseSoundCollection sounds;

        [BoxGroup("TechType"), ValidateInput(nameof(AutoRegister_Validate))]
        public string classId;

        [BoxGroup("TechType"), ValidateInput(nameof(AutoRegister_Validate))]
        public string displayName;

        [BoxGroup("TechType"), ResizableTextArea]
        public string tooltip;

        [BoxGroup("Common Properties")]
        public Sprite icon;

        [BoxGroup("Common Properties"), HideIf(nameof(isBuildable))]
        public Vector2Int itemSize = new Vector2Int(1, 1);

        [BoxGroup("Common Properties"), HideIf(nameof(isBuildable))]
        public bool isCraftable = false;

        [BoxGroup("Common Properties"), HideIf(nameof(isCraftable))]
        public bool isBuildable = false;

        [BoxGroup("Common Properties"), ShowIf(nameof(isCraftable))]
        public float craftingTime = 2.5f;

        [BoxGroup("Subnautica Data"), Label("Recipe"), SerializeField]
        private Recipe recipeSN;

        [BoxGroup("Subnautica Data"), Label("Craft Tree"), ShowIf(nameof(isCraftable)), SerializeField]
        private CraftTree_Type_SN craftTreeTypeSN = CraftTree_Type_SN.None;

        [BoxGroup("Subnautica Data"), Label("Craft Tree Path"), ShowIf(nameof(craftTreePathSN_ShowIf)), Dropdown(nameof(craftTreePathsSN)), SerializeField]
        private string craftTreePathSN = "";

        [BoxGroup("Subnautica Data"), Label("Tech Group"), ValidateInput(nameof(techGroupSN_Validate)), SerializeField]
        private TechGroup_SN techGroupSN = TechGroup_SN.Uncategorized;

        [BoxGroup("Subnautica Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategorySN_HideIf))]
        private TechCategory_SN techCategorySN;

        [BoxGroup("Below Zero Data"), Label("Recipe"), SerializeField]
        private Recipe recipeBZ;

        [BoxGroup("Below Zero Data"), Label("Craft Tree"), ShowIf(nameof(isCraftable)), SerializeField]
        private CraftTree_Type_BZ craftTreeTypeBZ = CraftTree_Type_BZ.None;

        [BoxGroup("Below Zero Data"), Label("Craft Tree Path"), ShowIf(nameof(craftTreePathBZ_ShowIf)), Dropdown(nameof(craftTreePathsBZ)), SerializeField]
        private string craftTreePathBZ = "";

        [BoxGroup("Below Zero Data"), Label("Tech Group"), ValidateInput(nameof(techGroupBZ_Validate)), SerializeField]
        private TechGroup_BZ techGroupBZ = TechGroup_BZ.Uncategorized;

        [BoxGroup("Below Zero Data"), Label("Tech Category"), SerializeField, HideIf(nameof(techCategoryBZ_HideIf))]
        private TechCategory_BZ techCategoryBZ;

#if !UNITY
        public SCHIZO.Items.ModItem ModItem { get; set; }

        public Recipe Recipe => Helpers.RetargetHelpers.Pick(recipeSN, recipeBZ);
        public CraftTree.Type CraftTreeType => (CraftTree.Type) Helpers.RetargetHelpers.Pick(craftTreeTypeBZ, craftTreeTypeBZ);
        public string[] CraftTreePath => Helpers.RetargetHelpers.Pick(craftTreePathSN, craftTreePathBZ).Split('/');
        public TechGroup TechGroup => (TechGroup) Helpers.RetargetHelpers.Pick(techGroupSN, techGroupBZ);
        public TechCategory TechCategory => (TechCategory) Helpers.RetargetHelpers.Pick(techCategorySN, techCategoryBZ);
#endif

        #region Unity editor stuff

#if UNITY
        public Recipe RecipeSN => recipeSN;
        public Recipe RecipeBZ => recipeBZ;
#endif

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnValidate_prefab();

            Utilities.DoNotExpose doNotExpose = UnityEditor.AssetDatabase.LoadAssetAtPath<Utilities.DoNotExpose>(UnityEditor.AssetDatabase.GetAssetPath(this));
            if (autoRegister && !doNotExpose)
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(CreateInstance<Utilities.DoNotExpose>(), this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            else if (!autoRegister && doNotExpose)
            {
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(doNotExpose);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
#endif

        #endregion

        #region NaughtyAttributes stuff

        private bool AutoRegister_Validate(string str) => !autoRegister || !string.IsNullOrWhiteSpace(str);

        private bool techGroupSN_Validate(TechGroup_SN val) => !autoRegister || !isBuildable || val != TechGroup_SN.Uncategorized;
        private bool techGroupBZ_Validate(TechGroup_BZ val) => !autoRegister || !isBuildable || val != TechGroup_BZ.Uncategorized;

        private bool techCategorySN_HideIf() => techGroupSN == TechGroup_SN.Uncategorized;
        private bool techCategoryBZ_HideIf() => techGroupBZ == TechGroup_BZ.Uncategorized;

        private bool craftTreePathSN_ShowIf() => isCraftable && craftTreeTypeSN != CraftTree_Type_SN.None;
        private bool craftTreePathBZ_ShowIf() => isCraftable && craftTreeTypeBZ != CraftTree_Type_BZ.None;

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

        #endregion
    }
}
