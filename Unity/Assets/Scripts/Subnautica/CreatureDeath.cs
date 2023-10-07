using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class CreatureDeath : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LiveMixin liveMixin;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody useRigidbody;
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public Eatable eatable;

    public float removeCorpseAfterSeconds = -1;
    public bool respawn = true;
    [ShowIf(nameof(respawn))] public bool respawnOnlyIfKilledByCreature = false;
    [ShowIf(nameof(respawn))] public float respawnInterval = 300;

    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject respawnerPrefab;

    [HideInInspector] public bool sink = true;
}
