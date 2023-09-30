using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures;

public abstract class PickupableCreatureLoader
{
    protected readonly PickupableCreatureData _creatureData;
    protected readonly CreatureSounds _creatureSounds;

    protected PickupableCreatureLoader(PickupableCreatureData data)
    {
        _creatureData = data;
        _creatureSounds = new CreatureSounds(data.soundData);
    }
}
