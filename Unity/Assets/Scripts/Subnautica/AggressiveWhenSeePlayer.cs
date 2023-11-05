using SCHIZO.Interop.Subnautica.Enums;
using TriInspector;

public class AggressiveWhenSeePlayer : AggressiveWhenSeeTarget
{
    [PropertyOrder(11)] public float playerAttackInterval;

    protected override bool ShowPlayerAttackInfobox() => targetType != EcoTargetType_All.None;
}
