using Nautilus.Assets;

namespace SCHIZO.Utilities;

public sealed class ModItem
{
    private readonly PrefabInfo _info;

    public string ClassId => _info.ClassID;

    public string DisplayName { get; private set; }

    public string Tooltip { get; private set; }

    public ModItem(string classId, string displayName, string tooltip)
    {
        _info = PrefabInfo.WithTechType(classId, displayName, tooltip);
        DisplayName = displayName;
        Tooltip = tooltip;
    }

    public static implicit operator PrefabInfo(ModItem self)
    {
        return self._info;
    }

    public static implicit operator TechType(ModItem self)
    {
        return self._info.TechType;
    }
}
