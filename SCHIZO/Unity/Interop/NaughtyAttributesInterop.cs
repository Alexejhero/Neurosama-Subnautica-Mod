// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedType.Global

using System;

// ReSharper disable once CheckNamespace
namespace NaughtyAttributes;

public class ExpandableAttribute : Attribute
{
}

public class ShowIfAttribute : Attribute
{
    public ShowIfAttribute(string condition)
    {
    }
}

public class HideIfAttribute : Attribute
{
    public HideIfAttribute(string condition)
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

public class InfoBoxAttribute : Attribute
{
    public InfoBoxAttribute(string text)
    {
    }
}
