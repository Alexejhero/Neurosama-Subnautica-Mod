namespace SCHIZO.Sounds;

public static class SoundConfig
{
    public static ISoundConfigProvider Provider { get; set; }
}

public interface ISoundConfigProvider
{
    float MinWorldSoundDelay { get; }
    float MaxWorldSoundDelay { get; }

    float MinInventorySoundDelay { get; }
    float MaxInventorySoundDelay { get; }
}
