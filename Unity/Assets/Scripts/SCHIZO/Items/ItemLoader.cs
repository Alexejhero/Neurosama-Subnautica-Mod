using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Items.Data;
using TriInspector;
using OurCreatureData = SCHIZO.Creatures.Data.CreatureData;

namespace SCHIZO.Items
{
    public partial class ItemLoader
    {
        protected virtual HashSet<string> AllowedClassIds => [];
        protected virtual Type[] AllowedItemDataTypes => [typeof(ItemData)];
        public virtual TriValidationResult AcceptsItem(ItemData item)
        {
            if (!item) return TriValidationResult.Valid;

            Type myType = GetType();
            string myTypeName = myType.Name;
            if (AllowedClassIds.Count > 0 && !AllowedClassIds.Any(c => item.classId.Equals(c, StringComparison.OrdinalIgnoreCase)))
                return TriValidationResult.Error($"{myTypeName} only accepts Class Id {string.Join("/", AllowedClassIds)} so {item.classId} is not allowed");

            Type itemType = item.GetType();
            string itemTypeName = itemType.Name;

            return AllowedItemDataTypes.Contains(itemType)
                ? TriValidationResult.Valid
                : Invalid(myType, AllowedItemDataTypes, itemType);
        }

        protected static TriValidationResult Invalid(Type loaderType, Type[] acceptedDataTypes, Type itemDataType)
        {
            string names = string.Join("/", acceptedDataTypes.Select(t => t.Name));
            return TriValidationResult.Error($"{loaderType.Name} only accepts {names} so {itemDataType.Name} is not allowed");
        }
    }
}
