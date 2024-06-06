using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SCHIZO.Commands.Input;

#nullable enable
public class NamedArgs(Dictionary<string, object?> args)
{
    private readonly Dictionary<string, object?> _args = args;
    public bool TryGetValue<T>(string name, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        return _args.TryGetValue(name, out object? obj) && TryParseOrConvert(obj, out value);
    }

    public bool TryParseOrConvert<T>(object? val, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        if (typeof(T).IsEnum)
        {
            return TryParseEnum(val, out value);
        }
        if (val is T t)
        {
            value = t;
            return true;
        }
        // like Convert.ChangeType but you can register your own converters (somewhere) (i've heard)
        TypeConverter? converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter is null)
            return false;
        try
        {
            value = (T)converter.ConvertFrom(val);
            return true;
        } catch { }
        try
        {
            value = (T) Convert.ChangeType(val, typeof(T));
            return true;
        } catch { }

        return false;
    }

    private bool TryParseEnum<TEnum>(object? val, [MaybeNullWhen(false)] out TEnum value)
    {
        value = default;
        if (val is null)
            return false;
        if (val is string str)
        {
            try
            {
                value = (TEnum) Enum.Parse(typeof(TEnum), str, true);
                return true;
            } catch { return false; }
        }
        else
        {
            try
            {
                value = (TEnum) Enum.ToObject(typeof(TEnum), val);
                return true;
            } catch { return false; }
        }
    }
}
