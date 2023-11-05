using TriInspector;
using UnityEngine;

[DeclareBoxGroup("Nutrition")]
[DeclareToggleGroup("Decomposes")]
public class Eatable : MonoBehaviour
{
    [Group("Nutrition")] public float foodValue;
    [Group("Nutrition")] public float waterValue;
    [Title("BZ only")]
    [Group("Nutrition")] public float healthValue;
    [Group("Nutrition")] public float coldMeterValue;

    [Group("Decomposes")] public bool decomposes = false;
    [Group("Decomposes")] public float kDecayRate = 0.015f;
    [InfoBox("Only rotten food can despawn")]
    [Group("Decomposes")] public bool despawns = true;
    [Group("Decomposes"), ShowIf(nameof(despawns))] public float despawnDelay = 300;

    [HideInInspector] public bool removeOnUse = true;
    [HideInInspector] public int charges = -1;
}
