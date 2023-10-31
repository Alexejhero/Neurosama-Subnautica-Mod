using HarmonyLib;

namespace SCHIZO.Twitch;

[HarmonyPatch]
public static class AchievementPatches
{
    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.HasUsedConsole))]
    [HarmonyPrefix]
    public static bool Prefix(out bool __result)
    {
        return __result = false;
    }
}
