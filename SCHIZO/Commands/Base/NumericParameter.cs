using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.Commands.Base;

#nullable enable
internal class NumericParameter : Parameter
{
    [DefaultValue(float.MinValue), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public float Min { get; init; } = float.MinValue;
    [DefaultValue(float.MaxValue)]
    public float Max { get; init; } = float.MaxValue;

    [SetsRequiredMembers]
    public NumericParameter(NamedModel name, bool integer, bool isOptional = false)
        : base(name, integer ? typeof(int) : typeof(float), isOptional)
    { }
    [SetsRequiredMembers]
    public NumericParameter(NamedModel name, bool integer, float defaultValue)
        : base(name, integer ? typeof(int) : typeof(float), defaultValue)
    { }
}
