using UnityEngine;

namespace SCHIZO.Ermshark;

public sealed class Ermshark : Creature
{
    private new void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < 2; i++)
        {
            GameObject child = Instantiate(ErmsharkData.Prefab, transform.position + Random.insideUnitSphere * 2, Quaternion.identity);
            child.transform.GetChild(0).localScale = transform.GetChild(0).localScale * 0.75f;
            LargeWorldEntity.Register(child);
            CrafterLogic.NotifyCraftEnd(child, ErmsharkData.Info.TechType);
            child.SendMessage("StartConstruction", SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }
}
