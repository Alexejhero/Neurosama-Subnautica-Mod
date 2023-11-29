using JetBrains.Annotations;
using SCHIZO.Items.Data;
using TriInspector;

namespace SCHIZO.Items.Gymbag
{
    [UsedImplicitly]
    public sealed partial class GymbagLoader : ItemLoader
    {
        public override TriValidationResult AcceptsItem(ItemData item)
        {
            string classId = item.classId.ToLower();
            return classId == "gymbag" || classId == "quantumgymbag" ? TriValidationResult.Valid
                : TriValidationResult.Error("GymbagLoader only accepts a Gymbag or a QuantumGymbag");
        }
    }
}
