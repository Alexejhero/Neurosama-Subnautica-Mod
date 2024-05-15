using System.Collections;
using SCHIZO.Creatures.Components;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;
using UnityEngine;
using UWE;

namespace SCHIZO.Creatures.Ermshark;

partial class Ermshark : IOnTakeDamage
{
    private bool _isReal = true;
    private static GameObject _ermsharkPrefab;
    private static IPrefabRequest _ermsharkRequest;

    private new IEnumerator Start()
    {
        base.Start();
        //if (_ermsharkRequest is null) gameObject.SetActive(false); // simulate bug

        _ermsharkRequest ??= PrefabDatabase.GetPrefabAsync(GetComponent<PrefabIdentifier>().classId);
        if (_ermsharkPrefab || _ermsharkRequest.TryGetPrefab(out _ermsharkPrefab)) yield break;

        yield return _ermsharkRequest;
        //yield return CoroutineHost.StartCoroutine(_ermsharkRequest);
        if (!_ermsharkRequest.TryGetPrefab(out _ermsharkPrefab))
            LOGGER.LogError($"Could not get prefab for {name}, so mitosis won't work!");
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

        actions.ForEach(action => action.StopPerform());

        if (mitosisRemaining > 0)
        {
            Mitosis(damageInfo.position, liveMixin.damageEffect);
        }
        else
        {
            if (_isReal)
            {
                SOS();
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(gameObject, 5);
            }
        }
    }

    private void SOS() // Save the shark from dying (reloading the save will respawn it)
    {
        transform.GetChild(0).localScale = Vector3.zero;
        liveMixin.ResetHealth();

        OnKill(); // disables self and SwimBehaviour
        locomotion.enabled = false;
        GetComponents<SoundPlayer>().ForEach(soundPlayer => soundPlayer.enabled = false);
    }

    private void Mitosis(Vector3 position, GameObject hurtEffect)
    {
        const float splitScaleModifier = 0.69f;
        if (!_ermsharkPrefab)
        {
            LOGGER.LogWarning("No prefab for mitosis, committing Minecraft");
            mitosisRemaining = 0;
            SOS();
            return;
        }

        liveMixin.ResetHealth();
        transform.position = position + Random.insideUnitSphere * 0.5f;
        Transform model = transform.GetChild(0);
        model.localScale *= splitScaleModifier;
        // if the shark gets small enough, carrying other fish looks weird (bc they're bigger than it)
        if (model.localScale.x < 0.3f) DisableCarry(gameObject);

        mitosisRemaining--;
        SpawnDecoy(position);

        for (int i = 0; i < 5; i++) Utils.SpawnPrefabAt(hurtEffect, transform, position).transform.localScale *= 2f;
    }

    private void SpawnDecoy(Vector3 position)
    {
        GameObject decoy = Instantiate(_ermsharkPrefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
        decoy.transform.GetChild(0).localScale = transform.GetChild(0).localScale;

        Ermshark ermshark = decoy.GetComponentInChildren<Ermshark>(true);
        ermshark.liveMixin.health = liveMixin.health;
        ermshark._isReal = false;
        Destroy(decoy.GetComponentInChildren<LargeWorldEntity>());

        ermshark.mitosisRemaining = mitosisRemaining;

        DisableCarry(decoy);
    }

    private static void DisableCarry(GameObject shark)
    {
        // forgor there are actually two of them on the shark
        foreach (CarryCreature carry in shark.GetComponents<CarryCreature>())
            Destroy(carry);
    }
}
