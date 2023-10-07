using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LiveMixinData.asset", menuName = "Subnautica/Create LiveMixin Data Asset")]
public class LiveMixinData : ScriptableObject
{
    public float maxHealth = 100;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool weldable = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool knifeable = true;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool destroyOnDeath = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float minDamageForSound = 0;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float loopEffectBelowPercent = 0;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public GameObject loopingDamageEffect = null;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool canResurrect;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool passDamageDataOnDeath;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool broadcastKillOnDeath = true;

    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject damageEffect = null;
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject deathEffect = null;
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject electricalDamageEffect = null;
}
