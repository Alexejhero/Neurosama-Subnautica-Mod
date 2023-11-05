// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global

#pragma warning disable CS9113 // Parameter is unread.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

public enum EConditionOperator
{
    And,
    Or
}

internal interface IDropdownList : IEnumerable<KeyValuePair<string, object>>;

[Conditional("NEVER")]
public class FoldoutAttribute(string name) : Attribute;
