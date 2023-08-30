using UnityEngine;

namespace SCHIZO.Ermshark;

public sealed class Ermshark : Creature, IOnTakeDamage
{
    private bool isReal = true;
    public int mitosisRemaining = 4; // 2^4 = 16 max ermsharks from one single spawn

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

        if (mitosisRemaining > 0)
        {
            Mitosis();
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

    private void Mitosis()
    {
        GameObject firstChild = Instantiate(ErmsharkData.Prefab, transform.position + Random.insideUnitSphere, Quaternion.identity);
        firstChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * 0.5f;

        UpdateChild(firstChild, isReal, mitosisRemaining - 1);

        GameObject secondChild = Instantiate(ErmsharkData.Prefab, transform.position + Random.insideUnitSphere, Quaternion.identity);
        secondChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * 0.5f;
        UpdateChild(secondChild, false, mitosisRemaining - 1);
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
