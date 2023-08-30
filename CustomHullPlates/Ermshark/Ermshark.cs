using UnityEngine;

namespace SCHIZO.Ermshark;

public sealed class Ermshark : Creature, IOnTakeDamage
{
    private bool isReal = true;

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (liveMixin.health > 0) return;

        GameObject firstChild = Instantiate(ErmsharkData.Prefab, transform.position + Random.insideUnitSphere * 2, Quaternion.identity);
        firstChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * 0.65f;
        if (!isReal) MarkFake(firstChild);

        GameObject secondChild = Instantiate(ErmsharkData.Prefab, transform.position + Random.insideUnitSphere * 2, Quaternion.identity);
        secondChild.transform.GetChild(0).localScale = transform.GetChild(0).localScale * 0.65f;
        MarkFake(secondChild);

        Destroy(gameObject);
    }

    private static void MarkFake(GameObject child)
    {
        child.GetComponentInChildren<Ermshark>(true).isReal = false;
        Destroy(child.GetComponentInChildren<LargeWorldEntity>());
    }
}
