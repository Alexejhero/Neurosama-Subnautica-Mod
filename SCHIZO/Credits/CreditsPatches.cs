using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace SCHIZO.Credits;

[HarmonyPatch]
public static class CreditsPatches
{
    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.Start))]
    public static class UpdateCreditsTextTranspiler
    {
        private static readonly MethodInfo _target =
#if BELOWZERO
            AccessTools.Method(typeof(UnityEngine.MonoBehaviour), nameof(UnityEngine.MonoBehaviour.Invoke));
#else
            AccessTools.Method(typeof(TMPro.TMP_Text), nameof(TMPro.TMP_Text.SetText), new[] { typeof(string), typeof(bool) });
#endif

        [HarmonyTranspiler, UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);
            matcher.Start();
            matcher.SearchForward(instr => instr.Calls(_target));
            matcher.Advance(1);
            matcher.InsertAndAdvance
            (
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UpdateCreditsTextTranspiler), nameof(Patch)))
            );
            return matcher.InstructionEnumeration();
        }

        private static void Patch(EndCreditsManager __instance)
        {
            if (__instance.GetComponentInChildren<CreditsManager>() is { } creditsManager)
            {
#if BELOWZERO
                __instance.centerText.SetText(creditsManager.GetCreditsTextBZ() + __instance.centerText.text);
#else
                float oldHeight = 14100;//__instance.textField.preferredHeight;
                __instance.textField.SetText(creditsManager.GetCreditsTextSN() + __instance.textField.text);
                __instance.scrollSpeed = __instance.textField.preferredHeight * __instance.scrollSpeed / oldHeight;
                __instance.scrollStep = __instance.textField.preferredHeight * __instance.scrollStep / oldHeight;
#endif
            }

#if SUBNAUTICA
            if (__instance.GetComponentInChildren<EasterEggManager>() is { } easterEggManager)
            {
                easterEggManager.Reset();
            }
#endif
        }
    }
}
