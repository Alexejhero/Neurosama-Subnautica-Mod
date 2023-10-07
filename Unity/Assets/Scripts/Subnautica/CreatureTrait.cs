using System;

[Serializable]
public class CreatureTrait
{
    public float value;
    public float falloff;

    public CreatureTrait(float value, float falloff)
    {
        this.value = value;
        this.falloff = falloff;
    }
}
