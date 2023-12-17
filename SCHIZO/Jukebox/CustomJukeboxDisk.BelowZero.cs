namespace SCHIZO.Jukebox;

public sealed class CustomJukeboxDisk : JukeboxDisk
{
    public new void Start()
    {
        if (track == default)
        {
            LOGGER.LogError($"Jukebox disk {name} at {transform.position} was not assigned a track, self-destructing");
            Destroy(this);
        }

        base.Start();
    }
}
