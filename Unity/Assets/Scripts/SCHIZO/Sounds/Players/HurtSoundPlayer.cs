namespace SCHIZO.Sounds.Players
{
    public sealed partial class HurtSoundPlayer : SoundPlayer
    {
        protected override string DefaultBus => buses[UNDERWATER_CREATURES];
    }
}
