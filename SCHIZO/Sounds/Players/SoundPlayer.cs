using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds.Players;

partial class SoundPlayer
{
    public float LastPlay { get; private set; } = -1;

    private List<Coroutine> _runningCoroutines;
    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(StartCoroutine(coroutine));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="delay">Delay (in seconds) before the sound will be played.</param>
    public void Play(float delay = 0)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        if (delay <= 0)
        {
            PlayAttached();
            return;
        }
        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlayAttached();
        }
    }

    public void PlayOneShot(float delay = 0)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        if (delay <= 0)
        {
            PlayDetached();
            return;
        }
        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlayDetached();
        }
    }

    public void CancelAllDelayed()
    {
        _runningCoroutines.ForEach(StopCoroutine);
        _runningCoroutines.Clear();
    }

    private void PlayAttached()
    {
        LastPlay = Time.time;

        emitter.PlayPath(soundEvent, Is3D);
    }

    private void PlayDetached()
    {
        LastPlay = Time.time;

        if (Is3D) RuntimeManager.PlayOneShot(soundEvent, transform.position);
        else RuntimeManager.PlayOneShotAttached(soundEvent, Player.main.gameObject);
    }

    public void Stop()
    {
        emitter!?.Stop();
    }
}
