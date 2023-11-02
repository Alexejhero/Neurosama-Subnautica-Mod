using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Helpers;

public static class RetargetHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static
#if SUBNAUTICA
        TSubnautica
#else
        TBelowZero
#endif
        Pick<TSubnautica, TBelowZero>(TSubnautica subnautica, TBelowZero belowZero)
    {
#if SUBNAUTICA
        return subnautica;
#else
        return belowZero;
#endif
    }
#if BELOWZERO
    public static float Evaluate(this CreatureAction action, Creature creature, float time)
        => action.Evaluate(time);
    public static void StartPerform(this CreatureAction action, Creature creature, float time)
        => action.StartPerform(time);
    public static void Perform(this CreatureAction action, Creature creature, float time, float deltaTime)
        => action.Perform(time, deltaTime);
    public static void StopPerform(this CreatureAction action, Creature creature, float time)
        => action.StopPerform(time);
    public static FMOD_CustomEmitter GetBiteSound(this MeleeAttack attack)
        => attack.biteSound;
#else
    public static float Evaluate(this CreatureAction action, float time)
        => action.Evaluate(action.creature, time);
    public static void StartPerform(this CreatureAction action, float time)
        => action.StartPerform(action.creature, time);
    public static void Perform(this CreatureAction action, float time, float deltaTime)
        => action.Perform(action.creature, time, deltaTime);
    public static void StopPerform(this CreatureAction action, float time)
        => action.StopPerform(action.creature, time);
    public static FMOD_StudioEventEmitter GetBiteSound(this MeleeAttack attack)
        => attack.attackSound;
#endif
    public static float Evaluate(this CreatureAction action)
        => action.Evaluate(Time.time);
    public static void StartPerform(this CreatureAction action)
        => action.StartPerform(action.creature, Time.time);
    public static void Perform(this CreatureAction action)
        => action.Perform(action.creature, Time.time, Time.deltaTime);
    public static void StopPerform(this CreatureAction action)
        => action.StopPerform(action.creature, Time.time);

}
