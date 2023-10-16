// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global

#pragma warning disable CS9113 // Parameter is unread.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

[Conditional("UNITY")]
internal class ExpandableAttribute : Attribute;

[Conditional("UNITY")]
internal class ShowIfAttribute(string condition) : Attribute;

[Conditional("UNITY")]
internal class HideIfAttribute(EConditionOperator conditionOperator, params string[] conditions) : Attribute
{
    public HideIfAttribute(string condition) : this(EConditionOperator.Or, condition)
    {
    }
}

public enum EConditionOperator
{
    And,
    Or
}

[Conditional("UNITY")]
internal class LabelAttribute(string label) : Attribute;

[Conditional("UNITY")]
internal class ResizableTextAreaAttribute : Attribute;

[Conditional("UNITY")]
internal class ReorderableListAttribute : Attribute;

[Conditional("UNITY")]
internal class AllowNestingAttribute : Attribute;

[Conditional("UNITY")]
internal class InfoBoxAttribute(string text) : Attribute;

[Conditional("UNITY")]
internal class BoxGroupAttribute(string group) : Attribute;

[Conditional("UNITY")]
internal class EnumFlagsAttribute : Attribute;

[Conditional("UNITY")]
internal class ValidateInputAttribute(string callbackName, string message = null) : Attribute;

[Conditional("UNITY")]
internal class RequiredAttribute : Attribute;

[Conditional("UNITY")]
internal class DropdownAttribute(string valuesName) : Attribute;

internal interface IDropdownList : IEnumerable<KeyValuePair<string, object>>;

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

[Conditional("UNITY")]
internal class ReadOnlyAttribute : Attribute;

[Conditional("UNITY")]
public class ValidatorAttribute : Attribute;

[Conditional("UNITY")]
public class ButtonAttribute(string text = null) : Attribute;
