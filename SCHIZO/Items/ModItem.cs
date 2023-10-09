using System;
using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Handlers;
using SCHIZO.Unity.Items;

namespace SCHIZO.Items;

public sealed class ModItem
{
    private static readonly HashSet<string> _registeredItems = new();

    public ItemData ItemData { get; private set; }
    public PrefabInfo PrefabInfo { get; private set; }

    public ModItem(ItemData data)
    {
        LOGGER.LogDebug("Registering item " + data.classId + " with name " + data.displayName);

        if (_registeredItems.Contains(data.classId)) throw new Exception("Item with classId " + data.classId + " has already been registered!");
        _registeredItems.Add(data.classId);

        if (data.ModItem != null) throw new Exception("ItemData with classId " + data.classId + " has already been registered!");
        data.ModItem = this;

        ItemData = data;

        PrefabInfo = PrefabInfo.WithTechType(data.classId, data.displayName, data.tooltip);
        PrefabInfo.WithIcon(data.icon).WithSizeInInventory(new Vector2int(data.itemSize.x, data.itemSize.y));

        if (ItemData.TechGroup != TechGroup.Uncategorized) CraftDataHandler.AddToGroup(ItemData.TechGroup, ItemData.TechCategory, this);
        if (ItemData.isBuildable) CraftDataHandler.AddBuildable(this);
    }

    public static implicit operator PrefabInfo(ModItem self) => self.PrefabInfo;

    public static implicit operator TechType(ModItem self) => self.PrefabInfo.TechType;
}
