// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.SocketUtil
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

internal static class SocketUtil
{
  [DllImport("libc", EntryPoint = "getservbyname", CharSet = CharSet.Ansi, SetLastError = true)]
  private static extern IntPtr getservbyname_linux(string name, string proto);

  [DllImport("libc", EntryPoint = "getservbyport", CharSet = CharSet.Ansi, SetLastError = true)]
  private static extern IntPtr getservbyport_linux(ushort port, string proto);

  [DllImport("ws2_32.dll", SetLastError = true)]
  private static extern int WSAStartup(short wVersionRequested, out SocketUtil.WSAData wsaData);

  [DllImport("ws2_32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
  private static extern IntPtr getservbyname(string name, string proto);

  [DllImport("ws2_32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
  private static extern IntPtr getservbyport(ushort port, string proto);

  [DllImport("Ws2_32.dll", SetLastError = true)]
  private static extern int WSAGetLastError();

  [DllImport("ws2_32.dll", SetLastError = true)]
  private static extern int WSACleanup();

  private static T PtrToStructure<T>(IntPtr result)
  {
    return (T) Marshal.PtrToStructure(result, typeof (T));
  }

  public static string GetServiceByPortWindows(ushort port, string protocol)
  {
    int num = Socket.OSSupportsIPv6 ? 1 : 0;
    IntPtr result = SocketUtil.getservbyport((ushort) IPAddress.HostToNetworkOrder((short) port), protocol);
    if (IntPtr.Zero == result)
      throw new SocketUtilException($"Could not resolve service for port {port}");
    return Environment.Is64BitProcess ? SocketUtil.PtrToStructure<SocketUtil.servent64>(result).s_name : SocketUtil.PtrToStructure<SocketUtil.servent>(result).s_name;
  }

  public static string GetServiceByPortNonWindows(ushort port, string protocol)
  {
    IntPtr result = SocketUtil.getservbyport_linux((ushort) IPAddress.HostToNetworkOrder((short) port), protocol);
    return !(IntPtr.Zero == result) ? SocketUtil.PtrToStructure<SocketUtil.servent>(result).s_name : throw new SocketUtilException($"Could not resolve service for port {port}");
  }

  public static string GetServiceByPort(ushort port, string protocol)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? SocketUtil.GetServiceByPortNonWindows(port, protocol) : SocketUtil.GetServiceByPortWindows(port, protocol);
  }

  public static ushort GetServiceByNameWindows(string service, string protocol)
  {
    int num = Socket.OSSupportsIPv6 ? 1 : 0;
    IntPtr result = SocketUtil.getservbyname(service, protocol);
    if (IntPtr.Zero == result)
      throw new SocketUtilException($"Could not resolve port for service {service}");
    return (ushort) IPAddress.NetworkToHostOrder(!Environment.Is64BitProcess ? (short) SocketUtil.PtrToStructure<SocketUtil.servent>(result).s_port : (short) SocketUtil.PtrToStructure<SocketUtil.servent64>(result).s_port);
  }

  public static ushort GetServiceByNameNonWindows(string service, string protocol)
  {
    IntPtr result = SocketUtil.getservbyname_linux(service, protocol);
    return !(IntPtr.Zero == result) ? (ushort) IPAddress.NetworkToHostOrder((short) SocketUtil.PtrToStructure<SocketUtil.servent>(result).s_port) : throw new SocketUtilException($"Could not resolve port for service {service}");
  }

  public static ushort GetServiceByName(string service, string protocol)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? SocketUtil.GetServiceByNameNonWindows(service, protocol) : SocketUtil.GetServiceByNameWindows(service, protocol);
  }

  public struct servent
  {
    public string s_name;
    public IntPtr s_aliases;
    public ushort s_port;
    public string s_proto;
  }

  public struct servent64
  {
    public string s_name;
    public IntPtr s_aliases;
    public string s_proto;
    public ushort s_port;
  }

  internal struct WSAData
  {
    public short wVersion;
    public short wHighVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
    public string szDescription;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
    public string szSystemStatus;
    public short iMaxSockets;
    public short iMaxUdpDg;
    public int lpVendorInfo;
  }
}
