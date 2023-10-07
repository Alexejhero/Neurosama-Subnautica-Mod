using NaughtyAttributes;

public class AttackCyclops : CreatureAction
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LastTarget lastTarget;
    public float aggressPerSecond = 0.5f;
    public float attackAggressionThreshold = 0.75f;
    public float attackPause = 5;
    public float maxDistToLeash = 30;
    public float swimVelocity = 10;
    public float swimInterval = 0.8f;
    public CreatureTrait aggressiveToNoise = new CreatureTrait(0, 0.06f);
}
