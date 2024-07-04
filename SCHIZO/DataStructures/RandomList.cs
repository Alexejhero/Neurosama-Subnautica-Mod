using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.DataStructures;

public class RandomList<T>(RandomList<T>.IInitialStateModifier initialStateModifier = null) : IReadOnlyCollection<T>
{
    public interface IInitialStateModifier
    {
        bool Register(T value);
        void MarkUsed(T value);
        void Reset();
    }

    private sealed class DefaultInitialStateModifier : IInitialStateModifier
    {
        public bool Register(T value) => false;

        public void MarkUsed(T value)
        {
        }

        public void Reset()
        {
        }
    }

    private readonly List<T> _remainingItems = [];
    private readonly List<T> _usedItems = [];
    private readonly IInitialStateModifier _ism = initialStateModifier ?? new DefaultInitialStateModifier();

    private bool _initialized;

    public int Count => _remainingItems.Count + _usedItems.Count;

    public void Add(T value)
    {
        bool used = _ism.Register(value);
        if (used) _usedItems.Add(value);
        else _remainingItems.Add(value);
    }

    public void AddRange(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            Add(value);
        }
    }

    public T GetRandom()
    {
        if (_remainingItems.Count == 0 && _usedItems.Count == 0) throw new InvalidOperationException("No items in list");

        if (!_initialized) Initialize();
        if (_remainingItems.Count == 0) Shuffle();

        T item = _remainingItems[0];
        _remainingItems.RemoveAt(0);
        _usedItems.Add(item);
        _ism.MarkUsed(item);
        return item;
    }

    private void Shuffle()
    {
        _remainingItems.AddRange(_usedItems);
        _remainingItems.Shuffle();
        _usedItems.Clear();
        _ism.Reset();
    }

    private void Initialize()
    {
        _remainingItems.Shuffle();
        _initialized = true;
    }

    public IEnumerator<T> GetEnumerator() => _remainingItems.Concat(_usedItems).ToList().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
