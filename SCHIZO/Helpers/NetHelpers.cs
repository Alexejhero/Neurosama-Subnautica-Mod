using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace SCHIZO.Helpers;
internal static class NetHelpers
{
    // windows says these ports are dynamic (netsh int ipv4>show dynamic protocol=tcp)
    public const ushort MinPrivatePort = 49152;
    public const ushort MaxPrivatePort = 65535;
    public static ushort? TryGetFreePort(ushort min = MinPrivatePort, ushort max = MaxPrivatePort)
    {
        HashSet<int> usedPorts = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            .Select(ep => ep.Port)
            .ToHashSet();

        ushort port;
        // try like 5 random before falling back to sequential
        for (int i = 0; i < 5; i++)
        {
            port = (ushort)Random.Range(min, max);
            if (!usedPorts.Contains(port) && TryUsePort(port))
                return port;
        }

        for (port = min; port <= max; port++)
        {
            if (!usedPorts.Contains(port) && TryUsePort(port))
                return port;
        }

        return null;
    }

    // random elevated garbage will reserve whole ranges of free ports and you can only cry about it
    private static bool TryUsePort(ushort port)
    {
        try
        {
            IPEndPoint ep = new(IPAddress.Loopback, port);
            Socket s = new(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(ep);
            s.Close();
            return true;
        }
        catch
        {
            LOGGER.LogWarning($"someone reserved port {port} and i want you to know i'm mad about it >:(");
            return false;
        }
    }
}
