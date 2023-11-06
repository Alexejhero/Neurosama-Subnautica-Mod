using SCHIZO.Interop.Subnautica;
using SCHIZO.TriInspector.Attributes;
using TriInspector;

public class AttackCyclops : CreatureAction
{
    [ComponentReferencesGroup, Required] public LastTarget lastTarget;
    public float aggressPerSecond = 0.5f;
    public float attackAggressionThreshold = 0.75f;
    public float attackPause = 5;
    public float maxDistToLeash = 30;
    public float swimVelocity = 10;
    public float swimInterval = 0.8f;
    public _CreatureTrait aggressiveToNoise = new _CreatureTrait(0, 0.06f);
}
