using UnityEngine;

namespace SCHIZO.Helpers;

public static class CreatureActionHelpers
{
#if SUBNAUTICA
    public static float Evaluate(this CreatureAction action, float time)
        => action.Evaluate(action.creature, time);
    public static void StartPerform(this CreatureAction action, float time)
        => action.StartPerform(action.creature, time);
    public static void Perform(this CreatureAction action, float time, float deltaTime)
        => action.Perform(action.creature, time, deltaTime);
    public static void StopPerform(this CreatureAction action, float time)
        => action.StopPerform(action.creature, time);
#endif

    public static float Evaluate(this CreatureAction action)
        => action.Evaluate(Time.time);
    public static void StartPerform(this CreatureAction action)
        => action.StartPerform(Time.time);
    public static void Perform(this CreatureAction action)
        => action.Perform(Time.time, Time.deltaTime);
    public static void StopPerform(this CreatureAction action)
        => action.StopPerform(Time.time);

#if BELOWZERO
    public static FMOD_CustomEmitter GetBiteSound(this MeleeAttack attack)
        => attack.biteSound;
#else
    public static FMOD_StudioEventEmitter GetBiteSound(this MeleeAttack attack)
        => attack.attackSound;
#endif
}
