using System;
using System.Diagnostics.CodeAnalysis;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.Commands.Base;
#nullable enable
internal class TextParameter : Parameter
{
    [SetsRequiredMembers]
    public TextParameter(NamedModel name, bool isOptional = false)
        : base(name, typeof(string), isOptional)
    { }
    [SetsRequiredMembers]
    public TextParameter(NamedModel name, string? defaultValue)
        : base(name, typeof(string), defaultValue)
    { }
    public int MinLength { get; init; }
    public int MaxLength { get; init; }
}
