using SCHIZO.Unity.Retargeting.BelowZero;
using SCHIZO.Unity.Retargeting.Subnautica;

namespace SCHIZO;

public static class Retargeting
{
    public static class TechType
    {
        public const global::TechType Peeper = global::TechType.ArcticPeeper;
        public const global::TechType Bag = global::TechType.QuantumLocker;

        public static global::TechType From(TechType_BZ value) => (global::TechType) value;
        public static global::TechType From(TechType_BZ_NoObsolete value) => (global::TechType) value;
        public static global::TechType Pick(TechType_SN sn, TechType_BZ bz) => From(bz);
        public static global::TechType Pick(TechType_SN sn, TechType_BZ_NoObsolete bz) => From(bz);
    }

    public static class TechCategory
    {
        public const global::TechCategory CookedFood = global::TechCategory.FoodAndDrinks;
        public const global::TechCategory CuredFood = global::TechCategory.FoodAndDrinks;

        public static global::TechCategory From(TechCategory_BZ value) => (global::TechCategory) value;
        public static global::TechCategory Pick(TechCategory_SN sn, TechCategory_BZ bz) => From(bz);
    }

    public static class TechGroup
    {
        public static global::TechGroup From(TechGroup_BZ value) => (global::TechGroup) value;
        public static global::TechGroup Pick(TechGroup_SN sn, TechGroup_BZ bz) => From(bz);
    }
}
