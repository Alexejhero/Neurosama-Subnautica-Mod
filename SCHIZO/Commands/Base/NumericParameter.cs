using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.Commands.Base;
[method: SetsRequiredMembers]
internal class NumericParameter(NamedModel name, bool integer, bool isOptional = false)
    : Parameter(name, integer ? typeof(int) : typeof(float), isOptional)
{
    [DefaultValue(float.MinValue), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public float Min { get; init; } = float.MinValue;
    [DefaultValue(float.MaxValue)]
    public float Max { get; init; } = float.MaxValue;
}
