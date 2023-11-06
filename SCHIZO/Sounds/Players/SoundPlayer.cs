namespace SCHIZO.Sounds.Players;

partial class SoundPlayer
{
    public void Play(float delay = 0)
    {
        if (Is3D) sounds.PlayRandom3D(emitter, delay);
        else sounds.PlayRandom2D(delay);
    }

    public void Stop()
    {
        if (Is3D) emitter.Stop();
        else sounds.Stop();
    }
}
