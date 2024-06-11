using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Nautilus.Extensions;
using SwarmControl.Models.Game;

namespace SCHIZO.Commands.Base;
#nullable enable
public class Parameter : NamedModel
{
    public Type Type { get; internal set; }
    public Type UnderlyingValueType { get; internal set; }
    public bool IsOptional { get; internal set; }
    public bool HasDefaultValue => _defaultValue != DBNull.Value;
    private object? _defaultValue = DBNull.Value;
    /// <summary>
    /// Default value for this parameter.<br/>
    /// </summary>
    /// <remarks>
    /// If not set, this will be <see cref="DBNull.Value"/>, to differentiate from an explicitly set <see langword="null"/>.<br/>
    /// If <see cref="IsOptional"/> is <see langword="true"/>, this will return <see cref="Type.Missing"/> instead (see <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.type.missing">documentation</seealso>).<br/>
    /// See <see cref="ParameterInfo.DefaultValue"/> for more info (specifically the <seealso href="https://referencesource.microsoft.com/#mscorlib/system/reflection/parameterinfo.cs,569">reference source</seealso>).
    /// </remarks>
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

    [SetsRequiredMembers]
    public Parameter(NamedModel name, Type type, bool isOptional = false)
        : this(name, type, DBNull.Value)
    {
        IsOptional = isOptional;
    }
    [SetsRequiredMembers]
    public Parameter(NamedModel name, Type type, object? defaultValue)
        : base(name)
    {
        Type = type;
        UnderlyingValueType = type.GetUnderlyingType();
        DefaultValue = defaultValue;
    }
    [SetsRequiredMembers]
    public Parameter(ParameterInfo info)
        : this(info.Name, info.ParameterType, info.IsOptional)
    {
        DefaultValue = info.DefaultValue;
    }
}
