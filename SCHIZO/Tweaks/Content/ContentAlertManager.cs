using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Commands;
using SCHIZO.ConsoleCommands;
using SCHIZO.Helpers;
using UnityEngine;
using UWE;

namespace SCHIZO.Tweaks.Content;

[RegisterConsoleCommands]
[HarmonyPatch]
partial class ContentAlertManager
{
    public static ContentAlertManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private const string ConCommandUsage = """
        contentalerts <option>
        options:
            <enable|disable> - all alerts
            <attach|detach> <techtype> - attach to techtype
            <clear> - detach all techtypes
        """;
    private static bool _alerting;
    internal static bool AlertsEnabled
    {
        get => _alerting;
        set
        {
            if (_alerting == value) return;
            _alerting = value;
            OnAlertsEnabledChanged?.Invoke();
        }
    }
    public static Action OnAlertsEnabledChanged;

    private static HashSet<TechType> _attached = [];

    [ConsoleCommand("contentalerts")]
    public static string OnConsoleCommand_contentalerts(params string[] args)
    {
        if (args is not [string subCommand, ..])
            return ConCommandUsage;

        switch (subCommand)
        {
            case "enable" or "disable":
                AlertsEnabled = subCommand == "enable";
                return null;
            case "attach" or "detach" when args.Length > 1:
                string techTypeName = args[1];
                if (!UWE.Utils.TryParseEnum(techTypeName, out TechType techType))
                    return MessageHelpers.TechTypeNotFound(techTypeName);
                if (subCommand == "attach")
                    AttachToTechType(techType);
                else
                    DetachFromTechType(techType);
                return null;
            case "clear":
                foreach (TechType tt in _attached.ToList()) // modifying collection
                    DetachFromTechType(tt);
                return null;
            default:
                return ConCommandUsage;
        }
    }

    public static void AttachToTechType(TechType techType)
    {
        _attached.Add(techType);
        foreach (GameObject obj in GameObjectEnumerableHelpers.AllOfTechType(techType))
        {
            AttachContentAlert(obj.transform);
        }
    }

    public static void DetachFromTechType(TechType techType)
    {
        _attached.Remove(techType);
        foreach (GameObject obj in GameObjectEnumerableHelpers.AllOfTechType(techType))
        {
            DetachContentAlert(obj.transform);
        }
    }

    [HarmonyPatch(typeof(UniqueIdentifier), nameof(UniqueIdentifier.Awake))]
    [HarmonyPostfix]
    private static void ClassIdAwake(UniqueIdentifier __instance)
    {
        if (__instance is PrefabIdentifier)
            CoroutineHost.StartCoroutine(Coro(__instance));
    }

    private static IEnumerator Coro(UniqueIdentifier uid)
    {
        while (CraftData.entClassTechTable is null)
            yield return null;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.15f));
        if (!uid || uid.classId is null) yield break;

        TechType techType = CraftData.GetTechType(uid.gameObject, out _);
        if (techType == TechType.None) yield break;
        if (!_attached.Contains(techType)) yield break;

        AttachContentAlert(uid.transform);
    }

    public static void AttachContentAlert(Transform target)
    {
        if (!target || target.GetComponentInChildren<ContentVisibilityHelper>()) return;

        Instantiate(Instance.alertPrefab, target);
    }

    public static void DetachContentAlert(Transform target)
    {
        ContentVisibilityHelper alert = target.GetComponentInChildren<ContentVisibilityHelper>();
        if (alert) GameObject.Destroy(alert.gameObject);
    }
}
