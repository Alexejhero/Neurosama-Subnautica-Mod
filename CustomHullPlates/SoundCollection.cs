using System;
using System.Collections.Generic;
using System.IO;
using Nautilus.Handlers;

namespace SCHIZO
{
    public sealed class SoundCollection
    {
        private readonly List<string> _remainingSounds = new List<string>();
        private readonly List<string> _playedSounds = new List<string>();

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

        public void PlayOne()
        {
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
