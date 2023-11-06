using SCHIZO.Helpers;
using SCHIZO.Items.Data.Crafting;

namespace SCHIZO.Items.Data;

partial class ItemData
{
    public ModItem ModItem { get; set; }

    public Recipe Recipe => RetargetHelpers.Pick(recipeSN, recipeBZ);
    public CraftTree.Type CraftTreeType => (CraftTree.Type) RetargetHelpers.Pick(craftTreeTypeBZ, craftTreeTypeBZ);
    public string[] CraftTreePath => RetargetHelpers.Pick(craftTreePathSN, craftTreePathBZ).Split('/');
    public TechGroup TechGroup => (TechGroup) RetargetHelpers.Pick(techGroupSN, techGroupBZ);
    public TechCategory TechCategory => (TechCategory) RetargetHelpers.Pick(techCategorySN, techCategoryBZ);
    public KnownTechInfo KnownTechInfo => RetargetHelpers.Pick(knownTechInfoSN, knownTechInfoBZ);
    public bool UnlockAtStart => RetargetHelpers.Pick(unlockAtStartSN, unlockAtStartBZ);
    public TechType RequiredForUnlock => RetargetHelpers.Pick(requiredForUnlockSN, requiredForUnlockBZ).GetTechType();
#if BELOWZERO
    public TechData.SoundType SoundType => (TechData.SoundType) soundTypeBZ;
#endif

    protected override void Register()
    {
        ModItem.Create(this);
    }

    protected override void PostRegister()
    {
        UnityPrefab.CreateAndRegister(ModItem);
    }
#if UNITY_EDITOR
    [ContextMenu("Set Loader/Fumo Item")]
    private void CreateFumoItemLoader() => AssignItemLoader(CreateInstance<FumoItem.FumoItemLoader>());
#endif
}
