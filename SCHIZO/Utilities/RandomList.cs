using System.Collections;
using System.Collections.Generic;

namespace SCHIZO.Utilities;

public sealed class RandomList<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    private int _index;

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Shuffle()
    {
        _index = 0;
        _items.Shuffle();
    }

    public T GetRandom()
    {
        if (_items.Count == 0)
        {
            return default;
        }

        if (_index >= _items.Count)
        {
            Shuffle();
        }

        return _items[_index++];
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
