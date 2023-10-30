using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Events.Ermcon;

[HarmonyPatch]
public static class ErmconPatches
{
    private static readonly MethodInfo GetComponentOfIHandTarget = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), null, new[] { typeof(IHandTarget) });

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.Send))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> HandTargetPriority(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.SearchForward(i => i.Calls(GetComponentOfIHandTarget));
        if (matcher.IsValid)
            matcher.Set(OpCodes.Call, new Func<GameObject, IHandTarget>(GetTargetsSkipDisabled).Method);

        return matcher.InstructionEnumeration();
    }
    private static IHandTarget GetTargetsSkipDisabled(GameObject target)
    {
        return target.GetComponents<IHandTarget>()
            .Where(t => t is not MonoBehaviour m || m.enabled)
            .OrderBy(t => t is not MonoBehaviour) // put non-MonoBehaviours last
            .FirstOrDefault();
    }
}
