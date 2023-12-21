using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds.Players;

partial class SoundPlayer
{
    public float LastPlay { get; private set; } = -1;

    private List<EventInstance> _playingEvents = [];
    private float _eventCleanupInterval = 1f;
    private float _nextUpdate;
    private List<Coroutine> _runningCoroutines;
    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        // not started on the object because it might be disabled (inventory sounds)
        _runningCoroutines.Add(CoroutineHost.StartCoroutine(coroutine));
    }
    protected virtual void FixedUpdate()
    {
        if (Time.time < _nextUpdate) return;
        _nextUpdate = Time.time + _eventCleanupInterval;
        foreach (EventInstance evt in _playingEvents)
        {
            if (evt.getPlaybackState(out PLAYBACK_STATE state) != FMOD.RESULT.OK
                || state == PLAYBACK_STATE.STOPPED)
                evt.release();
        }
        _playingEvents.RemoveAll(evt => !evt.isValid());
    }

    /// <summary>
    /// Play the <see cref="soundEvent">soundEvent</see>, attached to the <see cref="SoundPlayer"/>.
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
    /// <summary>
    /// Play the <see cref="soundEvent">soundEvent</see>, detached from the <see cref="SoundPlayer"/>.
    /// </summary>
    /// <remarks>
    /// Since the event instance is started then detached, the sound player does not own it.<br/>
    /// This means that e.g. it will not stop when <see cref="Stop"/> is called, even if it's called on the <see cref="SoundPlayer"/> that started playing the event.
    /// </remarks>
    /// <param name="delay">Delay (in seconds) before the sound will be played.</param>
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
        _runningCoroutines.ForEach(CoroutineHost.StopCoroutine);
        _runningCoroutines.Clear();
    }

    private void PlayAttached()
    {
        LastPlay = Time.time;

        if (emitter) emitter.PlayPath(soundEvent, Is3D);
        else
        {
            _playingEvents.Add(Is3D
                ? FMODHelpers.PlayPath2D(soundEvent)
                : FMODHelpers.PlayPath3DAttached(soundEvent, transform));
        }
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
        _playingEvents.ForEach(FMODHelpers.StopAndRelease);
        _playingEvents.Clear();
    }
}
