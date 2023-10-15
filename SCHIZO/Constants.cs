#if SUBNAUTICA
global using NIngredient = CraftData.Ingredient;
#else
global using NIngredient = Ingredient;
#endif

global using static SCHIZO.Constants;
using SCHIZO.Utilities;

namespace SCHIZO;

public static class Constants
{
#if BELOWZERO
    public static bool IS_SUBNAUTICA => false;
    public static bool IS_BELOWZERO => true;
    public static Game GAME => Game.BelowZero;
#else
    public static bool IS_SUBNAUTICA => true;
    public static bool IS_BELOWZERO => false;
    public static Game GAME => Game.Subnautica;
#endif
}
