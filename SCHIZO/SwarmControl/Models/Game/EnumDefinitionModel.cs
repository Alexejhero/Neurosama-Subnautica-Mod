using System;

namespace SwarmControl.Models.Game;
public class EnumDefinitionModel(Type enumType)
{
    public string Name { get; set; } = enumType.Name;
    public string[] Values { get; set; } = enumType.GetEnumNames();
}
