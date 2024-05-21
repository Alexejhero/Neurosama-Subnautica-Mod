using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Commands;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using Nautilus.Utility;
using SCHIZO.ConsoleCommands;
using SCHIZO.Helpers;
using UnityEngine;
using UWE;

namespace SCHIZO.Twitch;

[HarmonyPatch]
[RegisterConsoleCommands]
partial class CustomSignalManager
{
    private static SaveData _data;
    private static GameObject _prefab;
    private static readonly Dictionary<string, SignalPing> _allSignals = [];
    private static readonly Dictionary<string, SignalPing> _customSignals = [];
    internal static CustomSignalManager Instance { get; private set; }
    private void Awake()
    {
        _data = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        Instance = this;
        // Save already hooked up by RegisterSaveDataCache
        // Load, however, triggers on *starting* load (and we don't have signals to correlate that early)
        SaveUtils.RegisterOnFinishLoadingEvent(OnGameLoad);
        SaveUtils.RegisterOnQuitEvent(OnGameQuit);
    }

    private IEnumerator Start()
    {
        IPrefabRequest req = PrefabDatabase.GetPrefabAsync(signalPrefab.GetClassID());
        yield return req;
        if (!req.TryGetPrefab(out _prefab))
            LOGGER.LogError($"{nameof(CustomSignalManager)}.{nameof(signalPrefab)} (classid {signalPrefab.GetClassID()}) was not registered");
    }

    private void OnGameLoad()
    {
        _data.Load();
    }

    private void OnGameQuit()
    {
        _allSignals.Clear();
        _customSignals.Clear();
    }

    [HarmonyPatch(typeof(SignalPing), nameof(SignalPing.Start))]
    [HarmonyPostfix]
    private static void StartPostfix(SignalPing __instance)
    {
        _allSignals[__instance.descriptionKey] = __instance;
        if (_data.signals.ContainsKey(__instance.descriptionKey))
            _customSignals[__instance.descriptionKey] = __instance;
    }

    [ConsoleCommand("addsignal")]
    public static string OnConsoleCommand_addsignal(float x, float y, float z, params string[] nameSplit)
    {
        if (SaveLoadManager.temporarySavePath is null)
            return "Can't add signal, game is not loaded";
        if (!_prefab)
            return "developer forgor prefab for custom signal, please point and laugh";
        string signalName = string.Join(" ", nameSplit);
        if (_allSignals.ContainsKey(signalName))
            return $"There is already a signal named \"{signalName}\"";
        Vector3 pos = new(x, y, z);
        _customSignals[signalName] = Instance.CreateSignal(pos, signalName);
        return null;
    }
    private SignalPing CreateSignal(Vector3 pos, string signalName)
    {
        GameObject signalObj = GameObject.Instantiate(_prefab, pos, Quaternion.identity);
        signalObj.name = $"CustomSignal({signalName})";

        SignalPing signal = signalObj.EnsureComponent<SignalPing>();
        signal.descriptionKey = signalName;
        signal.pingInstance.SetType(PingType.Signal);
        signal.pingInstance.colorIndex = 0;
        signal.pos = pos;
        signal.disableOnEnter = true;

        return signal;
    }

    [ConsoleCommand("removesignal")]
    public static void OnConsoleCommand_removesignal(params string[] nameSplit)
    {
        if (nameSplit.Length == 0) return;

        string signalName = string.Join(" ", nameSplit);
        SignalPing signal = FindSignal(signalName, out bool isCustom);
        if (!signal) return;
        signalName = signal.descriptionKey;
        if (!isCustom)
            LOGGER.LogWarning($"removing possibly base game signal '{signalName}' at {signal.pos}");

        Destroy(signal.gameObject);
        _allSignals.Remove(signalName);
        _customSignals.Remove(signalName);
    }

    [ConsoleCommand("replacesignal")]
    public static void OnConsoleCommand_replacesignal(params string[] nameSplit)
    {
        if (nameSplit.Length == 0) return;

        string signalName = string.Join(" ", nameSplit);
        SignalPing oldSignal = FindSignal(signalName, out bool isCustom);
        if (!oldSignal) return;
        signalName = oldSignal.descriptionKey;
        if (!isCustom)
            LOGGER.LogWarning($"replacing possibly base game signal '{signalName}' at {oldSignal.pos}");

        SignalPing newSignal = Instance.CreateSignal(oldSignal.pos, signalName);
        _customSignals[signalName] = newSignal;
        newSignal.pingInstance.colorIndex = oldSignal.pingInstance.colorIndex;
        newSignal.pingInstance.notify = oldSignal.pingInstance.notify;
        newSignal.pingInstance.pingType = oldSignal.pingInstance.pingType;

        Destroy(oldSignal.gameObject);
        _allSignals.Remove(signalName);
    }

    private static SignalPing FindSignal(string signalName, out bool isCustomSignal)
    {
        isCustomSignal = true;
        SignalPing signal = _customSignals.PartialSearch(signalName, true);
        if (signal) return signal;

        isCustomSignal = false;
        return _allSignals.PartialSearch(signalName, true);
    }

    [HarmonyPatch(typeof(SignalPing), nameof(SignalPing.OnTriggerEnter))]
    [HarmonyPostfix]
    private static void OnDisableOnEnter(SignalPing __instance, Collider other)
    {
        if (!__instance.disableOnEnter) return;
        if (other.gameObject != Player.main.gameObject) return;

        string signalName = __instance.descriptionKey;
        if (!_customSignals.ContainsKey(signalName)) return;

        Destroy(__instance.gameObject);
        _customSignals.Remove(signalName);
        _allSignals.Remove(signalName);
    }

    [FileName("CustomSignals")]
    private class SaveData : SaveDataCache
    {
        public Dictionary<string, Vector3> signals = [];

        public SaveData()
        {
            OnStartedSaving += SaveSignals;
            OnFinishedLoading += LoadSignals;
        }

        private void SaveSignals(object sender, JsonFileEventArgs e)
        {
            signals.Clear();
            foreach (SignalPing signal in _customSignals.Values)
            {
                signals.Add(signal.descriptionKey, signal.pos);
            }
        }
        private void LoadSignals(object sender, JsonFileEventArgs e)
        {
            foreach (SignalPing signal in _allSignals.Values)
            {
                if (_customSignals.ContainsKey(signal.descriptionKey)) continue;
                if (signals.TryGetValue(signal.descriptionKey, out Vector3 pos) && signal.pos == pos)
                {
                    _customSignals[signal.descriptionKey] = signal;
                }
            }
        }
    }
}
