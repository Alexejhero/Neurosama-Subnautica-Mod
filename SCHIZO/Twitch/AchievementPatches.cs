namespace SCHIZO.Twitch;

[HarmonyPatch]
public static class AchievementPatches
{
    [HarmonyPatch(typeof(GameAchievements), nameof(GameAchievements.Unlock))]
    [HarmonyPrefix]
    public static bool Prefix(GameAchievements.Id id)
    {
        PlatformUtils.main.GetServices().UnlockAchievement(id);
        return false;
    }
}
