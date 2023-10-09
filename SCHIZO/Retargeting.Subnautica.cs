global using Ingredient = CraftData.Ingredient;
using System;
using SCHIZO.Unity.Retargeting.BelowZero;
using SCHIZO.Unity.Retargeting.Subnautica;

namespace SCHIZO;

public static class Retargeting
{
    public static class TechType
    {
        public const global::TechType Peeper = global::TechType.Peeper;
        public const global::TechType Bag = global::TechType.LuggageBag;
    }

    public static class TechCategory
    {
        public const global::TechCategory CookedFood = global::TechCategory.CookedFood;
        public const global::TechCategory CuredFood = global::TechCategory.CuredFood;

        public static global::TechCategory From(TechCategory_SN value) => (global::TechCategory) value;
        public static global::TechCategory Pick(TechCategory_SN sn, TechCategory_BZ bz) => From(sn);
    }

    public static class TechGroup
    {
        public static global::TechGroup From(TechGroup_SN value) => (global::TechGroup) value;
        public static global::TechGroup Pick(TechGroup_SN sn, TechGroup_BZ bz) => From(sn);
    }
}
