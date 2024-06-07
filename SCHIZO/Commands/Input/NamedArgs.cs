using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SCHIZO.Commands.Input;

#nullable enable
public class NamedArgs(Dictionary<string, object?> args)
{
    private readonly Dictionary<string, object?> _args = args;
    public object? this[string name]
    {
        get => _args[name];
        set => _args[name] = value;
    }

    public T GetOrDefault<T>(string name, T def = default!)
    {
        if (TryGetValue(name, out T? val))
            return val!;
        return def;
    }
    public bool TryGetValue<T>(string name, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        return _args.TryGetValue(name, out object? obj) && TryParseOrConvert(obj, out value);
    }

    // todo pull these out
    public static bool TryParseOrConvert<T>(object? val, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        if (TryParseOrConvert(val, typeof(T), out object? obj))
        {
            value = (T)obj!;
            return true;
        }
        return false;
    }

    public static bool TryParseOrConvert(object? val, Type type, [MaybeNullWhen(false)] out object? value)
    {
        value = default;
        if (type.IsEnum)
        {
            return TryParseEnum(val, type, out value);
        }
        if (val is null)
            return false;
        if (type.IsAssignableFrom(val.GetType()))
        {
            value = val;
            return true;
        }
        // like Convert.ChangeType but you can register your own converters (somewhere) (i've heard)
        TypeConverter? converter = TypeDescriptor.GetConverter(type);
        if (converter is null)
            return false;
        try
        {
            value = converter.ConvertFrom(val);
            return true;
        }
        catch { }
        try
        {
            value = Convert.ChangeType(val, type);
            return true;
        }
        catch { }

        return false;
    }

    public static bool TryParseEnum<TEnum>(object? val, [MaybeNullWhen(false)] out TEnum value)
    {
        value = default;
        if (TryParseEnum(val, typeof(TEnum), out object? obj))
        {
            value = (TEnum)obj!;
            return true;
        }
        return false;
    }

    public static bool TryParseEnum(object? val, Type type, [MaybeNullWhen(false)] out object? value)
    {
        value = default;
        if (val is null)
            return false;
        if (val is string str)
        {
            try
            {
                value = Enum.Parse(type, str, true);
                return true;
            } catch { return false; }
        }
        else
        {
            try
            {
                value = Enum.ToObject(type, val);
                return true;
            } catch { return false; }
        }
    }
}
