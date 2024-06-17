using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SwarmControl.Models.Game;
public class EnumDefinitionModel(Type enumType)
{
    public string Name { get; set; } = enumType.Name;
    public string[] Values { get; set; } = enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => f.GetCustomAttribute<EnumMemberAttribute>() is { } customDisplayName
            ? customDisplayName.Value
            : f.Name)
        .ToArray();
}
