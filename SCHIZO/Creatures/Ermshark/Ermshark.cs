using System.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.Creatures.Ermshark;

partial class Ermshark : IOnTakeDamage
{
    private bool _isReal = true;
    private static GameObject _ermsharkPrefab;
    private static bool _hasStartedPrefabCoroutine;

    private new IEnumerator Start()
    {
        base.Start();
        if (_hasStartedPrefabCoroutine) yield break;
        _hasStartedPrefabCoroutine = true;

        IPrefabRequest request = PrefabDatabase.GetPrefabAsync(GetComponent<PrefabIdentifier>().classId);
        yield return request;

        if (!request.TryGetPrefab(out _ermsharkPrefab))
            LOGGER.LogError($"Could not get prefab for {name}, so mitosis won't work!");
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

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
                gameObject.SetActive(false); // todo verify this works
                Destroy(gameObject, 5);
            }
        }
    }

    private void SOS() // Save the shark from dying (reloading the save will respawn it)
    {
        transform.GetChild(0).localScale = Vector3.zero;
        liveMixin.ResetHealth();

        enabled = false;
        locomotion.enabled = false;
        hurtSoundPlayer.enabled = false;
        ambientSoundPlayer.enabled = false;
    }

    private void Mitosis(Vector3 position, GameObject hurtEffect)
    {
        const float splitScaleModifier = 0.69f;
        if (!_ermsharkPrefab)
        {
            LOGGER.LogError($"No prefab for mitosis, committing Minecraft");
            mitosisRemaining = 0;
            SOS();
            return;
        }

        liveMixin.ResetHealth();
        transform.position = position + Random.insideUnitSphere * 0.5f;
        transform.GetChild(0).localScale *= splitScaleModifier;

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
    }
}
