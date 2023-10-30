using SCHIZO.Sounds;

namespace SCHIZO.Creatures.Components
{
    public sealed partial class HurtSoundPlayer : SoundPlayer
    {
        protected override string DefaultBus => buses[UNDERWATER_CREATURES];
        protected override bool Is3D => true;
    }
}
