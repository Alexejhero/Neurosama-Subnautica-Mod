using System.Reflection;
using HarmonyLib;

namespace SCHIZO.Patches;

[HarmonyPatch]
internal static class ChangeNautilusExtraItemInfo
{
    [HarmonyTargetMethod]
    private static MethodBase TargetMethod() => AccessTools.Method("Nautilus.Patchers.TooltipPatcher:WriteModName");

    [HarmonyPrefix]
    private static void Prefix(ref string text)
    {
        text = text switch
        {
            "SCHIZO" => "Neuro-sama Mod",
            "BelowZero" => "Subnautica: Below Zero",
            _ => text,
        };
    }
}
