using System;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Patches;

public class DisableCscSettingsLog : AssetPostprocessor
{
    private static readonly MethodInfo _targetMethod = AccessTools.Method(typeof(Debug), nameof(Debug.Log), new[] {typeof(object)});

    public override int GetPostprocessOrder() => int.MaxValue;

    private static string OnGeneratedCSProject(string path, string content)
    {
        if (!(Harmony.GetPatchInfo(_targetMethod)?.Prefixes?.Count > 0))
        {
            new Harmony(new Guid().ToString()).Patch(_targetMethod, new HarmonyMethod(typeof(DisableCscSettingsLog), nameof(Prefix)));
        }

        return content;
    }

    public static bool Prefix(object message)
    {
        return !message.ToString().Contains("OnGeneratedCSProject");
    }
}
