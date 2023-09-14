using System.Collections;
using Nautilus.Handlers;
using UnityEngine;
using FMODUnity;

namespace SCHIZO.Sounds;

public sealed class SoundCollection2D : SoundCollection
{
    private SoundCollection2D() {}

    public static SoundCollection2D Create(string path, string bus)
    {
        SoundCollection2D result = CreateInstance<SoundCollection2D>();
        result.Initialize(path, bus);
        return result;
    }

    protected override void RegisterSound(string id, string soundFile, string bus)
    {
        CustomSoundHandler.RegisterCustomSound(id, soundFile, bus);
        RuntimeManager.GetBus(bus).unlockChannelGroup();
    }

    public void Play(float delaySeconds = 0)
    {
        if (CONFIG.DisableAllNoises) return;

        if (delaySeconds == 0)
        {
            PlaySound();
            return;
        }

        StartSoundCoroutine(PlayWithDelay(delaySeconds));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlaySound();
        }
    }

    private void PlaySound()
    {
        LastPlay = Time.time;

        CustomSoundHandler.TryPlayCustomSound(_sounds.GetRandom(), out _);
    }
}
