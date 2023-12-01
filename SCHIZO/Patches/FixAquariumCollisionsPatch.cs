using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Patches;

[HarmonyPatch]
public static class FixAquariumCollisionsPatch
{
    [HarmonyPatch(typeof(DealDamageOnImpact), nameof(DealDamageOnImpact.OnCollisionEnter))]
    [HarmonyPrefix]
    public static bool FixAquariumCollisionDamage(DealDamageOnImpact __instance, Collision collision)
        => !__instance.gameObject.GetComponent<SubRoot>() || !collision.gameObject.GetComponent<Aquarium>();

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnCollisionEnter))]
    [HarmonyPrefix]
    public static bool FixAquariumCollisionSound(SubRoot __instance, Collision col) => !col.gameObject.GetComponent<Aquarium>();
}
