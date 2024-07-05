using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Tweaks.Content;

[HarmonyPatch]
partial class ContentAlertManager
{
    public static ContentAlertManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private static bool _alertsEnabled;
    internal static bool AlertsEnabled
    {
        get => _alertsEnabled;
        set
        {
            if (_alertsEnabled == value) return;
            _alertsEnabled = value;
            OnAlertsEnabledChanged?.Invoke();
        }
    }
    public static Action OnAlertsEnabledChanged;

    private static HashSet<TechType> _attached = [];

    public static void AttachToTechType(TechType techType)
    {
        if (!_attached.Add(techType)) return;
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
        if (CraftData.entClassTechTable is null) return;

        if (__instance is not PrefabIdentifier { classId.Length: >0 } pid) return;

        TechType techType = CraftData.entClassTechTable.GetOrDefault(pid.classId);
        if (!_attached.Contains(techType)) return;

        AttachContentAlert(pid.transform);
    }

    [HarmonyPatch(typeof(TechTag), MethodType.Constructor)]
    [HarmonyPostfix]
    private static void TechTagAwake(TechTag __instance)
    {
        if (!_attached.Contains(__instance.type)) return;

        AttachContentAlert(__instance.transform);
    }

    public static void AttachContentAlert(Transform target)
    {
        if (!target || target.transform.Find($"{Instance.alertPrefab.name}(Clone)")) return;

        Instantiate(Instance.alertPrefab, target);
    }

    public static void DetachContentAlert(Transform target)
    {
        foreach (ContentVisibilityHelper alert in target.GetComponentsInChildren<ContentVisibilityHelper>(true))
        {
            if (alert) GameObject.Destroy(alert.gameObject);
        }
    }

    public static void Clear()
    {
        foreach (TechType tt in _attached.ToList()) // modifying collection
            DetachFromTechType(tt);
    }
}
