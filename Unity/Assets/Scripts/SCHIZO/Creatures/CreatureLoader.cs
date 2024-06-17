using System;
using JetBrains.Annotations;
using SCHIZO.Items;
using OurCreatureData = SCHIZO.Creatures.Data.CreatureData;

namespace SCHIZO.Creatures
{
    [UsedImplicitly]
    public partial class CreatureLoader : ItemLoader
    {
        protected override Type[] AllowedItemDataTypes => [typeof(OurCreatureData)];
    }
}
