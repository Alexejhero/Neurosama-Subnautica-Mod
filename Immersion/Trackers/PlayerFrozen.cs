using Nautilus.Extensions;

namespace Immersion.Trackers;

[HarmonyPatch]
public sealed class PlayerFrozen : Tracker
{
    private bool isFrozen;

    internal void OnFrozen()
    {
        if (isFrozen) return;
        isFrozen = true;

        // i don't think the player can actually get frozen in ice but w/e
        string where = Player.main.frozenMixin._insideIce ? "in ice" : "underwater";
        React(Priority.High, $"{{player}} is frozen {where} and can't move!");
    }

    private void OnUnfrozen()
    {
        if (!isFrozen) return;
        isFrozen = false;
    }

    // just in case we don't get notified of the unfreeze, let's track it ourselves
    private void FixedUpdate()
    {
        if (!isFrozen) return;

        bool isActuallyFrozen = Player.main.Exists() is { } player
            && player.frozenMixin.Exists() is { } frozen
            && frozen.frozen;

        if (!isActuallyFrozen) OnUnfrozen();
    }

    [HarmonyPatch(typeof(PlayerFrozenMixin), nameof(PlayerFrozenMixin.Freeze))]
    [HarmonyPostfix]
    private static void NotifyFreeze(ref bool __result)
    {
        if (!__result) return;

        COMPONENT_HOLDER.GetComponent<PlayerFrozen>().OnFrozen();
    }

    [HarmonyPatch(typeof(PlayerFrozenMixin), nameof(PlayerFrozenMixin.Unfreeze))]
    [HarmonyPostfix]
    private static void NotifyUnfreeze()
    {
        COMPONENT_HOLDER.GetComponent<PlayerFrozen>().OnUnfrozen();
    }
}
