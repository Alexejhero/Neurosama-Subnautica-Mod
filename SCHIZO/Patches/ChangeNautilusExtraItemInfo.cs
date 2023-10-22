using System;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace SCHIZO.Patches;

[HarmonyPatch]
internal static class ChangeNautilusExtraItemInfo
{
    private static readonly Assembly _ourAssembly = typeof(ChangeNautilusExtraItemInfo).Assembly;

    private static readonly Action<StringBuilder, string> WriteModName = AccessTools.MethodDelegate<Action<StringBuilder, string>>(AccessTools.Method("Nautilus.Patchers.TooltipPatcher:WriteModName"));

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
