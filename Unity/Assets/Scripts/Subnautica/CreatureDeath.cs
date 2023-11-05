using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class CreatureDeath : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public LiveMixin liveMixin;
    [ComponentReferencesGroup, Required] public Rigidbody useRigidbody;
    [ComponentReferencesGroup] public Eatable eatable;

    public float removeCorpseAfterSeconds = -1;
    public bool respawn = true;
    [ShowIf(nameof(respawn))] public bool respawnOnlyIfKilledByCreature = false;
    [ShowIf(nameof(respawn))] public float respawnInterval = 300;
    public bool sink = true;

    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public GameObject respawnerPrefab;
}
