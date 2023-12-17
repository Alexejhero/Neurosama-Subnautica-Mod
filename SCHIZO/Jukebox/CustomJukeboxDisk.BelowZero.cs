using Nautilus.Handlers;
using Nautilus.Utility;

namespace SCHIZO.Jukebox;

public sealed class CustomJukeboxDisk : JukeboxDisk
{
    public string unlockFmodEvent;

    public new void Start()
    {
        if (track == default) LOGGER.LogWarning($"Jukebox disk {name} at {transform.position} was not assigned a track");

        if (!string.IsNullOrEmpty(unlockFmodEvent))
            acquireSound = AudioUtils.GetFmodAsset(unlockFmodEvent);

        base.Start();
    }
}
