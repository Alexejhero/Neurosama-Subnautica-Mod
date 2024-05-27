using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SCHIZO.Commands.Input;

public static class StringConverters
{
    public delegate (bool Success, object Value) TryConverter(string str);

    public static readonly Dictionary<Type, TryConverter> CustomConverters = [];
    private static readonly Dictionary<Type, TypeConverter> _cachedConverters = [];

    public static void RegisterCustomConverter(Type type, TryConverter converter)
    {
        CustomConverters[type] = converter;
    }

    private static readonly NumberFormatInfo _numberFormat = CultureInfo.InvariantCulture.NumberFormat;
    public static (bool Success, object Value) TryConvert(string value, Type type)
    {
        // TypeConverter might cover all of these (we'll just have to eat the first-time cost)
        if (type == typeof(string)) return (true, value);
        if (type == typeof(bool)) return (bool.TryParse(value, out bool b), b);
        if (type == typeof(int)) return (int.TryParse(value, NumberStyles.Integer, _numberFormat, out int i), i);
        if (type == typeof(float)) return (float.TryParse(value, NumberStyles.Float, _numberFormat, out float f), f);
        if (type == typeof(double)) return (double.TryParse(value, NumberStyles.Float, _numberFormat, out double d), d);
        if (typeof(Enum).IsAssignableFrom(type))
        {
            // no Enum.TryParse(Type, String, out object) in netfx...
            try
            {
                // nautilus patches Enum.Parse so this will handle custom enum values (e.g. modded techtypes) as well
                return (true, Enum.Parse(type, value));
            }
            catch
            {
                return (false, default);
            }
        }

        if (CustomConverters.TryGetValue(type, out TryConverter customConverter))
            return customConverter(value);

        // last resort, this does a bunch of reflection internally so it's slow(er) on first call
        if (!_cachedConverters.TryGetValue(type, out TypeConverter converter))
            converter = _cachedConverters[type] = TypeDescriptor.GetConverter(type);
        if (converter is { } && converter.CanConvertFrom(typeof(string)))
            return (true, converter.ConvertFromInvariantString(value));
        return (false, default);
    }
}
