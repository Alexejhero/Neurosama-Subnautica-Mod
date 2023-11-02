using System;
using System.Collections.Generic;
using Nautilus.Assets;

namespace SCHIZO;

public static class ModItems
{
    public static readonly ModItem Anneel = new("Anneel", "Anneel");
}

public sealed class ModItem
{
    private static readonly HashSet<string> _registeredItems = new();

    public readonly PrefabInfo PrefabInfo;

    public string ClassId => PrefabInfo.ClassID;

    public string DisplayName { get; private set; }

    public string Tooltip { get; private set; }

    public ModItem(string classId, string displayName, string tooltip = "")
    {
        LOGGER.LogDebug("Creating Moditem " + classId + " with name " + displayName);

        if (_registeredItems.Contains(classId)) throw new Exception("Item with classId " + classId + " has already been created!");
        _registeredItems.Add(classId);

        PrefabInfo = PrefabInfo.WithTechType(classId, displayName, tooltip);
        DisplayName = displayName;
        Tooltip = tooltip;
    }

    public static implicit operator PrefabInfo(ModItem self) => self.PrefabInfo;

    public static implicit operator TechType(ModItem self) => self.PrefabInfo.TechType;
}
