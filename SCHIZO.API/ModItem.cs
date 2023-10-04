using System;
using System.Collections.Generic;
using Nautilus.Assets;

namespace SCHIZO.API;

public sealed class ModItem
{
    private static readonly HashSet<string> _registeredItems = new();

    private readonly PrefabInfo _info;

    public string ClassId => _info.ClassID;

    public string DisplayName { get; private set; }

    public string Tooltip { get; private set; }

    public ModItem(string classId, string displayName, string tooltip = "")
    {
        LOGGER.LogDebug("Registering item " + classId + " with name " + displayName);

        if (_registeredItems.Contains(classId)) throw new Exception("Item with classId " + classId + " has already been registered!");
        _registeredItems.Add(classId);

        _info = PrefabInfo.WithTechType(classId, displayName, tooltip);
        DisplayName = displayName;
        Tooltip = tooltip;
    }

    public static implicit operator PrefabInfo(ModItem self) => self._info;

    public static implicit operator TechType(ModItem self) => self._info.TechType;
}
