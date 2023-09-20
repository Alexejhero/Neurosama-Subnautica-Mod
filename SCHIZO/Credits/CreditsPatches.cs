using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Credits;

[HarmonyPatch]
public static class CreditsPatches
{
    private static readonly Dictionary<string, string> _credits = new()
    {
        ["2Pfrog"] = "2D Art",
        ["AlexejheroDev"] = "Project Lead, Programming, Prefab Setup",
        ["Azit"] = "2D Art",
        ["baa14453"] = "Lore",
        ["budwheizzah"] = "Programming, Prefab Setup, 2D Art, Sounds",
        ["chrom"] = "2D Art",
        ["CJMAXiK"] = "2D Art, Sounds",
        ["darkeew"] = "Prefab Setup",
        ["FutabaKuuhaku"] = "3D Modeling",
        ["Govorunb"] = "Programming",
        ["greencap"] = "3D Modeling",
        ["Hakuhan"] = "2D Art",
        ["Kat"] = "3D Modeling",
        ["Kaz"] = "2D Art",
        ["Lorx"] = "2D Art",
        ["Moloch"] = "2D Art",
        ["MyBraza"] = "2D Art, Sounds",
        ["NetPlayz"] = "2D Art",
        ["P3R"] = "2D Art",
        ["paccha"] = "2D Art",
        ["Rune"] = "2D Art",
        ["SADecsSs"] = "2D Art",
        ["Sandro"] = "2D Art",
        ["SomeOldGuy"] = "2D Art",
        ["sugarph"] = "2D Art",
        ["Troobs"] = "2D Art",
        ["Vaalmyr"] = "3D Modeling",
        ["w1n7er"] = "3D Modeling, Animations",
        ["yamplum"] = "2D Art, Lore",
        ["YuG"] = "3D Modeling",
    };

    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.Start))]
    public static class UpdateCreditsTextTranspiler
    {
        private static readonly MethodInfo _target =
#if SUBNAUTICA
            AccessTools.Method(typeof(TMPro.TMP_Text), nameof(TMPro.TMP_Text.SetText), new[] { typeof(string), typeof(bool) });
#else
            AccessTools.Method(typeof(MonoBehaviour), nameof(MonoBehaviour.Invoke));
#endif

        [HarmonyTranspiler, UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(_target))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CreditsPatches), nameof(Patch)));
                }
            }
        }

        private static void Patch(EndCreditsManager __instance)
        {
#if SUBNAUTICA
            EasterEggPatches.easterEggAdjusted = false;

            float oldHeight = 14100;//__instance.textField.preferredHeight;
            __instance.textField.SetText(GetCreditsText() + __instance.textField.text);
            __instance.scrollSpeed = __instance.textField.preferredHeight * __instance.scrollSpeed / oldHeight;
            __instance.scrollStep = __instance.textField.preferredHeight * __instance.scrollStep / oldHeight;
#else
            LOGGER.LogWarning(__instance.centerText.text);

            float oldHeight = __instance.centerText.preferredHeight;
            __instance.centerText.SetText(GetCreditsText() + __instance.centerText.text);
            __instance.secondsUntilScrollComplete = __instance.centerText.preferredHeight * __instance.secondsUntilScrollComplete / oldHeight;
#endif
        }
    }

    private static string GetCreditsText()
    {
        StringBuilder builder = new($"<style=h1>Neuro-sama {(IS_SUBNAUTICA ? "Subnautica" : "Below Zero")} Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (KeyValuePair<string, string> credit in _credits)
        {
            builder.Append("<style=left>");
            builder.Append(credit.Key);
            builder.Append("</style>");
            builder.Append("<style=right>");
            builder.Append(credit.Value);
            builder.Append("</style>");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        return builder.ToString();
    }
}
