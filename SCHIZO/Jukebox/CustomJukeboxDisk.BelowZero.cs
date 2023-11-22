using System.Text;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Jukebox;

public sealed class CustomJukeboxDisk : JukeboxDisk
{
    public AudioClip unlockSound;

    public new void Start()
    {
        if (track == default) LOGGER.LogWarning($"Jukebox disk {name} at {transform.position} was not assigned a track");

        // the lore is that when you pick up a disk, AL-AN plays a snippet of it in your head
        // if (unlockSound && Story.StoryGoalManager.main!?.GetAlanActor() == Actor.AlAn)
        if (unlockSound) // temp for development/testing // Alex's PR comment: uuh
        {
            int soundLenHash = unlockSound.samples.GetHashCode();
            int soundNameHash = unlockSound.name.GetHashCode();
            string guid = new StringBuilder(32).Insert(0, $"{soundLenHash:x08}{soundNameHash:x08}", 2).ToString();

            if (!CustomSoundHandler.TryGetCustomSound(guid, out _))
            {
                string bus = BusPaths.SFX.GetBusName();
                CustomSoundHandler.RegisterCustomSound(guid, unlockSound, bus, AudioUtils.StandardSoundModes_2D);
            }

            acquireSound = AudioUtils.GetFmodAsset(guid);
        }
        base.Start();
    }
}
