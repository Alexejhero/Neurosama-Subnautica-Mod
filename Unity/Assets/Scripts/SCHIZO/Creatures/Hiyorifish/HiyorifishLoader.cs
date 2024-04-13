using System;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Hiyorifish;
public partial class HiyorifishLoader : CreatureLoader
{
    private const string targetClassId = "hiyorifish";

    [Range(0, 1)]
    public float scanProgressLimit;
    public override TriValidationResult AcceptsItem(ItemData item)
    {
        if (item.classId.Equals(targetClassId, StringComparison.OrdinalIgnoreCase))
            return TriValidationResult.Valid;
        return TriValidationResult.Error($"{nameof(HiyorifishLoader)} is meant only for classid \"{targetClassId}\"");
    }
}
