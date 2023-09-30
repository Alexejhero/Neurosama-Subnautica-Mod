global using Ingredient = CraftData.Ingredient;
using System.Diagnostics;
using ECCLibrary.Data;

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
    }

    [Conditional("SUBNAUTICA")]
    public static void WithoutInfection(this CreatureTemplate template)
    {
        template.CanBeInfected = false;
    }
}
