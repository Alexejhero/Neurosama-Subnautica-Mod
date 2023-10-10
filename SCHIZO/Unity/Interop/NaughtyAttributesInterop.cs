// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

[Conditional("UNITY")]
internal class ExpandableAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class ShowIfAttribute : Attribute
{
    public ShowIfAttribute(string condition)
    {
    }
}

[Conditional("UNITY")]
internal class HideIfAttribute : Attribute
{
    public HideIfAttribute(string condition)
    {
    }
}

[Conditional("UNITY")]
internal class LabelAttribute : Attribute
{
    public LabelAttribute(string label)
    {
    }
}

[Conditional("UNITY")]
internal class ResizableTextAreaAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class ReorderableListAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class AllowNestingAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class InfoBoxAttribute : Attribute
{
    public InfoBoxAttribute(string text)
    {
    }
}

[Conditional("UNITY")]
internal class BoxGroupAttribute : Attribute
{
    public BoxGroupAttribute(string group)
    {
    }
}

[Conditional("UNITY")]
internal class EnumFlagsAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class ValidateInputAttribute : Attribute
{
    public ValidateInputAttribute(string callbackName, string message = null)
    {
    }
}

[Conditional("UNITY")]
internal class RequiredAttribute : Attribute
{
}

[Conditional("UNITY")]
internal class DropdownAttribute : Attribute
{
    public DropdownAttribute(string valuesName)
    {
    }
}

internal interface IDropdownList : IEnumerable<KeyValuePair<string, object>>
{
}

internal class DropdownList<T> : IDropdownList
{
    public void Add(string displayName, T value)
    {
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield break;
    }
}
