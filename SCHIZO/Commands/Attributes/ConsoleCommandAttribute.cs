using System;
using JetBrains.Annotations;

namespace SCHIZO.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class), MeansImplicitUse]
internal class ConsoleCommandAttribute(string overrideName = null) : Attribute
{
    public string OverrideName { get; } = overrideName;
}
