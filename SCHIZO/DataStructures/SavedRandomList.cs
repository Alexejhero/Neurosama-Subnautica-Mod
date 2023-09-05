using System;
using System.Collections;
using System.Collections.Generic;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.DataStructures;

public sealed class SavedRandomList<T> : IEnumerable
{
    private record struct Item(string Identifier, T Value);

    private class PlayerPrefsManager : RandomList<Item>.IInitialStateModifier
    {
        private record struct RegistryKey(string Value);

        private readonly string _key;
        private readonly HashSet<string> _identifiers = new();

        public PlayerPrefsManager(string key)
        {
            _key = key;
        }

        public bool Register(Item item)
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

        public void MarkUsed(Item item)
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

        private bool Contains(RegistryKey key) => PlayerPrefs.HasKey(key.Value);

        private bool GetState(RegistryKey key) => PlayerPrefsExtra.GetBool(key.Value, default);

        private void SetState(RegistryKey key, bool used) => PlayerPrefsExtra.SetBool(key.Value, used);

        private RegistryKey KeyOf(string identifier) => new($"SCHIZO_RandomList_{_key}_{identifier}");
    }

    private readonly RandomList<Item> _randomList;

    public SavedRandomList(string playerPrefsKey)
    {
        if (string.IsNullOrWhiteSpace(playerPrefsKey)) throw new ArgumentNullException(nameof(playerPrefsKey));

        _randomList = new RandomList<Item>(new PlayerPrefsManager(playerPrefsKey));
    }

    public void Add(object id, T value)
    {
        _randomList.Add(new Item(id.ToString(), value));
    }

    public T GetRandom()
    {
        return _randomList.GetRandom().Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();
}
