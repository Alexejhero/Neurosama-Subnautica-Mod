using System;
using JetBrains.Annotations;
using SCHIZO.Items;
using SCHIZO.Items.Data;
using TriInspector;
using OurCreatureData = SCHIZO.Creatures.Data.CreatureData;

namespace SCHIZO.Creatures
{
    [UsedImplicitly]
    public partial class CreatureLoader : ItemLoader
    {
        public override TriValidationResult AcceptsItem(ItemData item)
        {
            if (!item) return TriValidationResult.Valid;

            Type type = item.GetType();

            if (type == typeof(OurCreatureData)) return TriValidationResult.Valid;
            if (type == typeof(ItemData)) return Invalid(GetType().Name, item.GetType().Name);

            return Unknown(GetType().Name, item.GetType().Name);
        }
    }
}
