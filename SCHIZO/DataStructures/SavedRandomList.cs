using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.DataStructures;

public sealed class SavedRandomList<T> : IEnumerable<T>
{
    private record struct IdentifiableItem(string Identifier, T Value);

    private class PlayerPrefsManager(string playerPrefsPrefix) : RandomList<IdentifiableItem>.IInitialStateModifier
    {
        private record struct RegistryKey(string Value);

        private readonly HashSet<string> _identifiers = [];

        public bool Register(IdentifiableItem item)
        {
            string identifier = item.Identifier;

            if (_identifiers.Contains(identifier)) throw new InvalidOperationException($"Duplicate identifier: {identifier}");
            _identifiers.Add(identifier);

            // possible gotcha to look out for: PlayerPrefsExtra stores some values like Vectors in multiple keys, so HasKey might return false even if the keys exist
            // not the case for booleans though
            RegistryKey key = KeyOf(identifier);
            if (Contains(key)) return GetState(key);

            SetState(key, false);
            return false;
        }

        public void MarkUsed(IdentifiableItem item)
        {
            SetState(KeyOf(item.Identifier), true);
        }

        public void Reset()
        {
            foreach (string identifier in _identifiers)
            {
                SetState(KeyOf(identifier), false);
            }
        }

        private RegistryKey KeyOf(string identifier) => new($"SCHIZO_RandomList_{playerPrefsPrefix}_{identifier}");

        private static bool Contains(RegistryKey key) => PlayerPrefs.HasKey(key.Value);

        private static bool GetState(RegistryKey key) => PlayerPrefsExtra.GetBool(key.Value, default);

        private static void SetState(RegistryKey key, bool used) => PlayerPrefsExtra.SetBool(key.Value, used);
    }

    private readonly RandomList<IdentifiableItem> _randomList;

    public SavedRandomList(string playerPrefsKey)
    {
        if (string.IsNullOrWhiteSpace(playerPrefsKey)) throw new ArgumentNullException(nameof(playerPrefsKey));

        _randomList = new RandomList<IdentifiableItem>(new PlayerPrefsManager(playerPrefsKey));
    }

    public T this[object id]
    {
        set
        {
            string identifier = id.ToString();

            if (_randomList.Any(e => e.Identifier == identifier))
                throw new InvalidOperationException($"Identifier already exists: {identifier}");

            _randomList.Add(new IdentifiableItem(identifier, value));
        }
    }

    public T GetRandom()
    {
        return _randomList.GetRandom().Value;
    }

    public IEnumerator<T> GetEnumerator() => _randomList.Select(item => item.Value).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
