using NaughtyAttributes;
using UnityEngine;

public class LiveMixin : MonoBehaviour
{
    [Required, Expandable] public LiveMixinData data;
    public float health;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset _damageSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset _deathSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset _damageClip;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset _deathClip;
}
