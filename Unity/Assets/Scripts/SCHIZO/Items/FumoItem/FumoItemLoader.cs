using SCHIZO.Items.Data;
using SCHIZO.Spawns;
using TriInspector;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemLoader : ItemLoader
    {
        [InfoBox("This is LOCAL to the drop pod prefab since it lands in a random location")]
        public SpawnLocation spawn;
        public override TriValidationResult AcceptsItem(ItemData item)
        {
            return item.classId.ToLower() == "neurofumoitem"
                ? TriValidationResult.Valid
                : TriValidationResult.Error("FumoItemLoader only accepts a NeuroFumoItem");
        }
    }
}
