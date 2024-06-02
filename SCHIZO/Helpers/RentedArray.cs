using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCHIZO.Helpers;
internal struct RentedArray<T>(int size, bool clearAfterUse) : IDisposable
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
