using System;
using System.Buffers;
using System.Threading;

namespace SCHIZO.Helpers;
internal struct RentedArray<T>(int size, bool clearAfterUse = true) : IDisposable
{
    private int _disposed;
    public bool ClearAfterUse { get; } = clearAfterUse;
    public T[] Array { get; } = ArrayPool<T>.Shared.Rent(size);
    public readonly bool IsDisposed => _disposed != default;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != default)
            return;
        ArrayPool<T>.Shared.Return(Array);
    }
}
