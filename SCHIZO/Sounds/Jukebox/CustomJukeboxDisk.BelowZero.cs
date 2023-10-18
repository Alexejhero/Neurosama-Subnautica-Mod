using System.Text;
using Nautilus.Handlers;
using Nautilus.Utility;
using Story;
using UnityEngine;

namespace SCHIZO.Sounds.Jukebox_;
partial class CustomJukeboxDisk
{
    public CustomJukeboxTrack _track;
    public AudioClip unlockSound;

    public new void Start()
    {
        if (_track) track = _track;
        if (track == default) LOGGER.LogWarning($"Jukebox disk {name} at {transform.position} was not assigned a track");

        // the lore is that when you pick up a disk, AL-AN plays a snippet of it in your head
        //if (unlockSound && Story.StoryGoalManager.main!?.GetAlanActor() == Actor.AlAn)
        if (unlockSound) // temp for development/testing
        {
            int soundLenHash = unlockSound.samples.GetHashCode();
            int soundNameHash = unlockSound.name.GetHashCode();
            string guid = new StringBuilder(32).Insert(0, $"{soundLenHash:x08}{soundNameHash:x08}", 2).ToString();

            if (!CustomSoundHandler.TryGetCustomSound(guid, out _))
                CustomSoundHandler.RegisterCustomSound(guid, unlockSound, "bus:/master/SFX_for_pause/PDA_pause/jukebox", AudioUtils.StandardSoundModes_2D);

            acquireSound = AudioUtils.GetFmodAsset(guid);
        }
        base.Start();
    }
}
