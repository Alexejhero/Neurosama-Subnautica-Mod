using System.Collections;
using System.Collections.Generic;

namespace SCHIZO.DataStructures;

public sealed class RandomList<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    private int _index;
    private bool _needsShuffling;

    public void Add(T item)
    {
        _items.Add(item);
        _needsShuffling = true;
    }

    public void Shuffle()
    {
        _index = 0;
        _items.Shuffle();
        _needsShuffling = false;
    }

    public T GetRandom()
    {
        if (_items.Count == 0) return default;
        if (_index >= _items.Count || _needsShuffling) Shuffle();
        return _items[_index++];
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
