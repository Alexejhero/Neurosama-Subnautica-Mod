global using static SCHIZO.API.Global;
using System.Reflection;
using BepInEx.Logging;

namespace SCHIZO.API;

public static class Global
{
#if BELOWZERO
    public static bool IS_BELOWZERO => true;
#else
    public static bool IS_BELOWZERO => false;
#endif
    public static bool IS_SUBNAUTICA => !IS_BELOWZERO;

    internal static readonly ManualLogSource LOGGER = Logger.CreateLogSource("SCHIZO API");

    public static Assembly MAIN_ASSEMBLY;
}
