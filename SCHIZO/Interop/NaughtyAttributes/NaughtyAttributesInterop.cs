// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global

#pragma warning disable CS9113 // Parameter is unread.

using System;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

[Obsolete]
public enum EConditionOperator
{
    And,
    Or
}

[Obsolete]
internal interface IDropdownList : IEnumerable<KeyValuePair<string, object>>;

[Conditional("NEVER")]
[Obsolete]
public class FoldoutAttribute(string name) : Attribute;
