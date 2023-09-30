namespace SCHIZO.DataStructures;

public class RandomList<T> : IEnumerable<T>
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

    private readonly List<T> _remainingItems = new();
    private readonly List<T> _usedItems = new();
    private readonly IInitialStateModifier _ism;

    private bool _initialized;

    public RandomList(IInitialStateModifier initialStateModifier = null)
    {
        _ism = initialStateModifier ?? new DefaultInitialStateModifier();
    }

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
