using System;
using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Handlers;
using SCHIZO.Items.Data;
using SCHIZO.Sounds;

namespace SCHIZO.Items;

public sealed class ModItem
{
    private static readonly HashSet<string> _registeredItems = new();

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

        PrefabInfo = PrefabInfo.WithTechType(data.classId, data.displayName, data.tooltip);
        if (data.icon) PrefabInfo.WithIcon(data.icon);
        PrefabInfo.WithSizeInInventory(new Vector2int(data.itemSize.x, data.itemSize.y));
    }

    public void LoadStep2()
    {
        if (ItemData.isCraftable && ItemData.CraftTreeType != CraftTree.Type.None)
        {
            CraftTreeHandler.AddCraftingNode(ItemData.CraftTreeType, this, ItemData.CraftTreePath);
            CraftDataHandler.SetCraftingTime(this, ItemData.craftingTime);
        }

        if (ItemData.isBuildable)
        {
            CraftDataHandler.AddBuildable(this);
        }

        if (ItemData.Recipe)
        {
            CraftDataHandler.SetRecipeData(this, ItemData.Recipe.Convert());
        }

        if (ItemData.TechGroup != TechGroup.Uncategorized)
        {
            CraftDataHandler.AddToGroup(ItemData.TechGroup, ItemData.TechCategory, this);
        }

        if (ItemData.DatabankInfo)
        {
            DatabankInfo i = ItemData.DatabankInfo;

            PDAHandler.AddEncyclopediaEntry(PrefabInfo.ClassID, i.encyPath, i.title, i.description.text, i.texture, i.unlockSprite,
                i.isImportantUnlock ? PDAHandler.UnlockImportant : PDAHandler.UnlockBasic);

            if (i.scanSounds) ScanSoundHandler.Register(this, i.scanSounds);
        }
    }

    public static implicit operator PrefabInfo(ModItem self) => self.PrefabInfo;

    public static implicit operator TechType(ModItem self) => self.PrefabInfo.TechType;
}
