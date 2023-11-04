using UnityEngine;

namespace SCHIZO.Creatures.Components;

public partial interface IOnMeleeAttack
{
    /// <summary>
    /// Handle an attack before it does damage to the target.<br/>
    /// Useful if you want to do something else instead.
    /// </summary>
    /// <returns>Whether the attack was "handled" (i.e. whether to cancel the attack).</returns>
    // don't rename this to OnMeleeAttack, MeleeAttack sends a message named "OnMeleeAttack" to a completely different component
    bool HandleMeleeAttack(GameObject target);
}
