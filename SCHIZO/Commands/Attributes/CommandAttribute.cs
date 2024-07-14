using System;
using JetBrains.Annotations;

namespace SCHIZO.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
public class CommandAttribute : Attribute
{
    public required string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool RegisterConsoleCommand { get; set; }
    public string Remarks { get; set; }
}
