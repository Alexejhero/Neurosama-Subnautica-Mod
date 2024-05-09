namespace Immersion.Patches;

[HarmonyPatch]
public static class SurvivalAlertPatches
{
    // please note: while there *is* a mechanism to disable alerts in the base game
    /// (<see cref="GameModeManager.GetOption{TValue}(GameOption)"/>, specifically for <see cref="GameOption.ShowHungerAlerts"/> and <see cref="GameOption.ShowThirstAlerts"/>)
    // it disables achievements, and the patch to re-enable them would be longer

    public static bool EnableSurvivalAlerts { get; set; } = true;

    [HarmonyPatch(typeof(Survival), nameof(Survival.UpdateWarningSounds))]
    [HarmonyPrefix]
    private static bool SuppressSurvivalAlerts() => EnableSurvivalAlerts;
}
