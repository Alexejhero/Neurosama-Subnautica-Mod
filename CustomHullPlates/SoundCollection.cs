using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO
{
    public sealed class SoundCollection
    {
        private readonly List<string> _remainingSounds = new List<string>();
        private readonly List<string> _playedSounds = new List<string>();

        public float lastPlay { get; private set; } = -1;

        private readonly List<Coroutine> _runningCoroutines = new List<Coroutine>();

        public SoundCollection(string dirpath, string bus)
        {
            foreach (string soundFile in Directory.GetFiles(dirpath))
            {
                string id = Guid.NewGuid().ToString();
                CustomSoundHandler.RegisterCustomSound(id, soundFile, bus);
                _remainingSounds.Add(id);
            }

            _remainingSounds.Shuffle();
        }

        public void Play(float delay = 0)
        {
            if (SchizoPlugin.config.DisableAllNoises) return;

            if (delay == 0)
            {
                PlaySound();
                return;
            }

            IEnumerator PlayWithDelay(float del)
            {
                yield return new WaitForSeconds(del);
                PlaySound();
            }

            Coroutine c = GameInput.instance.StartCoroutine(PlayWithDelay(delay));
            _runningCoroutines.Add(c);
        }

        public void CancelAllDelayed()
        {
            foreach (Coroutine c in _runningCoroutines)
            {
                GameInput.instance.StopCoroutine(c);
            }

            _runningCoroutines.Clear();
        }

        private void PlaySound()
        {
            lastPlay = Time.time;

            if (_remainingSounds.Count == 0)
            {
                _remainingSounds.AddRange(_playedSounds);
                _playedSounds.Clear();
            }

            CustomSoundHandler.TryPlayCustomSound(_remainingSounds[0], out _);
            _playedSounds.Add(_remainingSounds[0]);
            _remainingSounds.RemoveAt(0);
        }
    }
}
