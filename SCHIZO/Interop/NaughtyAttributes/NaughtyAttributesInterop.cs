// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global

#pragma warning disable CS9113 // Parameter is unread.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

[Conditional("NEVER")]
internal class ExpandableAttribute : Attribute;

[Conditional("NEVER")]
internal class ShowIfAttribute(string condition) : Attribute;

[Conditional("NEVER")]
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

[Conditional("NEVER")]
internal class LabelAttribute(string label) : Attribute;

[Conditional("NEVER")]
internal class ResizableTextAreaAttribute : Attribute;

[Conditional("NEVER")]
internal class ReorderableListAttribute : Attribute;

[Conditional("NEVER")]
internal class AllowNestingAttribute : Attribute;

[Conditional("NEVER")]
internal class InfoBoxAttribute(string text) : Attribute;

[Conditional("NEVER")]
internal class BoxGroupAttribute(string group) : Attribute;

[Conditional("NEVER")]
internal class EnumFlagsAttribute : Attribute;

[Conditional("NEVER")]
internal class ValidateInputAttribute(string callbackName, string message = null) : Attribute;

[Conditional("NEVER")]
internal class RequiredAttribute(string message = null) : Attribute;

[Conditional("NEVER")]
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

[Conditional("NEVER")]
internal class ReadOnlyAttribute : Attribute;

[Conditional("NEVER")]
internal class ValidatorAttribute : Attribute;

[Conditional("NEVER")]
internal class ButtonAttribute(string text = null) : Attribute;
