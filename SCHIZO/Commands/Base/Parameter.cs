using System;
using System.Reflection;
using Nautilus.Extensions;

namespace SCHIZO.Commands.Base;
#nullable enable
public class Parameter
{
    public string Name { get; internal set; }
    public string? DisplayName { get; internal set; }
    public string? Description { get; internal set; }
    public Type Type { get; internal set; }
    public Type UnderlyingValueType { get; internal set; }
    public bool IsOptional { get; internal set; }
    public bool HasDefaultValue => _defaultValue == DBNull.Value;
    private object? _defaultValue = DBNull.Value;
    public object? DefaultValue
    {
        get
        {
            // https://referencesource.microsoft.com/#mscorlib/system/reflection/parameterinfo.cs,569
            object? value = _defaultValue;
            return IsOptional && value == DBNull.Value
                ? Type.Missing
                : value;
        }
        set => _defaultValue = value;
    }

    public Parameter(string name, Type type, bool isOptional = false, string? displayName = null, string? description = null)
        : this(name, type, DBNull.Value, displayName, description)
    {
        IsOptional = isOptional;
    }
    public Parameter(string name, Type type, object? defaultValue, string? displayName = null, string? description = null)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Type = type;
        UnderlyingValueType = type.GetUnderlyingType();
        DefaultValue = defaultValue;
    }
    public Parameter(ParameterInfo info)
        : this(info.Name, info.ParameterType, info.IsOptional)
    {
        DefaultValue = info.DefaultValue;
    }
}
