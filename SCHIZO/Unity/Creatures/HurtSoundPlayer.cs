using Nautilus.Utility;
using SCHIZO.Sounds;

namespace SCHIZO.Unity.Creatures;

partial class HurtSoundPlayer : IOnTakeDamage
{
    private FMODSoundCollection _fmodSounds;

    private void Awake()
    {
        _fmodSounds = new FMODSoundCollection(hurtSounds, AudioUtils.BusPaths.UnderwaterCreatures);
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage == 0) return;
        _fmodSounds.Play((FMOD_CustomEmitter) emitter);
    }
}
