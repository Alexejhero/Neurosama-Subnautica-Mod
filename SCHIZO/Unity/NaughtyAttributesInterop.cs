using System;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

public enum EConditionOperator
{
    And,
    Or
}

public class ExpandableAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class ShowIfAttribute : Attribute
{
    public ShowIfAttribute(string condition)
    {
    }

    public ShowIfAttribute(EConditionOperator conditionOperator, params string[] conditions)
    {
    }

    public ShowIfAttribute(string enumName, object enumValue)
    {
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class HideIfAttribute : Attribute
{
    public HideIfAttribute(string condition)
    {
    }

    public HideIfAttribute(EConditionOperator conditionOperator, params string[] conditions)
    {
    }

    public HideIfAttribute(string enumName, object enumValue)
    {
    }
}

public class LabelAttribute : Attribute
{
    public LabelAttribute(string label)
    {
    }
}

public class ResizableTextAreaAttribute : Attribute
{
}

public class ReorderableListAttribute : Attribute
{
}

public class AllowNestingAttribute : Attribute
{
}
