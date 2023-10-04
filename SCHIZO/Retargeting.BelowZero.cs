using System.Diagnostics;
using ECCLibrary.Data;

namespace SCHIZO;

public static class Retargeting
{
    public static class TechType
    {
        public const global::TechType Peeper = global::TechType.ArcticPeeper;
        public const global::TechType Bag = global::TechType.QuantumLocker;
    }

    public static class TechCategory
    {
        public const global::TechCategory CookedFood = global::TechCategory.FoodAndDrinks;
        public const global::TechCategory CuredFood = global::TechCategory.FoodAndDrinks;
    }
}
