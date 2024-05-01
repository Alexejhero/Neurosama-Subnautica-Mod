using System.Reflection;
using System.Reflection.Emit;
using Immersion.Trackers;

namespace Immersion.Patches;

[HarmonyPatch]
public static class LowOxygenAlertPatches
{
    public static bool PatchSucceeded { get; private set; }

    [HarmonyPatch(typeof(LowOxygenAlert), nameof(LowOxygenAlert.Update))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);

        FieldInfo notifField = typeof(LowOxygenAlert.Alert).GetField(nameof(LowOxygenAlert.Alert.notification));
        MethodInfo playMethod = typeof(PDANotification).GetMethod(nameof(PDANotification.Play), []);
        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Ldfld, notifField),
            new CodeMatch(OpCodes.Callvirt, playMethod)
        );
        if (!matcher.IsValid)
        {
            LOGGER.LogWarning($"Could not patch {nameof(LowOxygenAlert)}.{nameof(LowOxygenAlert.Update)}, {nameof(OxygenAlerts)} will not function");
            return instructions;
        }
        matcher.Advance(1);
        matcher.RemoveInstructions(2);
        matcher.Insert(
            CodeInstruction.Call((LowOxygenAlert.Alert alert) => PlayAlert(alert))
        );

        PatchSucceeded = true;
        LOGGER.LogDebug($"Oxygen alerts patched for {nameof(OxygenAlerts)}");
        return matcher.InstructionEnumeration();
    }

    private static void PlayAlert(LowOxygenAlert.Alert alert)
    {
        if (alert is OxygenAlerts.Alert ourAlert)
            ourAlert.Play();
        else
            alert.notification.Play();
    }
}
