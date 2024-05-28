using System;

namespace SCHIZO.Commands.Output;

public static class CommonResults
{
    // sizeof says 1 byte for the empty types (i'm guessing type marker or pointer padding)
    public readonly record struct OKResult;
    public readonly record struct ShowUsageResult;
    // aaaa i'm having withdrawals i miss rust enums
    public readonly record struct ExceptionResult(Exception Exception);
    // no MessageResult(string) because you can just return a string (may still add one in the future for log levels and such)

    public static OKResult OK() => new();
    public static ShowUsageResult ShowUsage() => new();
    public static ExceptionResult Exception(Exception e) => new(e);
}
