using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SCHIZO.Helpers;
using SCHIZO.Items.Components;
using UnityEngine;

namespace SCHIZO.Patches;

[HarmonyPatch]
internal static class AutoDestroyManualSpawns
{
    private static readonly MethodInfo NotifyCraftEnd = AccessTools.Method(typeof(CrafterLogic), nameof(CrafterLogic.NotifyCraftEnd));
    private static readonly MethodInfo UnityEngineDebugLogFormat = AccessTools.Method(typeof(Debug), nameof(Debug.LogFormat), [typeof(string), typeof(object[])]);

    [HarmonyTargetMethod]
    private static MethodBase TargetMethod() => AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(SpawnConsoleCommand), nameof(SpawnConsoleCommand.SpawnAsync)));

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        CodeMatcher matcher = new(instructions, generator);
        LocalBuilder lifetimeLocal = generator.DeclareLocal(typeof(float));

        matcher.MatchForward(false, new CodeMatch(ci => ci.Calls(UnityEngineDebugLogFormat)));
        if (!matcher.IsValid)
        {
            LOGGER.LogWarning("Could not patch SpawnConsoleCommand.SpawnAsync to auto destroy manual spawns (UnityEngine.Debug.LogFormat call not found)");
            return instructions;
        }
        // i know it's kind of lazy to insert our instructions in the middle of a method call
        // unfortunately there's a loop right afterwards so the alternative is even less elegant
        //matcher.Advance(1);
        // get lifetime from args
        // float lifetime = GetLifetime(this);
        matcher.Insert(
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(AutoDestroyManualSpawns), nameof(GetLifetime))),
            new(OpCodes.Stloc, lifetimeLocal)
        );

        // match:
        // CrafterLogic.NotifyCraftEnd(gameObject, this.<techType>5__2);
        matcher.MatchForward(false,
            new(OpCodes.Dup),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld),
            new(ci => ci.Calls(NotifyCraftEnd))
        );
        if (!matcher.IsValid)
        {
            LOGGER.LogWarning("Could not patch SpawnConsoleCommand.SpawnAsync to auto destroy manual spawns (CrafterLogic.NotifyCraftEnd call not found)");
            return instructions;
        }
        matcher.Insert(
            new(OpCodes.Dup),
            lifetimeLocal.LoadInstruction(),
            CodeInstruction.Call((GameObject go, float lifetime) => AddManualSpawnTag(go, lifetime))
        );

        return matcher.InstructionEnumeration();
    }

    private static void AddManualSpawnTag(GameObject obj, float lifetime)
    {
        if (lifetime is <= 0 or float.PositiveInfinity) return;

        float dieAt = Time.time + lifetime;
        LOGGER.LogDebug($"{obj.name} ({obj.GetInstanceID()}) is fated to die in {lifetime}s (at {dieAt})");
        DestroyAtTime destroy = obj.EnsureComponent<DestroyAtTime>();
        destroy.time = dieAt;

        // prevent unload/load since it would remove our manually-added component
        Component.Destroy(obj.GetComponent<LargeWorldEntity>());
    }

    private static float GetLifetime(object iEnumerator)
    {
        Hashtable args = GetArgs(iEnumerator);
        if (args.Count >= 4 && float.TryParse((string) args[3], out float lifetime))
        {
            return lifetime;
        }
        return float.PositiveInfinity;
    }

    private static Hashtable GetArgs(object iEnumerator)
    {
        NotificationCenter.Notification x = (NotificationCenter.Notification) AccessTools.Field(iEnumerator.GetType(), "n").GetValue(iEnumerator);
        return x.data;
    }
}
