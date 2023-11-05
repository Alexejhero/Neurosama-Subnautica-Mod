using SCHIZO.TriInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Subnautica/Live Mixin Data")]
public class LiveMixinData : TriScriptableObject
{
    public float maxHealth = 100;
    public bool passDamageDataOnDeath;
    public bool broadcastKillOnDeath = true;

    [UnexploredGroup] public bool weldable = false;
    [UnexploredGroup] public bool knifeable = true;
    [UnexploredGroup] public bool destroyOnDeath = false;
    [UnexploredGroup] public float minDamageForSound = 0;
    [UnexploredGroup] public float loopEffectBelowPercent = 0;
    [UnexploredGroup] public GameObject loopingDamageEffect = null;
    [UnexploredGroup] public bool canResurrect;

    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject damageEffect = null;
    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject deathEffect = null;
    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject electricalDamageEffect = null;
}
