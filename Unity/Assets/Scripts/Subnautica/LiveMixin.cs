using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class LiveMixin : MonoBehaviour
{
    [Required, Expandable] public LiveMixinData data;
    public float health;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset damageSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset deathSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset damageClip;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset deathClip;
}
