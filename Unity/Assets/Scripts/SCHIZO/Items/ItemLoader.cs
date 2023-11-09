using System;
using SCHIZO.Items.Data;
using TriInspector;
using OurCreatureData = SCHIZO.Creatures.Data.CreatureData;

namespace SCHIZO.Items
{
    public partial class ItemLoader
    {
        public virtual TriValidationResult AcceptsItem(ItemData item)
        {
            if (!item) return TriValidationResult.Valid;

            Type type = item.GetType();

            if (type == typeof(OurCreatureData)) return Invalid(GetType().Name, item.GetType().Name);
            if (type == typeof(ItemData)) return TriValidationResult.Valid;

            return Unknown(GetType().Name, item.GetType().Name);
        }

        protected static TriValidationResult Unknown(string loaderType, string itemDataType)
        {
            return TriValidationResult.Warning($"{loaderType} has not explicitly defined if it accepts {itemDataType}!");
        }

        protected static TriValidationResult Invalid(string loaderType, string itemDataType)
        {
            return TriValidationResult.Error($"{loaderType} does not accept {itemDataType}!");
        }
    }
}
