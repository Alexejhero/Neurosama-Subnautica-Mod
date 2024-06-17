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
        return _args.TryGetValue(name, out object? obj) && Conversion.TryParseOrConvert(obj, out value);
    }
}
