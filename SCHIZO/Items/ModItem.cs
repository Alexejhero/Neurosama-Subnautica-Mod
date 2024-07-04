using System;
using System.Collections.Generic;
using Nautilus.Assets;
using SCHIZO.Items.Data;

namespace SCHIZO.Items;

public sealed class ModItem
{
    private static readonly HashSet<string> _registeredItems = [];

    public ItemData ItemData { get; }
    public PrefabInfo PrefabInfo { get; }

    public static ModItem Create(ItemData data) => new(data);

    public ModItem(ItemData data)
    {
        LOGGER.LogDebug("Creating ModItem " + data.classId + " with name " + data.displayName);

        if (_registeredItems.Contains(data.classId)) throw new Exception("Item with classId " + data.classId + " has already been created!");
        _registeredItems.Add(data.classId);

        if (data.ModItem != null) throw new Exception("ItemData with classId " + data.classId + " has already been created!");
        data.ModItem = this;

        ItemData = data;

        PrefabInfo = PrefabInfo.WithTechType(data.classId, data.displayName, data.tooltip, unlockAtStart: false);
        if (data.icon) PrefabInfo.WithIcon(data.icon);
        PrefabInfo.WithSizeInInventory(new Vector2int(data.itemSize.x, data.itemSize.y));
    }

    public static implicit operator PrefabInfo(ModItem self) => self.PrefabInfo;

    public static implicit operator TechType(ModItem self) => self.PrefabInfo.TechType;
}
