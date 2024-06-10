using System;
using Newtonsoft.Json;
using SCHIZO.Commands.Base;
using UnityEngine;

namespace SwarmControl.Shared.Models.Game;

#nullable enable
public class ParameterModel : NamedModel
{
    public enum ParameterType
    {
        String,
        Integer,
        Float,
        Boolean,
        Vector3
    }
    [JsonIgnore]
    public Type ActualType { get; set; }
    public object Type { get; set; }
    public bool Required { get; set; }
    public object? DefaultValue { get; set; }

    public ParameterModel(Parameter param)
        : base(param)
    {
        ActualType = param.Type;
        Type = GetTypeToken(param.Type);
        Required = !param.IsOptional;
        DefaultValue = param.HasDefaultValue ? param.DefaultValue : null;
    }

    private static object GetTypeToken(Type type)
    {
        if (type.IsEnum)
            return $"{char.ToLowerInvariant(type.Name[0])}{type.Name[1..]}"; // i cba to deal with json naming
        if (type == typeof(string))
            return ParameterType.String;
        if (type == typeof(int) || type == typeof(long))
            return ParameterType.Integer;
        if (type == typeof(float) || type == typeof(double))
            return ParameterType.Float;
        if (type == typeof(bool))
            return ParameterType.Boolean;
        if (type == typeof(Vector3))
            return ParameterType.Vector3;
        return "unk_"+type.Name;
    }
}
