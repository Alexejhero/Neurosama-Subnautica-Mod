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
    public DatabankInfo DatabankInfo => RetargetHelpers.Pick(databankInfoSN, databankInfoBZ);
}
