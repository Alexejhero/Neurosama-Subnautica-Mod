using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using SCHIZO.Attributes;
using UnityEngine;

namespace SCHIZO.Commands.Input;

#nullable enable
internal class Vector3Converter : TypeConverter
{
    [InitializeMod]
    private static void RegisterConverter()
    {
        TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3Converter)));
    }

    // scuffed in to only support the current frontend-sent format
    private static readonly HashSet<Type> _canConvertFromDirect = [
        typeof(Vector3),
        typeof(JArray), // bruh
        //typeof(string),
        //typeof(Dictionary<string, object>),
        //typeof(Dictionary<string, string>),
        //typeof(Dictionary<string, int>),
        //typeof(Dictionary<string, float>)
    ];
    private static readonly HashSet<Type> _canConvertFromArray = [
        typeof(string),
        //typeof(int),
        //typeof(float)
    ];
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // we can convert from:
        // Vector3 itself (obviously)
        // an "(x,y,z)" string
        // an {x:1,y:2,z:3} dictionary
        // an [x,y,z] array
        return sourceType.IsArray
            ? _canConvertFromArray.Contains(sourceType.GetElementType())
            : _canConvertFromDirect.Contains(sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(Vector3);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
    {
        if (value is string[] arr && TryConvertStringArray(arr, out Vector3 v))
            return v;
        if (value is JArray { Count: 3 } jArr)
        {
            float[] parsed = new float[3];
            for (int i = 0; i < jArr.Count; i++)
            {
                string part = jArr[i].ToString();
                if (!Conversion.TryParseOrConvert(part, out float f))
                    return default(Vector3);
                parsed[i] = f;
            }
            return new Vector3(parsed[0], parsed[1], parsed[2]);
        }
        return default(Vector3);
    }

    private static bool TryConvertStringArray(string[] arr, out Vector3 value)
    {
        value = default;
        if (arr.Length != 3)
            return false;
        float[] parsed = new float[3];
        for (int i = 0; i < arr.Length; i++)
        {
            string part = arr[i].Trim();
            if (!Conversion.TryParseOrConvert(part, out float f))
                return false;
            parsed[i] = f;
        }
        value = new Vector3(parsed[0], parsed[1], parsed[2]);
        return true;
    }
}
