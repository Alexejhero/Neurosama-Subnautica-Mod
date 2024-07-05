using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SCHIZO.Helpers;
using UnityEngine;
using UWE;

namespace SCHIZO.Tweaks.Content;

[HarmonyPatch]
partial class ContentAlertManager
{
    public static ContentAlertManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

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
        if (!target || target.transform.Find($"{Instance.alertPrefab.name} (Clone)")) return;

        Instantiate(Instance.alertPrefab, target);
    }

    public static void DetachContentAlert(Transform target)
    {
        foreach (ContentVisibilityHelper alert in target.GetComponentsInChildren<ContentVisibilityHelper>())
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
