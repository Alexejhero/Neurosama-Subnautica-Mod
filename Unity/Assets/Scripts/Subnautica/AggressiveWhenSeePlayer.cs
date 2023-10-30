using NaughtyAttributes;

public class AggressiveWhenSeePlayer : AggressiveWhenSeeTarget
{
    [InfoBox("If Target Type is not set to None, other targets will be attacked if the player cannot be attacked.")]
    public float playerAttackInterval;
}
