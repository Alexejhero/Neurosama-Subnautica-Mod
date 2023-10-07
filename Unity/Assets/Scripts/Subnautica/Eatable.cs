using NaughtyAttributes;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    [BoxGroup("Nutrition")] public float foodValue;
    [BoxGroup("Nutrition")] public float waterValue;
    [BoxGroup("Nutrition"), Label("Health Value (BZ only)")] public float healthValue;
    [BoxGroup("Nutrition"), Label("Cold Meter Value (BZ only)")] public float coldMeterValue;

    [BoxGroup("Decomposing")] public bool decomposes = false;
    [BoxGroup("Decomposing"), ShowIf(nameof(decomposes))] public float kDecayRate = 0.015f;
    [InfoBox("Only rotten food can despawn")]
    [BoxGroup("Decomposing"), ShowIf(nameof(decomposes))] public bool despawns = true;
    [BoxGroup("Decomposing"), ShowIf(EConditionOperator.And, nameof(decomposes), nameof(despawns))] public float despawnDelay = 300;

    [HideInInspector] public bool removeOnUse = true;
    [HideInInspector] public int charges = -1;
}
