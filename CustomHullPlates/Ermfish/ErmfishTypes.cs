using System.Collections.Generic;
using Nautilus.Assets;

namespace SCHIZO.Ermfish;

public static class ErmfishTypes
{
    public static readonly PrefabInfo Regular = PrefabInfo.WithTechType("ermfish", "Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>");
    public static readonly PrefabInfo Cooked = PrefabInfo.WithTechType("cookedermfish", "Cooked Ermfish", "erm\n<size=75%>(Model by w1n7er, icon by SADecsSs)</size>");
    public static readonly PrefabInfo Cured = PrefabInfo.WithTechType("curedermfish", "Cured Ermfish", "erm\n<size=75%>(Model by w1n7er, icon by SADecsSs)</size>");
    public static readonly List<TechType> AllTechTypes = new() { Regular.TechType, Cooked.TechType, Cured.TechType };
}
