using JetBrains.Annotations;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.ErmBed
{
    [UsedImplicitly]
    public sealed partial class ErmBedLoader : ItemLoader
    {
        [Required]
        public Texture2D blanketTexture;

        public override TriValidationResult AcceptsItem(ItemData item)
        {
            return item.classId.ToLower() == "ermbed" ? TriValidationResult.Valid
                : TriValidationResult.Error("ErmBedLoader only accepts an ErmBed");
        }
    }
}
