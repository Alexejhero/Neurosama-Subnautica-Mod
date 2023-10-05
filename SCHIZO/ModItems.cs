using System.Collections.Generic;
using Nautilus.Assets;

namespace SCHIZO;

public static class ModItems
{
    public static readonly ModItem Erm = new("NeuroErm2", "Erm", "erm\n<size=75%>(Model by w1n7er)</size>");

    public static readonly ModItem Ermfish = new("ermfish", "Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
    public static readonly ModItem CookedErmfish = new("cookedermfish", "Cooked Ermfish", "erm\n<size=75%>(Model by w1n7er, icon by SADecsSs)</size>");
    public static readonly ModItem CuredErmfish = new("curedermfish", "Cured Ermfish", "erm\n<size=75%>(Model by w1n7er, icon by SADecsSs)</size>");

    public static readonly ModItem Ermshark = new("ermshark", "Ermshark");
    public static readonly ModItem Anneel = new("Anneel", "Anneel");

    public static readonly ModItem Tutel = new("tutel", "Tutel", "that's crazy\n<size=75%>(Model by FutabaKuuhaku)</size>");
    public static readonly ModItem CookedTutel = new("cookedtutel", "Cooked Tutel", "that's actually crazy\n<size=75%>(Model by FutabaKuuhaku)</size>");
    public static readonly ModItem CuredTutel = new("curedtutel", "Cured Tutel", "that's messed up\n<size=75%>(Model by FutabaKuuhaku)</size>");

    public static readonly ModItem Gymbag = new("gymbag", "Gymbag", "A lightly worn gym bag that emanates a faint smell. How does it fit all that stuff?..\n<size=75%>(Model by Vaalmyr)</size>");
}

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
