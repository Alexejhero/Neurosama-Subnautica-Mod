using NaughtyAttributes;
using SCHIZO.Sounds;

namespace SCHIZO.Creatures.Components
{
    public partial class GetCarried : CustomCreatureAction
    {
        [BoxGroup("Sounds")]
        public float carryNoiseInterval = 5f;
        [BoxGroup("Sounds")]
        public BaseSoundCollection pickupSounds;
        [BoxGroup("Sounds")]
        public BaseSoundCollection carrySounds;
        [BoxGroup("Sounds")]
        public BaseSoundCollection releaseSounds;
    }
}
