using System;
using Nautilus.Utility;
using SCHIZO.Unity.Creatures;
using SCHIZO.Unity.Sounds;

namespace SCHIZO.Sounds;

[Obsolete]
public sealed class CreatureSounds
{
    public readonly SoundPlayer AmbientItemSounds;
    public readonly SoundPlayer AmbientWorldSounds;

    public readonly SoundPlayer PickupSounds;
    public readonly SoundPlayer DropSounds;

    public readonly SoundPlayer DrawSounds;
    public readonly SoundPlayer HolsterSounds;

    public readonly SoundPlayer CookSounds;
    public readonly SoundPlayer EatSounds;

    public readonly SoundPlayer HurtSounds;
    public readonly SoundPlayer AttackSounds;

    public readonly SoundPlayer ScanSounds;

    public CreatureSounds(CreatureSoundData soundData)
    {
        AmbientItemSounds = CreateSoundPlayer(soundData.ambientItemSounds, AudioUtils.BusPaths.PDAVoice);
        AmbientWorldSounds = CreateSoundPlayer(soundData.ambientWorldSounds, AudioUtils.BusPaths.UnderwaterCreatures);

        PickupSounds = CreateSoundPlayer(soundData.pickupSounds, AudioUtils.BusPaths.PDAVoice);
        DropSounds = CreateSoundPlayer(soundData.dropSounds, AudioUtils.BusPaths.UnderwaterCreatures);

        DrawSounds = CreateSoundPlayer(soundData.drawSounds, AudioUtils.BusPaths.PDAVoice);
        HolsterSounds = CreateSoundPlayer(soundData.holsterSounds, AudioUtils.BusPaths.PDAVoice);

        CookSounds = CreateSoundPlayer(soundData.cookSounds, AudioUtils.BusPaths.PDAVoice);
        EatSounds = CreateSoundPlayer(soundData.eatSounds, AudioUtils.BusPaths.PDAVoice);

        HurtSounds = CreateSoundPlayer(soundData.hurtSounds, AudioUtils.BusPaths.UnderwaterCreatures);
        AttackSounds = CreateSoundPlayer(soundData.attackSounds, AudioUtils.BusPaths.UnderwaterCreatures);

        ScanSounds = CreateSoundPlayer(soundData.scanSounds, AudioUtils.BusPaths.PDAVoice);
    }

    private static SoundPlayer CreateSoundPlayer(BaseSoundCollection sounds, string bus)
    {
        return sounds != null ? new SoundPlayer(sounds, bus) : null;
    }
}
