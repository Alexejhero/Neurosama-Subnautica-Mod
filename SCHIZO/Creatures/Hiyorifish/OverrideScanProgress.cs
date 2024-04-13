using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Creatures.Hiyorifish;

[HarmonyPatch]
public static class OverrideScanProgress
{
    public delegate float UpdateScanProgress(PDAScanner.ScanTarget target, float scanTime);
    public static readonly Dictionary<TechType, UpdateScanProgress> updateFuncs = [];

    private static void UpdateScanProgressProxy(ref PDAScanner.ScanTarget target, float scanTime)
    {
        UpdateScanProgress updateFunc = updateFuncs.GetOrDefault(target.techType, DefaultUpdateFunc);
        target.progress = updateFunc(target, scanTime);

    }
    private static float DefaultUpdateFunc(PDAScanner.ScanTarget target, float scanTime)
        => target.progress + Time.deltaTime / scanTime;

    public static void Register(TechType techType, UpdateScanProgress updateFunc)
    {
        if (updateFuncs.ContainsKey(techType))
            LOGGER.LogWarning($"Scan override function already defined for {techType}, overwriting");
        updateFuncs[techType] = updateFunc;
    }

    public static UpdateScanProgress Default() => DefaultUpdateFunc;
    public static UpdateScanProgress Limit(float limit)
    {
        return (target, scanTime) =>
        {
            // slow down w/ more progress
            float scaledScanTime = scanTime / (1 - target.progress);
            float next = target.progress + Time.deltaTime / scaledScanTime;
            next += Random.Range(-0.005f, 0.005f); // jitter
            if (next >= limit) // just so it doesn't show/hit 100%
                next = limit - Random.Range(0.02f, 0.05f);

            return next;
        };
    }

    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Scan))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateScanProgressPatch(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        /// progress = progress + deltaTime / timeToScan
        FieldInfo progressField = AccessTools.Field(typeof(PDAScanner.ScanTarget), "progress");
        MethodInfo getDeltaTime = AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime));
        const string SCANTIME_MATCH_NAME = "scanTime";
        CodeMatch[] match = [
            new CodeMatch(ci => ci.LoadsField(progressField, true)),
            new CodeMatch(OpCodes.Dup),
            new CodeMatch(OpCodes.Ldind_R4),
            new CodeMatch(ci => ci.Calls(getDeltaTime)),
            new CodeMatch(ci => ci.LoadsLocal(null, typeof(float)), SCANTIME_MATCH_NAME),
            new CodeMatch(OpCodes.Div),
            new CodeMatch(OpCodes.Add),
            new CodeMatch(OpCodes.Stind_R4)
        ];
        matcher.MatchForward(false, match);
        if (!matcher.IsValid)
        {
            // if we can't patch Scan, the creature will get scanned and unlock an empty nothingburger of an ency entry
            // which i guess isn't awful for a normally "unscannable" creature
            LOGGER.LogError($"Failed to patch Scan to override scan progress");
            return instructions;
        }
        matcher.RemoveInstructions(match.Length);

        /// UpdateScanProgressProxy(PDAScanner.scanTarget, timeToScan);
        matcher.Insert(
            matcher.NamedMatch(SCANTIME_MATCH_NAME).Clone(),
            SymbolExtensions.GetMethodInfo(() => UpdateScanProgressProxy(ref PDAScanner.scanTarget, 0f))
                .CallInstruction()
        );
        return matcher.InstructionEnumeration();
    }
}
