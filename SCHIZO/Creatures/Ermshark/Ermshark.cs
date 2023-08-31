using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class Ermshark : Creature, IOnTakeDamage
{
    private bool isReal = true;
    public int mitosisRemaining = 4; // 2^4 = 16 max ermsharks from one single spawn

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

        if (mitosisRemaining > 0)
        {
            Mitosis(damageInfo.position, liveMixin.damageEffect);
        }
        else if (isReal)
        {
            SOS();
            return;
        }

        Destroy(gameObject);
    }

    private void SOS() // Save the shark from dying (reloading the save will respawn it)
    {
        transform.GetChild(0).localScale = Vector3.zero;
        liveMixin.health = 20;
        enabled = false;
    }

    private void Mitosis(Vector3 position, GameObject hurtEffect)
    {
        const float childScaleModifier = 0.69f;

        GameObject firstChild = Instantiate(ErmsharkData.Prefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
        firstChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * childScaleModifier;
        UpdateChild(firstChild, isReal, mitosisRemaining - 1);

        GameObject secondChild = Instantiate(ErmsharkData.Prefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
        secondChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * childScaleModifier;
        UpdateChild(secondChild, false, mitosisRemaining - 1);

        for (int i = 0; i < 5; i++) Utils.SpawnPrefabAt(hurtEffect, transform, position).transform.localScale *= 2f;
    }

    private static void UpdateChild(GameObject child, bool isReal, int mitosisRemaining)
    {
        Ermshark ermshark = child.GetComponentInChildren<Ermshark>(true);
        if (!isReal)
        {
            ermshark.isReal = false;
            Destroy(child.GetComponentInChildren<LargeWorldEntity>());
        }

        ermshark.mitosisRemaining = mitosisRemaining;
    }
}
