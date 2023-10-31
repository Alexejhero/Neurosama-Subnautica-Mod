using System.Reflection;
using System.Runtime.CompilerServices;
using Nautilus.Commands;

[assembly: IgnoresAccessChecksTo("Nautilus")]

namespace SCHIZO.Console.BetterThanNautilus;

public class ConsoleCommandEx : ConsoleCommand
{
    public ConsoleCommandEx(string trigger, MethodInfo targetMethod, bool isDelegate = false, object instance = null) : base(trigger, targetMethod, isDelegate, instance)
    {
    }
}
