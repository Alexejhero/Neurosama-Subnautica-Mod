using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemLoader : ItemLoader
    {
        public string spawnerClassId;
        [SerializeField]
        private GameObject spawnerPrefab;
        public override TriValidationResult AcceptsItem(ItemData item)
        {
            return item.classId.Equals("neurofumoitem", System.StringComparison.OrdinalIgnoreCase)
                ? TriValidationResult.Valid
                : TriValidationResult.Error("FumoItemLoader only accepts a NeuroFumoItem");
        }
    }
}
