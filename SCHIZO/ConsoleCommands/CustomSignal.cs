using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Commands;
using Story;
using UnityEngine;

namespace SCHIZO.ConsoleCommands;

[HarmonyPatch]
[RegisterConsoleCommands]
internal static class CustomSignal
{
    private static readonly Dictionary<string, SignalPing> _signals = [];

    [HarmonyPatch(typeof(SignalPing), nameof(SignalPing.Start))]
    [HarmonyPostfix]
    private static void StartPostfix(SignalPing __instance)
    {
        _signals[__instance.descriptionKey] = __instance;
    }

    [ConsoleCommand("addsignal")]
    public static string OnConsoleCommand_addsignal(float x, float y, float z, params string[] nameSplit)
    {
        if (!StoryGoalManager.main)
            return "No signals in main menu";
        string signalName = string.Join(" ", nameSplit);
        if (_signals.ContainsKey(signalName))
            return $"There is already a signal named \"{signalName}\"";
        Vector3 pos = new(x, y, z);
        CreateSignal(pos, signalName);
        return null;
    }
    private static void CreateSignal(Vector3 pos, string signalName)
    {
        GameObject signalObj = GameObject.Instantiate(StoryGoalManager.main.onGoalUnlockTracker.signalPrefab, pos, Quaternion.identity);

        SignalPing signal = signalObj.GetComponent<SignalPing>();
        signal.descriptionKey = signalName;
        signal.pingInstance.SetType(PingType.Signal);
        signal.pos = pos;
        signal.disableOnEnter = true;
    }

    [ConsoleCommand("removesignal")]
    public static void OnConsoleCommand_removesignal(params string[] nameSplit)
    {
        string signalName = string.Join(" ", nameSplit);
        if (!_signals.TryGetValue(signalName, out SignalPing signal))
        {
            signal = _signals.Values.FirstOrDefault(s => s.descriptionKey.Contains(signalName));
        }
        if (!signal || signal.pingInstance.pingType != PingType.Signal) return;
        Object.Destroy(signal.gameObject);
    }
}
