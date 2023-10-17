using SCHIZO.Helpers;

namespace SCHIZO.Items.Data;

partial class CloneItemData
{
    public TechType CloneTarget => (TechType) RetargetHelpers.Pick(cloneTargetSN, cloneTargetBZ);
}
