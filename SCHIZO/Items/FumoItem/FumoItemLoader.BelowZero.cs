using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemLoader
{
    public override void Load(ModItem modItem)
    {
        base.Load(modItem);
        CustomPrefab prefab = new(spawnerClassId, null, null);
        prefab.SetGameObject(() =>
        {
            GameObject instance = GameObject.Instantiate(spawnerPrefab);
            PrefabUtils.AddBasicComponents(instance, spawnerClassId, TechType.None, LargeWorldEntity.CellLevel.Global);
            instance.SetActive(false); // why do we have to do this manually again
            return instance;
        });
        prefab.Register();
        FumoItemPatches.Register(spawnerClassId);
    }
}
