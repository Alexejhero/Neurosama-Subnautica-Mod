using Immersion.Formatting;
using Nautilus.Extensions;

namespace Immersion.Trackers;

// it's named Empathy because sending 'pls immersion disable empathy' in twitch chat would be funny
// that's it that's the whole jkoke
[HarmonyPatch]
public sealed class Empathy : Tracker
{
    private static readonly string[] _penglingMessages = [
        "{player} has stolen a pengling away from its loving family!",
        "{player} is kidnapping a helpless baby penguin!",
        "{player} is about to separate a baby penguin from its family!",
    ];
    private static readonly string[] _roadkillMessages = [
        "{player} has killed yet another innocent creature with {possessive} seatruck!",
        "{player} drove {possessive} seatruck into a helpless fish, killing it instantly!",
        "{player} rammed {possessive} seatruck full speed into a poor defenseless fish!"
    ];

    private float _timeLastNotified;
    private static float _penglingCooldown = 5f;
    private static float _roadkillCooldown = 60f; // no spam

    public void OnPenglingPickedUp() => Notify(_penglingMessages, _penglingCooldown);
    public void OnRoadkill() => Notify(_roadkillMessages, _roadkillCooldown);

    private void Notify(string[] messages, float cooldown)
    {
        if (Time.time < _timeLastNotified + cooldown) return;
        _timeLastNotified = Time.time;

        React(Priority.Low, Format.FormatPlayer(messages.GetRandom()));
    }
#nullable enable
    private static Empathy? Instance => COMPONENT_HOLDER.GetComponent<Empathy>().Exists();

    [HarmonyPatch(typeof(PenguinGroupDefense), nameof(PenguinGroupDefense.AddAggressionToTarget))]
    [HarmonyPostfix]
    private static void HookAfterPickup(GameObject target)
    {
        if (target != Player.main.gameObject) return;

        Instance?.OnPenglingPickedUp();
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
    [HarmonyPostfix]
    private static void CheckRoadkill(LiveMixin __instance, DamageType type, GameObject dealer)
    {
        // faster checks first
        if (type != DamageType.Collide || !dealer || __instance.IsAlive()) return;
        if (!dealer.GetComponent<SeaTruckSegment>() || !__instance.GetComponent<Creature>()) return;

        Instance?.OnRoadkill();
    }
}
