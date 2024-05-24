using System;
using JetBrains.Annotations;

namespace SCHIZO.Commands.Attributes;

/// <summary>
/// Marks the given method to auto-register as a subcommand.
/// <br/>
/// <br/>
/// <b>Not valid if declared on a non-<see cref="Command"/> class!</b>
/// </summary>
/// <param name="nameOverride">
/// If provided, this name will be used for the subcommand.<br/>
/// Otherwise, by default the name is taken from the member on which this attribute is declared.
/// </param>
[AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
internal class SubCommandAttribute(string nameOverride = null) : Attribute
{
    public string NameOverride { get; set; } = nameOverride;
    public string DisplayName { get; set; }
    public string Description { get; set; }
}
