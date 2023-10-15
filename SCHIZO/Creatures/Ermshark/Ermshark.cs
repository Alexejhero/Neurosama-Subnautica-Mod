using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

partial class Ermshark : IOnTakeDamage
{
    private bool _isReal = true;

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

        if (mitosisRemaining > 0)
        {
            Mitosis(damageInfo.position, liveMixin.damageEffect);
        }
        else if (_isReal)
        {
            SOS();
            return;
        }

        gameObject.SetActive(false); // todo verify this works
        Destroy(gameObject, 5);
    }

    private void SOS() // Save the shark from dying (reloading the save will respawn it)
    {
        transform.GetChild(0).localScale = Vector3.zero;
        liveMixin.health = 20;

        enabled = false;
        locomotion.enabled = false;
        hurtSoundPlayer.enabled = false;
        ambientSoundPlayer.enabled = false;
    }

    private void Mitosis(Vector3 position, GameObject hurtEffect)
    {
        const float childScaleModifier = 0.69f;

        GameObject firstChild = Instantiate(ermsharkPrefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
        firstChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * childScaleModifier;
        UpdateChild(firstChild, _isReal, mitosisRemaining - 1);

        GameObject secondChild = Instantiate(ermsharkPrefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
        secondChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * childScaleModifier;
        UpdateChild(secondChild, false, mitosisRemaining - 1);

        for (int i = 0; i < 5; i++) Utils.SpawnPrefabAt(hurtEffect, transform, position).transform.localScale *= 2f;
    }

    private static void UpdateChild(GameObject child, bool isReal, int mitosisRemaining)
    {
        Ermshark ermshark = child.GetComponentInChildren<Ermshark>(true);
        if (!isReal)
        {
            ermshark._isReal = false;
            Destroy(child.GetComponentInChildren<LargeWorldEntity>());
        }

        ermshark.mitosisRemaining = mitosisRemaining;
    }
}
