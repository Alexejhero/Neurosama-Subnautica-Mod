using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Events.ErmfishDefenseForce;

[HarmonyPatch]
public static class EdfPatches
{
    [HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
    [HarmonyPostfix]
    public static void OnEat(Survival __instance, GameObject useObj)
    {
        if (!ErmfishDefenseForce.instance) return;
        if (!__instance.GetComponent<Player>()) return;

        ErmfishDefenseForce.instance.OnEat(CraftData.GetTechType(useObj));
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
    [HarmonyPostfix]
    public static void OnAttack(LiveMixin __instance, GameObject dealer)
    {
        if (!ErmfishDefenseForce.instance) return;
        if (dealer != Player.main.gameObject) return;

        ErmfishDefenseForce.instance.OnAttack(CraftData.GetTechType(__instance.gameObject));
    }

    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
    [HarmonyPostfix]
    public static void OnCook(TechType techType)
    {
        if (!ErmfishDefenseForce.instance) return;

#if BELOWZERO
        IEnumerable<NIngredient> ingredients = TechData.GetIngredients(techType);
#else
        NTechData techData = CraftData.techData[techType];
        IEnumerable<NIngredient> ingredients = techData._ingredients;
#endif
        foreach (NIngredient ingredient in ingredients)
        {
            for (int i = 0; i < ingredient.amount; i++)
                ErmfishDefenseForce.instance.OnCook(ingredient.techType);
        }
    }

    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.Pickup))]
    [HarmonyPostfix]
    public static void OnPickup(Pickupable __instance)
    {
        if (!ErmfishDefenseForce.instance) return;

        ErmfishDefenseForce.instance.OnPickup(__instance.GetTechType());
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
    [HarmonyPostfix]
    public static void ClearKarmaOnPlayerDeathByDefenders(LiveMixin __instance, DamageInfo inDamage)
    {
        if (__instance.gameObject != Player.main.gameObject) return;
        if (__instance.health - inDamage.damage > 0) return;

        GameObject dealerDefender = ErmfishDefenseForce.instance.ActiveDefenders
            .FirstOrDefault(def => inDamage.dealer.transform.IsChildOf(def.transform));
        if (!dealerDefender) return;

        ErmfishDefenseForce.instance.OnPlayerKilledByDefender(dealerDefender);
    }
}
