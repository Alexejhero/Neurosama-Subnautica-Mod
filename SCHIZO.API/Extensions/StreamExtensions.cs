using System.IO;

namespace SCHIZO.API.Extensions;

public static class StreamExtensions
{
    public static byte[] ReadFully(this Stream stream)
    {
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
