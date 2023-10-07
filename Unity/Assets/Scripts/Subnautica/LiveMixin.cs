using NaughtyAttributes;
using UnityEngine;

public class LiveMixin : MonoBehaviour
{
    [Required, Expandable] public LiveMixinData data;
    public float health;

    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _damageSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _deathSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _damageClip;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _deathClip;
}
