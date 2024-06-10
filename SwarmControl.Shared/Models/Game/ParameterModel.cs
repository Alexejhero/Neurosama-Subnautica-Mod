using System;
using Newtonsoft.Json;
using SCHIZO.Commands.Base;

namespace SwarmControl.Shared.Models.Game;

#nullable enable
public class ParameterModel : NamedModel
{
    [JsonIgnore]
    public Type ActualType { get; set; }
    public string Type { get; set; }
    public bool Required { get; set; }
    public object? DefaultValue { get; set; }

    public ParameterModel(Parameter param)
        : base(param)
    {
        ActualType = param.Type;
        Type = GetTypeName(param.Type);
        Required = !param.IsOptional;
        DefaultValue = param.HasDefaultValue ? param.DefaultValue : null;
    }

    private static string GetTypeName(Type type)
    {
        if (type.IsEnum)
            return type.Name;
        if (type == typeof(string))
            return "string";
        if (type == typeof(int) || type == typeof(long))
            return "integer";
        if (type == typeof(float) || type == typeof(double))
            return "float";
        if (type == typeof(bool))
            return "boolean";
        return "unk_"+type.Name;
    }
}
