using System;
using Newtonsoft.Json;
using SCHIZO.Commands.Base;
using UnityEngine;

namespace SwarmControl.Models.Game;

#nullable enable
public class ParameterModel(Parameter param) : NamedModel(param)
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
    public Type ActualType { get; set; } = param.Type;
    public object Type { get; set; } = GetTypeToken(param.Type);
    public bool Required { get; set; } = !param.IsOptional;
    public object? DefaultValue { get; set; } = param.HasDefaultValue ? param.DefaultValue : null;

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
