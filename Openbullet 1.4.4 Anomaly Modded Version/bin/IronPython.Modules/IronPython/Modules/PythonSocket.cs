// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSocket
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class PythonSocket
{
  private static readonly object _defaultTimeoutKey = new object();
  private static readonly object _defaultBufsizeKey = new object();
  private const int DefaultBufferSize = 8192 /*0x2000*/;
  public static PythonTuple _delegate_methods = PythonTuple.MakeTuple((object) "recv", (object) "recvfrom", (object) "recv_into", (object) "recvfrom_into", (object) "send", (object) "sendto");
  public const string __doc__ = "Implementation module for socket operations.\n\nThis module is a loose wrapper around the .NET System.Net.Sockets API, so you\nmay find the corresponding MSDN documentation helpful in decoding error\nmessages and understanding corner cases.\n\nThis implementation of socket differs slightly from the standard CPython\nsocket module. Many of these differences are due to the implementation of the\n.NET socket libraries. These differences are summarized below. For full\ndetails, check the docstrings of the functions mentioned.\n - s.accept(), s.connect(), and s.connect_ex() do not support timeouts.\n - Timeouts in s.sendall() don't work correctly.\n - s.dup() is not implemented.\n - SSL support is not implemented.\nAn Extra IronPython-specific function is exposed only if the clr module is\nimported:\n - s.HandleToSocket() returns the System.Net.Sockets.Socket object associated\n   with a particular \"file descriptor number\" (as returned by s.fileno()).\n";
  public static PythonType SocketType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonSocket.socket));
  private const string AnyAddrToken = "";
  private const string BroadcastAddrToken = "<broadcast>";
  private const string LocalhostAddrToken = "";
  private const int IPv4AddrBytes = 4;
  private const int IPv6AddrBytes = 16 /*0x10*/;
  private const double MillisecondsPerSecond = 1000.0;
  public static object _GLOBAL_DEFAULT_TIMEOUT = new object();
  public const int AF_APPLETALK = 16 /*0x10*/;
  public const int AF_DECnet = 12;
  public const int AF_INET = 2;
  public const int AF_INET6 = 23;
  public const int AF_IPX = 6;
  public const int AF_IRDA = 26;
  public const int AF_SNA = 11;
  public const int AF_UNSPEC = 0;
  public const int AI_CANONNAME = 2;
  public const int AI_NUMERICHOST = 4;
  public const int AI_PASSIVE = 1;
  public const int EAI_AGAIN = 11002;
  public const int EAI_BADFLAGS = 10022;
  public const int EAI_FAIL = 11003;
  public const int EAI_FAMILY = 10047;
  public const int EAI_MEMORY = 10055;
  public const int EAI_NODATA = 11001;
  public const int EAI_NONAME = 11001;
  public const int EAI_SERVICE = 10109;
  public const int EAI_SOCKTYPE = 10044;
  public const int EAI_SYSTEM = -1;
  public const int EBADF = 9;
  public const int INADDR_ALLHOSTS_GROUP = -536870911 /*0xE0000001*/;
  public const int INADDR_ANY = 0;
  public const int INADDR_BROADCAST = -1;
  public const int INADDR_LOOPBACK = 2130706433 /*0x7F000001*/;
  public const int INADDR_MAX_LOCAL_GROUP = -536870657 /*0xE00000FF*/;
  public const int INADDR_NONE = -1;
  public const int INADDR_UNSPEC_GROUP = -536870912 /*0xE0000000*/;
  public const int IPPORT_RESERVED = 1024 /*0x0400*/;
  public const int IPPORT_USERRESERVED = 5000;
  public const int IPPROTO_AH = 51;
  public const int IPPROTO_DSTOPTS = 60;
  public const int IPPROTO_ESP = 50;
  public const int IPPROTO_FRAGMENT = 44;
  public const int IPPROTO_GGP = 3;
  public const int IPPROTO_HOPOPTS = 0;
  public const int IPPROTO_ICMP = 1;
  public const int IPPROTO_ICMPV6 = 58;
  public const int IPPROTO_IDP = 22;
  public const int IPPROTO_IGMP = 2;
  public const int IPPROTO_IP = 0;
  public const int IPPROTO_IPV4 = 4;
  public const int IPPROTO_IPV6 = 41;
  public const int IPPROTO_MAX = 256 /*0x0100*/;
  public const int IPPROTO_ND = 77;
  public const int IPPROTO_NONE = 59;
  public const int IPPROTO_PUP = 12;
  public const int IPPROTO_RAW = 255 /*0xFF*/;
  public const int IPPROTO_ROUTING = 43;
  public const int IPPROTO_TCP = 6;
  public const int IPPROTO_UDP = 17;
  public const int IPV6_HOPLIMIT = 21;
  public const int IPV6_JOIN_GROUP = 12;
  public const int IPV6_LEAVE_GROUP = 13;
  public const int IPV6_MULTICAST_HOPS = 10;
  public const int IPV6_MULTICAST_IF = 9;
  public const int IPV6_MULTICAST_LOOP = 11;
  public const int IPV6_PKTINFO = 19;
  public const int IPV6_UNICAST_HOPS = 4;
  public const int IP_ADD_MEMBERSHIP = 12;
  public const int IP_DROP_MEMBERSHIP = 13;
  public const int IP_HDRINCL = 2;
  public const int IP_MULTICAST_IF = 9;
  public const int IP_MULTICAST_LOOP = 11;
  public const int IP_MULTICAST_TTL = 10;
  public const int IP_OPTIONS = 1;
  public const int IP_TOS = 3;
  public const int IP_TTL = 4;
  public const int MSG_DONTROUTE = 4;
  public const int MSG_OOB = 1;
  public const int MSG_PEEK = 2;
  public const int NI_DGRAM = 16 /*0x10*/;
  public const int NI_MAXHOST = 1025;
  public const int NI_MAXSERV = 32 /*0x20*/;
  public const int NI_NAMEREQD = 4;
  public const int NI_NOFQDN = 1;
  public const int NI_NUMERICHOST = 2;
  public const int NI_NUMERICSERV = 8;
  public const int SHUT_RD = 0;
  public const int SHUT_RDWR = 2;
  public const int SHUT_WR = 1;
  public const int SOCK_DGRAM = 2;
  public const int SOCK_RAW = 3;
  public const int SOCK_RDM = 4;
  public const int SOCK_SEQPACKET = 5;
  public const int SOCK_STREAM = 1;
  public const int SOL_IP = 0;
  public const int SOL_IPV6 = 41;
  public const int SOL_SOCKET = 65535 /*0xFFFF*/;
  public const int SOL_TCP = 6;
  public const int SOL_UDP = 17;
  public const int SOMAXCONN = 2147483647 /*0x7FFFFFFF*/;
  public const int SO_ACCEPTCONN = 2;
  public const int SO_BROADCAST = 32 /*0x20*/;
  public const int SO_DEBUG = 1;
  public const int SO_DONTROUTE = 16 /*0x10*/;
  public const int SO_ERROR = 4103;
  public const int SO_EXCLUSIVEADDRUSE = -5;
  public const int SO_KEEPALIVE = 8;
  public const int SO_LINGER = 128 /*0x80*/;
  public const int SO_OOBINLINE = 256 /*0x0100*/;
  public const int SO_RCVBUF = 4098;
  public const int SO_RCVLOWAT = 4100;
  public const int SO_RCVTIMEO = 4102;
  public const int SO_REUSEADDR = 4;
  public const int SO_SNDBUF = 4097;
  public const int SO_SNDLOWAT = 4099;
  public const int SO_SNDTIMEO = 4101;
  public const int SO_TYPE = 4104;
  public const int SO_USELOOPBACK = 64 /*0x40*/;
  public const int TCP_NODELAY = 1;
  public static readonly BigInteger SIO_RCVALL = (BigInteger) 2550136833L /*0x98000001*/;
  public static readonly BigInteger SIO_KEEPALIVE_VALS = (BigInteger) 2550136836L /*0x98000004*/;
  public const int RCVALL_ON = 1;
  public const int RCVALL_OFF = 0;
  public const int RCVALL_SOCKETLEVELONLY = 2;
  public const int RCVALL_MAX = 3;
  public const int has_ipv6 = 1;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (!context.HasModuleState(PythonSocket._defaultTimeoutKey))
      context.SetModuleState(PythonSocket._defaultTimeoutKey, (object) null);
    context.SetModuleState(PythonSocket._defaultBufsizeKey, (object) 8192 /*0x2000*/);
    PythonType socketError = PythonSocket.GetSocketError(context, dict);
    context.EnsureModuleException((object) "socketherror", socketError, dict, "herror", "socket");
    context.EnsureModuleException((object) "socketgaierror", socketError, dict, "gaierror", "socket");
    context.EnsureModuleException((object) "sockettimeout", socketError, dict, "timeout", "socket");
  }

  internal static PythonType GetSocketError(PythonContext context, PythonDictionary dict)
  {
    return context.EnsureModuleException((object) "socketerror", PythonExceptions.IOError, dict, "error", "socket");
  }

  [Documentation("Connect to *address* and return the socket object.\n\nConvenience function.  Connect to *address* (a 2-tuple ``(host,\nport)``) and return the socket object.  Passing the optional\n*timeout* parameter will set the timeout on the socket instance\nbefore attempting to connect.  If no *timeout* is supplied, the\nglobal default timeout setting returned by :func:`getdefaulttimeout`\nis used.\n")]
  public static PythonSocket.socket create_connection(CodeContext context, PythonTuple address)
  {
    return PythonSocket.create_connection(context, address, PythonSocket._GLOBAL_DEFAULT_TIMEOUT);
  }

  [Documentation("Connect to *address* and return the socket object.\n\nConvenience function.  Connect to *address* (a 2-tuple ``(host,\nport)``) and return the socket object.  Passing the optional\n*timeout* parameter will set the timeout on the socket instance\nbefore attempting to connect.  If no *timeout* is supplied, the\nglobal default timeout setting returned by :func:`getdefaulttimeout`\nis used.\n")]
  public static PythonSocket.socket create_connection(
    CodeContext context,
    PythonTuple address,
    object timeout)
  {
    return PythonSocket.create_connection(context, address, timeout, (PythonTuple) null);
  }

  public static PythonSocket.socket create_connection(
    CodeContext context,
    PythonTuple address,
    object timeout,
    PythonTuple source_address)
  {
    string host = Converter.ConvertToString(address[0]);
    object port = address[1];
    Exception exception = (Exception) null;
    IronPython.Runtime.List list = PythonSocket.getaddrinfo(context, host, port, socktype: 1);
    if (list.Count == 0)
      throw PythonExceptions.CreateThrowableForRaise(context, PythonSocket.error(context), (object) "getaddrinfo returns an empty list");
    foreach (PythonTuple pythonTuple in list)
    {
      int int32_1 = Converter.ConvertToInt32(pythonTuple[0]);
      int int32_2 = Converter.ConvertToInt32(pythonTuple[1]);
      int int32_3 = Converter.ConvertToInt32(pythonTuple[2]);
      PythonTuple address1 = (PythonTuple) pythonTuple[4];
      PythonSocket.socket connection = (PythonSocket.socket) null;
      try
      {
        connection = new PythonSocket.socket();
        connection.__init__(context, int32_1, int32_2, int32_3);
        if (timeout != PythonSocket._GLOBAL_DEFAULT_TIMEOUT)
          connection.settimeout(timeout);
        if (source_address != null)
          connection.bind(source_address);
        connection.connect(address1);
        return connection;
      }
      catch (Exception ex)
      {
        exception = ex;
        connection?.close();
      }
    }
    throw exception;
  }

  [Documentation("")]
  public static IronPython.Runtime.List getaddrinfo(
    CodeContext context,
    string host,
    object port,
    int family = 0,
    int socktype = 0,
    int proto = 0,
    int flags = 0)
  {
    switch (port)
    {
      case null:
        result = 0;
        goto label_10;
      case int result:
label_10:
        if (socktype != 0)
        {
          System.Net.Sockets.SocketType socketType = (System.Net.Sockets.SocketType) System.Enum.ToObject(typeof (System.Net.Sockets.SocketType), socktype);
          if (socketType == System.Net.Sockets.SocketType.Unknown || !System.Enum.IsDefined(typeof (System.Net.Sockets.SocketType), (object) socketType))
            throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) PythonTuple.MakeTuple((object) 10044, (object) "getaddrinfo failed"));
        }
        AddressFamily family1 = (AddressFamily) System.Enum.ToObject(typeof (AddressFamily), family);
        if (!System.Enum.IsDefined(typeof (AddressFamily), (object) family1))
          throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) PythonTuple.MakeTuple((object) 10047, (object) "getaddrinfo failed"));
        System.Enum.ToObject(typeof (ProtocolType), proto);
        if (host == null)
          host = "localhost";
        IPAddress[] addresses = PythonSocket.HostToAddresses(context, host, family1);
        IronPython.Runtime.List list = new IronPython.Runtime.List();
        foreach (IPAddress address in addresses)
          list.append((object) PythonTuple.MakeTuple((object) (int) address.AddressFamily, (object) socktype, (object) proto, (object) "", (object) PythonSocket.EndPointToTuple(new IPEndPoint(address, result))));
        return list;
      case Extensible<int> _:
        result = ((Extensible<int>) port).Value;
        goto label_10;
      case string _:
        if (!int.TryParse((string) port, out result))
        {
          try
          {
            port = (object) PythonSocket.getservbyname(context, (string) port);
            goto label_10;
          }
          catch
          {
            throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) "getaddrinfo failed");
          }
        }
        else
          goto label_10;
      case ExtensibleString _:
        if (!int.TryParse(((Extensible<string>) port).Value, out result))
        {
          try
          {
            port = (object) PythonSocket.getservbyname(context, (string) port);
            goto label_10;
          }
          catch
          {
            throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) "getaddrinfo failed");
          }
        }
        else
          goto label_10;
      default:
        throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) "getaddrinfo failed");
    }
  }

  private static PythonType gaierror(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "socketgaierror");
  }

  [Documentation("getfqdn([hostname_or_ip]) -> hostname\n\nReturn the fully-qualified domain name for the specified hostname or IP\naddress. An unspecified or empty name is interpreted as the local host. If the\nname lookup fails, the passed-in name is returned as-is.")]
  public static string getfqdn(string host)
  {
    host = host != null ? host.Trim() : throw PythonOps.TypeError("expected string, got None");
    if (host == string.Empty || host == "0.0.0.0")
      host = PythonSocket.gethostname();
    if (host == "<broadcast>")
      return host;
    try
    {
      IPHostEntry hostEntry = Dns.GetHostEntry(host);
      if (hostEntry.HostName.Contains("."))
        return hostEntry.HostName;
      foreach (string alias in hostEntry.Aliases)
      {
        if (alias.Contains("."))
          return alias;
      }
    }
    catch (SocketException ex)
    {
    }
    return host;
  }

  [Documentation("")]
  public static string getfqdn() => PythonSocket.getfqdn("");

  [Documentation("gethostbyname(hostname) -> ip address\n\nReturn the string IPv4 address associated with the given hostname (e.g.\n'10.10.0.1'). The hostname is returned as-is if it an IPv4 address. The empty\nstring is treated as the local host.\n\ngethostbyname() doesn't support IPv6; for IPv4/IPv6 support, use getaddrinfo().")]
  public static string gethostbyname(CodeContext context, string host)
  {
    return PythonSocket.HostToAddress(context, host, AddressFamily.InterNetwork).ToString();
  }

  [Documentation("gethostbyname_ex(hostname) -> (hostname, aliases, ip_addresses)\n\nReturn the real host name, a list of aliases, and a list of IP addresses\nassociated with the given hostname. If the hostname is an IPv4 address, the\ntuple ([hostname, [], [hostname]) is returned without doing a DNS lookup.\n\ngethostbyname_ex() doesn't support IPv6; for IPv4/IPv6 support, use\ngetaddrinfo().")]
  public static PythonTuple gethostbyname_ex(CodeContext context, string host)
  {
    IronPython.Runtime.List list1 = PythonOps.MakeList();
    IPAddress address1;
    string str;
    IronPython.Runtime.List list2;
    if (IPAddress.TryParse(host, out address1))
    {
      if (AddressFamily.InterNetwork == address1.AddressFamily)
      {
        str = host;
        list2 = PythonOps.MakeEmptyList(0);
        list1.append((object) host);
      }
      else
        throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) 11001, (object) "no IPv4 addresses associated with host");
    }
    else
    {
      IPHostEntry hostEntry;
      try
      {
        hostEntry = Dns.GetHostEntry(host);
      }
      catch (SocketException ex)
      {
        throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) (int) ex.SocketErrorCode, (object) "no IPv4 addresses associated with host");
      }
      str = hostEntry.HostName;
      list2 = PythonOps.MakeList((object[]) hostEntry.Aliases);
      foreach (IPAddress address2 in hostEntry.AddressList)
      {
        if (AddressFamily.InterNetwork == address2.AddressFamily)
          list1.append((object) address2.ToString());
      }
    }
    return PythonTuple.MakeTuple((object) str, (object) list2, (object) list1);
  }

  [Documentation("gethostname() -> hostname\nReturn this machine's hostname")]
  public static string gethostname() => Dns.GetHostName();

  [Documentation("gethostbyaddr(host) -> (hostname, aliases, ipaddrs)\n\nReturn a tuple of (primary hostname, alias hostnames, ip addresses). host may\nbe either a hostname or an IP address.")]
  public static object gethostbyaddr(CodeContext context, string host)
  {
    if (host == "")
      host = PythonSocket.gethostname();
    host = PythonSocket.gethostbyname(context, host);
    IPAddress[] hostAddresses;
    IPHostEntry hostEntry;
    try
    {
      hostAddresses = Dns.GetHostAddresses(host);
      hostEntry = Dns.GetHostEntry(host);
    }
    catch (Exception ex)
    {
      throw PythonSocket.MakeException(context, ex);
    }
    IronPython.Runtime.List list = PythonOps.MakeList();
    foreach (IPAddress ipAddress in hostAddresses)
      list.append((object) ipAddress.ToString());
    return (object) PythonTuple.MakeTuple((object) hostEntry.HostName, (object) PythonOps.MakeList((object[]) hostEntry.Aliases), (object) list);
  }

  [Documentation("getnameinfo(socketaddr, flags) -> (host, port)\nGiven a socket address, the return a tuple of the corresponding hostname and\nport. Available flags:\n - NI_NOFQDN: Return only the hostname part of the domain name for hosts on the\n   same domain as the executing machine.\n - NI_NUMERICHOST: return the numeric form of the host (e.g. '127.0.0.1' or\n   '::1' rather than 'localhost').\n - NI_NAMEREQD: Raise an error if the hostname cannot be looked up.\n - NI_NUMERICSERV: Return string containing the numeric form of the port (e.g.\n   '80' rather than 'http'). This flag is required (see below).\n - NI_DGRAM: Silently ignored (see below).\n\n")]
  public static object getnameinfo(CodeContext context, PythonTuple socketAddr, int flags)
  {
    string host = socketAddr.__len__() >= 2 && socketAddr.__len__() <= 4 ? Converter.ConvertToString(socketAddr[0]) : throw PythonOps.TypeError("socket address must be a 2-tuple (IPv4 or IPv6) or 4-tuple (IPv6)");
    if (host == null)
      throw PythonOps.TypeError("argument 1 must be string");
    int port;
    try
    {
      port = (int) socketAddr[1];
    }
    catch (InvalidCastException ex)
    {
      throw PythonOps.TypeError("an integer is required");
    }
    IList<IPAddress> ipAddressList1;
    try
    {
      ipAddressList1 = (IList<IPAddress>) PythonSocket.HostToAddresses(context, host, AddressFamily.InterNetwork);
      if (ipAddressList1.Count < 1)
        throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "sockaddr resolved to zero addresses");
    }
    catch (SocketException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) (int) ex.SocketErrorCode, (object) ex.Message);
    }
    catch (IndexOutOfRangeException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) "sockaddr resolved to zero addresses");
    }
    if (ipAddressList1.Count > 1)
    {
      List<IPAddress> ipAddressList2 = new List<IPAddress>(ipAddressList1.Count);
      foreach (IPAddress ipAddress in (IEnumerable<IPAddress>) ipAddressList1)
      {
        if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
          ipAddressList2.Add(ipAddress);
      }
      ipAddressList1 = ipAddressList2.Count <= 1 ? (IList<IPAddress>) ipAddressList2 : throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "sockaddr resolved to multiple addresses");
    }
    if (ipAddressList1.Count < 1)
      throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "sockaddr resolved to zero addresses");
    IPHostEntry hostEntry;
    try
    {
      hostEntry = Dns.GetHostEntry(ipAddressList1[0]);
    }
    catch (SocketException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) (int) ex.SocketErrorCode, (object) ex.Message);
    }
    string str1 = (flags & 2) == 0 ? ((flags & 1) == 0 ? hostEntry.HostName : PythonSocket.RemoveLocalDomain(hostEntry.HostName)) : ipAddressList1[0].ToString();
    string str2;
    if ((flags & 8) == 0)
    {
      try
      {
        str2 = PythonSocket.getservbyport(context, port);
      }
      catch
      {
        str2 = port.ToString();
      }
      flags |= 8;
    }
    else
      str2 = port.ToString();
    return (object) PythonTuple.MakeTuple((object) str1, (object) str2);
  }

  [Documentation("getprotobyname(protoname) -> integer proto\n\nGiven a string protocol name (e.g. \"udp\"), return the associated integer\nprotocol number, suitable for passing to socket(). The name is case\ninsensitive.\n\nRaises socket.error if no protocol number can be found.")]
  public static object getprotobyname(CodeContext context, string protocolName)
  {
    switch (protocolName.ToLower())
    {
      case "ah":
        return (object) 51;
      case "dstopts":
        return (object) 60;
      case "esp":
        return (object) 50;
      case "fragment":
        return (object) 44;
      case "ggp":
        return (object) 3;
      case "icmp":
        return (object) 1;
      case "icmpv6":
        return (object) 58;
      case "ip":
        return (object) 0;
      case "ipv4":
        return (object) 4;
      case "ipv6":
        return (object) 41;
      case "nd":
        return (object) 77;
      case "none":
        return (object) 59;
      case "pup":
        return (object) 12;
      case "raw":
        return (object) (int) byte.MaxValue;
      case "routing":
        return (object) 43;
      case "tcp":
        return (object) 6;
      case "udp":
        return (object) 17;
      default:
        throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "protocol not found");
    }
  }

  [Documentation("getservbyname(service_name[, protocol_name]) -> port\n\nReturn a port number from a service name and protocol name.\nThe optional protocol name, if given, should be 'tcp' or 'udp',\notherwise any protocol will match.")]
  public static int getservbyname(CodeContext context, string serviceName, string protocolName = null)
  {
    if (protocolName != null)
    {
      protocolName = protocolName.ToLower();
      if (protocolName != "udp" && protocolName != "tcp")
        throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "service/proto not found");
    }
    try
    {
      return (int) SocketUtil.GetServiceByName(serviceName, protocolName);
    }
    catch
    {
    }
    switch (serviceName.ToLower())
    {
      case "bootpc":
        if (protocolName == null || protocolName == "udp")
          return 68;
        break;
      case "bootps":
        if (protocolName == null || protocolName == "udp")
          return 67;
        break;
      case "daytime":
        return 13;
      case "dhcpv6-client":
        return 546;
      case "dhcpv6-server":
        return 547;
      case "domain":
        return 53;
      case "echo":
        return 7;
      case "ftp":
        if (protocolName == null || protocolName == "tcp")
          return 21;
        break;
      case "ftp-data":
        return 20;
      case "http":
        if (protocolName == null || protocolName == "tcp")
          return 80 /*0x50*/;
        break;
      case "https":
        if (protocolName == null || protocolName == "tcp")
          return 430;
        break;
      case "imap":
        if (protocolName == null || protocolName == "tcp")
          return 143;
        break;
      case "kerberos":
        return 88;
      case "ldap":
        if (protocolName == null || protocolName == "tcp")
          return 389;
        break;
      case "nameserver":
        return 42;
      case "nicname":
        if (protocolName == null || protocolName == "tcp")
          return 43;
        break;
      case "nntp":
        if (protocolName == null || protocolName == "tcp")
          return 119;
        break;
      case "ntp":
        if (protocolName == null || protocolName == "udp")
          return 123;
        break;
      case "pop2":
        if (protocolName == null || protocolName == "tcp")
          return 109;
        break;
      case "pop3":
        if (protocolName == null || protocolName == "tcp")
          return 110;
        break;
      case "rlp":
        return 39;
      case "rtelnet":
        if (protocolName == null || protocolName == "tcp")
          return 107;
        break;
      case "rtsp":
        return 554;
      case "smtp":
        if (protocolName == null || protocolName == "tcp")
          return 25;
        break;
      case "snmp":
        if (protocolName == null || protocolName == "udp")
          return 161;
        break;
      case "snmptrap":
        if (protocolName == null || protocolName == "udp")
          return 162;
        break;
      case "ssh":
        return 22;
      case "telnet":
        return 23;
      case "tftp":
        if (protocolName == null || protocolName == "udp")
          return 69;
        break;
      case "time":
        return 37;
    }
    throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "service/proto not found");
  }

  [Documentation("getservbyport(port[, protocol_name]) -> service_name\n\nReturn a service name from a port number and protocol name.\nThe optional protocol name, if given, should be 'tcp' or 'udp',\notherwise any protocol will match.")]
  public static string getservbyport(CodeContext context, int port, string protocolName = null)
  {
    if (port < 0 || port > (int) ushort.MaxValue)
      throw PythonOps.OverflowError("getservbyport: port must be 0-65535.");
    if (protocolName != null)
    {
      protocolName = protocolName.ToLower();
      if (protocolName != "udp" && protocolName != "tcp")
        throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "port/proto not found");
    }
    try
    {
      return SocketUtil.GetServiceByPort((ushort) port, protocolName);
    }
    catch
    {
    }
    switch (port)
    {
      case 7:
        return "echo";
      case 13:
        return "daytime";
      case 20:
        return "ftp-data";
      case 21:
        if (protocolName == null || protocolName == "tcp")
          return "ftp";
        break;
      case 22:
        return "ssh";
      case 23:
        return "telnet";
      case 25:
        if (protocolName == null || protocolName == "tcp")
          return "smtp";
        break;
      case 37:
        return "time";
      case 39:
        return "rlp";
      case 42:
        return "nameserver";
      case 43:
        if (protocolName == null || protocolName == "tcp")
          return "nicname";
        break;
      case 53:
        return "domain";
      case 67:
        if (protocolName == null || protocolName == "udp")
          return "bootps";
        break;
      case 68:
        if (protocolName == null || protocolName == "udp")
          return "bootpc";
        break;
      case 69:
        if (protocolName == null || protocolName == "udp")
          return "tftp";
        break;
      case 80 /*0x50*/:
        if (protocolName == null || protocolName == "tcp")
          return "http";
        break;
      case 88:
        return "kerberos";
      case 107:
        if (protocolName == null || protocolName == "tcp")
          return "rtelnet";
        break;
      case 109:
        if (protocolName == null || protocolName == "tcp")
          return "pop2";
        break;
      case 110:
        if (protocolName == null || protocolName == "tcp")
          return "pop3";
        break;
      case 119:
        if (protocolName == null || protocolName == "tcp")
          return "nntp";
        break;
      case 123:
        if (protocolName == null || protocolName == "udp")
          return "ntp";
        break;
      case 143:
        if (protocolName == null || protocolName == "tcp")
          return "imap";
        break;
      case 161:
        if (protocolName == null || protocolName == "udp")
          return "snmp";
        break;
      case 162:
        if (protocolName == null || protocolName == "udp")
          return "snmptrap";
        break;
      case 389:
        if (protocolName == null || protocolName == "tcp")
          return "ldap";
        break;
      case 430:
        if (protocolName == null || protocolName == "tcp")
          return "https";
        break;
      case 546:
        return "dhcpv6-client";
      case 547:
        return "dhcpv6-server";
      case 554:
        return "rtsp";
    }
    throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "port/proto not found");
  }

  [Documentation("ntohl(x) -> integer\n\nConvert a 32-bit integer from network byte order to host byte order.")]
  public static object ntohl(object x)
  {
    int hostOrder = IPAddress.NetworkToHostOrder(PythonSocket.SignInsensitiveToInt32(x));
    return hostOrder < 0 ? (object) (BigInteger) (uint) hostOrder : (object) hostOrder;
  }

  [Documentation("ntohs(x) -> integer\n\nConvert a 16-bit integer from network byte order to host byte order.")]
  public static int ntohs(object x)
  {
    return (int) (ushort) IPAddress.NetworkToHostOrder(PythonSocket.SignInsensitiveToInt16(x));
  }

  [Documentation("htonl(x) -> integer\n\nConvert a 32bit integer from host byte order to network byte order.")]
  public static object htonl(object x)
  {
    int networkOrder = IPAddress.HostToNetworkOrder(PythonSocket.SignInsensitiveToInt32(x));
    return networkOrder < 0 ? (object) (BigInteger) (uint) networkOrder : (object) networkOrder;
  }

  [Documentation("htons(x) -> integer\n\nConvert a 16-bit integer from host byte order to network byte order.")]
  public static int htons(object x)
  {
    return (int) (ushort) IPAddress.HostToNetworkOrder(PythonSocket.SignInsensitiveToInt16(x));
  }

  private static int SignInsensitiveToInt32(object x)
  {
    BigInteger bigInteger = Converter.ConvertToBigInteger(x);
    if (bigInteger < 0L)
      throw PythonOps.OverflowError("can't convert negative number to unsigned long");
    return bigInteger <= (long) int.MaxValue ? (int) bigInteger : (int) (uint) bigInteger;
  }

  private static short SignInsensitiveToInt16(object x)
  {
    BigInteger bigInteger = Converter.ConvertToBigInteger(x);
    if (bigInteger < 0L)
      throw PythonOps.OverflowError("can't convert negative number to unsigned long");
    return bigInteger <= (long) short.MaxValue ? (short) bigInteger : (short) (ushort) bigInteger;
  }

  [Documentation("inet_pton(addr_family, ip_string) -> packed_ip\n\nConvert an IP address (in string format, e.g. '127.0.0.1' or '::1') to a 32-bit\npacked binary format, as 4-byte (IPv4) or 16-byte (IPv6) string. The return\nformat matches the format of the standard C library's in_addr or in6_addr\nstruct.\n\nIf the address format is invalid, socket.error will be raised. Validity is\ndetermined by the .NET System.Net.IPAddress.Parse() method.\n\ninet_pton() supports IPv4 and IPv6.")]
  public static string inet_pton(CodeContext context, int addressFamily, string ipString)
  {
    if (addressFamily != 2 && addressFamily != 23)
      throw PythonSocket.MakeException(context, (Exception) new SocketException(10047));
    IPAddress ipAddress;
    try
    {
      ipAddress = IPAddress.Parse(ipString);
      if ((AddressFamily) addressFamily != ipAddress.AddressFamily)
        throw PythonSocket.MakeException(context, (Exception) new SocketException(10047));
    }
    catch (FormatException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "illegal IP address passed to inet_pton");
    }
    return ((IList<byte>) ipAddress.GetAddressBytes()).MakeString();
  }

  [Documentation("inet_ntop(address_family, packed_ip) -> ip_string\n\nConvert a packed IP address (a 4-byte [IPv4] or 16-byte [IPv6] string) to a\nstring IP address (e.g. '127.0.0.1' or '::1').\n\nThe input format matches the format of the standard C library's in_addr or\nin6_addr struct. If the input string is not exactly 4 bytes or 16 bytes,\nsocket.error will be raised.\n\ninet_ntop() supports IPv4 and IPv6.")]
  public static string inet_ntop(CodeContext context, int addressFamily, string packedIP)
  {
    if ((packedIP.Length != 4 || addressFamily != 2) && (packedIP.Length != 16 /*0x10*/ || addressFamily != 23))
      throw PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) "invalid length of packed IP address string");
    byte[] numArray = packedIP.MakeByteArray();
    return addressFamily == 23 ? PythonSocket.IPv6BytesToColonHex(numArray) : new IPAddress(numArray).ToString();
  }

  [Documentation("inet_aton(ip_string) -> packed_ip\nConvert an IP address (in string dotted quad format, e.g. '127.0.0.1') to a\n32-bit packed binary format, as four-character string. The return format\nmatches the format of the standard C library's in_addr struct.\n\nIf the address format is invalid, socket.error will be raised. Validity is\ndetermined by the .NET System.Net.IPAddress.Parse() method.\n\ninet_aton() supports only IPv4.")]
  public static string inet_aton(CodeContext context, string ipString)
  {
    return PythonSocket.inet_pton(context, 2, ipString);
  }

  [Documentation("inet_ntoa(packed_ip) -> ip_string\n\nConvert a packed IP address (a 4-byte string) to a string IP address (in dotted\nquad format, e.g. '127.0.0.1'). The input format matches the format of the\nstandard C library's in_addr struct.\n\nIf the input string is not exactly 4 bytes, socket.error will be raised.\n\ninet_ntoa() supports only IPv4.")]
  public static string inet_ntoa(CodeContext context, string packedIP)
  {
    return PythonSocket.inet_ntop(context, 2, packedIP);
  }

  [Documentation("getdefaulttimeout() -> timeout\n\nReturn the default timeout for new socket objects in seconds as a float. A\nvalue of None means that sockets have no timeout and begin in blocking mode.\nThe default value when the module is imported is None.")]
  public static object getdefaulttimeout(CodeContext context)
  {
    int? defaultTimeout = PythonSocket.GetDefaultTimeout(context);
    return !defaultTimeout.HasValue ? (object) null : (object) ((double) defaultTimeout.Value / 1000.0);
  }

  [Documentation("setdefaulttimeout(timeout) -> None\n\nSet the default timeout for new socket objects. timeout must be either None,\nmeaning that sockets have no timeout and start in blocking mode, or a\nnon-negative float that specifies the default timeout in seconds.")]
  public static void setdefaulttimeout(CodeContext context, object timeout)
  {
    if (timeout == null)
    {
      PythonSocket.SetDefaultTimeout(context, new int?());
    }
    else
    {
      double num = Converter.ConvertToDouble(timeout);
      if (num < 0.0)
        throw PythonOps.ValueError("a non-negative float is required");
      PythonSocket.SetDefaultTimeout(context, new int?((int) (num * 1000.0)));
    }
  }

  internal static Exception MakeException(CodeContext context, Exception exception)
  {
    switch (exception)
    {
      case SocketException _:
        SocketException socketException = (SocketException) exception;
        switch (socketException.SocketErrorCode)
        {
          case SocketError.NotConnected:
          case SocketError.TimedOut:
            return PythonExceptions.CreateThrowable(PythonSocket.timeout(context), (object) (int) socketException.SocketErrorCode, (object) socketException.Message);
          default:
            return PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) (int) socketException.SocketErrorCode, (object) socketException.Message);
        }
      case ObjectDisposedException _:
        return PythonExceptions.CreateThrowable(PythonSocket.error(context), (object) 9, (object) "the socket is closed");
      case InvalidOperationException _:
        return PythonSocket.MakeException(context, (Exception) new SocketException(10022));
      default:
        return exception;
    }
  }

  private static string IPv6BytesToColonHex(byte[] ipBytes)
  {
    int[] numArray = new int[8];
    for (int index1 = 0; index1 < numArray.Length; ++index1)
    {
      for (int index2 = 0; index2 < 2; ++index2)
      {
        numArray[index1] <<= 8;
        numArray[index1] += (int) ipBytes[index1 * 2 + index2];
      }
    }
    int num1 = 0;
    int num2 = 0;
    for (int index3 = 0; index3 < numArray.Length; ++index3)
    {
      if (numArray[index3] == 0)
      {
        for (int index4 = index3; index4 < numArray.Length; ++index4)
        {
          if (numArray[index4] != 0)
          {
            index3 += num2;
            break;
          }
          if (index4 - index3 + 1 > num2)
          {
            num1 = index3;
            num2 = index4 - index3 + 1;
          }
        }
      }
    }
    StringBuilder stringBuilder = new StringBuilder(48 /*0x30*/);
    for (int index = 0; index < numArray.Length; ++index)
    {
      if (index != 0)
        stringBuilder.Append(':');
      if (num2 > 0 && index == num1)
      {
        if (num1 == 0)
          stringBuilder.Append(':');
        if (num1 + num2 == numArray.Length)
          stringBuilder.Append(':');
        index += num2 - 1;
      }
      else
        stringBuilder.Append(numArray[index].ToString("x"));
    }
    return stringBuilder.ToString();
  }

  private static string ConvertSpecialAddresses(string host)
  {
    switch (host)
    {
      case "":
        return IPAddress.Any.ToString();
      case "<broadcast>":
        return IPAddress.Broadcast.ToString();
      default:
        return host;
    }
  }

  private static IPAddress HostToAddress(CodeContext context, string host, AddressFamily family)
  {
    return PythonSocket.HostToAddresses(context, host, family)[0];
  }

  private static IPAddress[] HostToAddresses(
    CodeContext context,
    string host,
    AddressFamily family)
  {
    host = PythonSocket.ConvertSpecialAddresses(host);
    try
    {
      bool flag = true;
      int num = 0;
      foreach (char c in host)
      {
        if (!char.IsNumber(c) && c != '.')
          flag = false;
        else if (c == '.')
          ++num;
      }
      if (flag)
      {
        IPAddress address;
        if (num == 3 && IPAddress.TryParse(host, out address) && (family == AddressFamily.Unspecified || family == address.AddressFamily))
          return new IPAddress[1]{ address };
      }
      else
      {
        IPHostEntry hostEntry = Dns.GetHostEntry(host);
        List<IPAddress> ipAddressList = new List<IPAddress>();
        foreach (IPAddress address in hostEntry.AddressList)
        {
          if (family == AddressFamily.Unspecified || family == address.AddressFamily)
            ipAddressList.Add(address);
        }
        if (ipAddressList.Count > 0)
          return ipAddressList.ToArray();
      }
      throw new SocketException(11001);
    }
    catch (SocketException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSocket.gaierror(context), (object) (int) ex.SocketErrorCode, (object) "no addresses of the specified family associated with host");
    }
  }

  private static string RemoveLocalDomain(string fqdn)
  {
    char[] separator = new char[1]{ '.' };
    string[] strArray1 = PythonSocket.getfqdn().Split(separator, 2);
    string[] strArray2 = fqdn.Split(separator, 2);
    return strArray1.Length < 2 || strArray2.Length < 2 || !(strArray1[1] == strArray2[1]) ? fqdn : strArray2[0];
  }

  private static IPEndPoint TupleToEndPoint(
    CodeContext context,
    PythonTuple address,
    AddressFamily family,
    out string host)
  {
    if (address.__len__() != 2 && address.__len__() != 4)
      throw PythonOps.TypeError("address tuple must have exactly 2 (IPv4) or exactly 4 (IPv6) elements");
    try
    {
      host = Converter.ConvertToString(address[0]);
    }
    catch (ArgumentTypeException ex)
    {
      throw PythonOps.TypeError("host must be string");
    }
    int int32;
    try
    {
      int32 = context.LanguageContext.ConvertToInt32(address[1]);
    }
    catch (ArgumentTypeException ex)
    {
      throw PythonOps.TypeError("port must be integer");
    }
    if (int32 < 0 || int32 > (int) ushort.MaxValue)
      throw PythonOps.OverflowError("getsockaddrarg: port must be 0-65535");
    IPAddress address1 = PythonSocket.HostToAddress(context, host, family);
    if (address.__len__() == 2)
      return new IPEndPoint(address1, int32);
    try
    {
      Converter.ConvertToInt64(address[2]);
    }
    catch (ArgumentTypeException ex)
    {
      throw PythonOps.TypeError("flowinfo must be integer");
    }
    long int64;
    try
    {
      int64 = Converter.ConvertToInt64(address[3]);
    }
    catch (ArgumentTypeException ex)
    {
      throw PythonOps.TypeError("scopeid must be integer");
    }
    return new IPEndPoint(address1, int32)
    {
      Address = {
        ScopeId = int64
      }
    };
  }

  private static PythonTuple EndPointToTuple(IPEndPoint endPoint)
  {
    string str = endPoint.Address.ToString();
    int port = endPoint.Port;
    switch (endPoint.Address.AddressFamily)
    {
      case AddressFamily.InterNetwork:
        return PythonTuple.MakeTuple((object) str, (object) port);
      case AddressFamily.InterNetworkV6:
        long num = 0;
        long scopeId = endPoint.Address.ScopeId;
        return PythonTuple.MakeTuple((object) str, (object) port, (object) num, (object) scopeId);
      default:
        throw new SocketException(10047);
    }
  }

  private static int? GetDefaultTimeout(CodeContext context)
  {
    return (int?) context.LanguageContext.GetModuleState(PythonSocket._defaultTimeoutKey);
  }

  private static void SetDefaultTimeout(CodeContext context, int? timeout)
  {
    context.LanguageContext.SetModuleState(PythonSocket._defaultTimeoutKey, (object) timeout);
  }

  private static PythonType error(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "socketerror");
  }

  private static PythonType herror(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "socketherror");
  }

  private static PythonType timeout(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "sockettimeout");
  }

  [PythonType]
  [Documentation("socket([family[, type[, proto]]]) -> socket object\n\nCreate a socket (a network connection endpoint) of the given family, type,\nand protocol. socket() accepts keyword arguments.\n - family (address family) defaults to AF_INET\n - type (socket type) defaults to SOCK_STREAM\n - proto (protocol type) defaults to 0, which specifies the default protocol\n\nThis module supports only IP sockets. It does not support raw or Unix sockets.\nBoth IPv4 and IPv6 are supported.")]
  public class socket : IWeakReferenceable
  {
    private static readonly Dictionary<IntPtr, WeakReference> _handleToSocket = new Dictionary<IntPtr, WeakReference>();
    private const int DefaultAddressFamily = 2;
    private const int DefaultSocketType = 1;
    private const int DefaultProtocolType = 0;
    internal Socket _socket;
    internal string _hostName;
    private WeakRefTracker _weakRefTracker;
    private int _referenceCount = 1;
    public const string __module__ = "socket";
    internal CodeContext _context;
    private int _timeout;
    private IAsyncResult _acceptResult;

    public socket()
    {
    }

    public void __init__(
      CodeContext context,
      int addressFamily = 2,
      int socketType = 1,
      int protocolType = 0,
      PythonSocket.socket _sock = null)
    {
      System.Net.Sockets.SocketType socketType1 = (System.Net.Sockets.SocketType) System.Enum.ToObject(typeof (System.Net.Sockets.SocketType), socketType);
      if (!System.Enum.IsDefined(typeof (System.Net.Sockets.SocketType), (object) socketType1))
        throw PythonSocket.MakeException(context, (Exception) new SocketException(10044));
      AddressFamily addressFamily1 = (AddressFamily) System.Enum.ToObject(typeof (AddressFamily), addressFamily);
      if (!System.Enum.IsDefined(typeof (AddressFamily), (object) addressFamily1))
        throw PythonSocket.MakeException(context, (Exception) new SocketException(10047));
      ProtocolType protocolType1 = (ProtocolType) System.Enum.ToObject(typeof (ProtocolType), protocolType);
      if (!System.Enum.IsDefined(typeof (ProtocolType), (object) protocolType1))
        throw PythonSocket.MakeException(context, (Exception) new SocketException(10043));
      if (_sock == null)
      {
        Socket socket;
        try
        {
          socket = new Socket(addressFamily1, socketType1, protocolType1);
        }
        catch (SocketException ex)
        {
          throw PythonSocket.MakeException(context, (Exception) ex);
        }
        this.Initialize(context, socket);
      }
      else
      {
        this._socket = _sock._socket;
        this._hostName = _sock._hostName;
        GC.SuppressFinalize((object) _sock);
        this.Initialize(context, this._socket);
      }
    }

    public void __del__() => this._close();

    ~socket() => this._close();

    public PythonSocket.socket _sock => this;

    [Documentation("accept() -> (conn, address)\n\nAccept a connection. The socket must be bound and listening before calling\naccept(). conn is a new socket object connected to the remote host, and\naddress is the remote host's address (e.g. a (host, port) tuple for IPv4).\n\n")]
    public PythonTuple accept()
    {
      Socket socket1;
      try
      {
        if (this._acceptResult != null && this._acceptResult.IsCompleted)
        {
          socket1 = this._socket.EndAccept(this._acceptResult);
        }
        else
        {
          int timeout = this._timeout;
          if (timeout != 0)
          {
            IAsyncResult asyncResult = this._acceptResult ?? this._socket.BeginAccept((AsyncCallback) (x => { }), (object) null);
            if (asyncResult.AsyncWaitHandle.WaitOne(timeout))
            {
              socket1 = this._socket.EndAccept(asyncResult);
              this._acceptResult = (IAsyncResult) null;
            }
            else
            {
              this._acceptResult = asyncResult;
              throw PythonExceptions.CreateThrowable(PythonSocket.timeout(this._context), (object) 0, (object) "timeout");
            }
          }
          else
            socket1 = this._socket.Accept();
        }
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
      PythonSocket.socket socket2 = new PythonSocket.socket(this._context, socket1);
      return PythonTuple.MakeTuple((object) socket2, (object) socket2.getpeername());
    }

    [Documentation("bind(address) -> None\n\nBind to an address. If the socket is already bound, socket.error is raised.\nFor IP sockets, address is a (host, port) tuple. Raw sockets are not\nsupported.\n\nIf you do not care which local address is assigned, set host to INADDR_ANY and\nthe system will assign the most appropriate network address. Similarly, if you\nset port to 0, the system will assign an available port number between 1024\nand 5000.")]
    public void bind(PythonTuple address)
    {
      IPEndPoint endPoint = PythonSocket.TupleToEndPoint(this._context, address, this._socket.AddressFamily, out this._hostName);
      try
      {
        this._socket.Bind((EndPoint) endPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("close() -> None\n\nClose the socket. It cannot be used after being closed.")]
    public void close()
    {
      if (Interlocked.Decrement(ref this._referenceCount) >= 1)
        return;
      this._close();
    }

    internal void _close()
    {
      if (this._socket == null)
        return;
      lock (PythonSocket.socket._handleToSocket)
      {
        WeakReference weakReference;
        if (PythonSocket.socket._handleToSocket.TryGetValue(this._socket.Handle, out weakReference))
        {
          Socket target = weakReference.Target as Socket;
          if (target != this._socket)
          {
            if (target != null)
              goto label_10;
          }
          PythonSocket.socket._handleToSocket.Remove(this._socket.Handle);
        }
      }
label_10:
      this._socket.Dispose();
      this._referenceCount = 0;
    }

    [Documentation("connect(address) -> None\n\nConnect to a remote socket at the given address. IP addresses are expressed\nas (host, port).\n\nRaises socket.error if the socket has been closed, the socket is listening, or\nanother connection error occurred.\nDifference from CPython: connect() does not support timeouts in blocking mode.\nIf a timeout is set and the socket is in blocking mode, connect() will block\nindefinitely until a connection is made or an error occurs.")]
    public void connect(PythonTuple address)
    {
      IPEndPoint endPoint = PythonSocket.TupleToEndPoint(this._context, address, this._socket.AddressFamily, out this._hostName);
      try
      {
        this._socket.Connect((EndPoint) endPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("connect_ex(address) -> error_code\n\nLike connect(), but return an error code insted of raising an exception for\nsocket exceptions raised by the underlying system Connect() call. Note that\nexceptions other than SocketException generated by the system Connect() call\nwill still be raised.\n\nA return value of 0 indicates that the connect call was successful.\nDifference from CPython: connect_ex() does not support timeouts in blocking\nmode. If a timeout is set and the socket is in blocking mode, connect_ex() will\nblock indefinitely until a connection is made or an error occurs.")]
    public int connect_ex(PythonTuple address)
    {
      IPEndPoint endPoint = PythonSocket.TupleToEndPoint(this._context, address, this._socket.AddressFamily, out this._hostName);
      try
      {
        this._socket.Connect((EndPoint) endPoint);
      }
      catch (SocketException ex)
      {
        return (int) ex.SocketErrorCode;
      }
      return 0;
    }

    [Documentation("fileno() -> file_handle\n\nReturn the underlying system handle for this socket (a 64-bit integer).")]
    public long fileno()
    {
      try
      {
        return this._socket.Handle.ToInt64();
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("getpeername() -> address\n\nReturn the address of the remote end of this socket. The address format is\nfamily-dependent (e.g. a (host, port) tuple for IPv4).")]
    public PythonTuple getpeername()
    {
      try
      {
        if (!(this._socket.RemoteEndPoint is IPEndPoint remoteEndPoint))
          throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10047));
        return PythonSocket.EndPointToTuple(remoteEndPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("getsockname() -> address\n\nReturn the address of the local end of this socket. The address format is\nfamily-dependent (e.g. a (host, port) tuple for IPv4).")]
    public PythonTuple getsockname()
    {
      try
      {
        if (!(this._socket.LocalEndPoint is IPEndPoint localEndPoint))
          throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10022));
        return PythonSocket.EndPointToTuple(localEndPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("getsockopt(level, optname[, buflen]) -> value\n\nReturn the value of a socket option. level is one of the SOL_* constants\ndefined in this module, and optname is one of the SO_* constants. If buflen is\nomitted or zero, an integer value is returned. If it is present, a byte string\nwhose maximum length is buflen bytes) is returned. The caller must the decode\nthe resulting byte string.")]
    public object getsockopt(int optionLevel, int optionName, int optionLength = 0)
    {
      SocketOptionLevel optionLevel1 = (SocketOptionLevel) System.Enum.ToObject(typeof (SocketOptionLevel), optionLevel);
      if (!System.Enum.IsDefined(typeof (SocketOptionLevel), (object) optionLevel1))
        throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10022));
      SocketOptionName optionName1 = (SocketOptionName) System.Enum.ToObject(typeof (SocketOptionName), optionName);
      if (!System.Enum.IsDefined(typeof (SocketOptionName), (object) optionName1))
        throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10042));
      try
      {
        return optionLength == 0 ? (object) (int) this._socket.GetSocketOption(optionLevel1, optionName1) : (object) ((IList<byte>) this._socket.GetSocketOption(optionLevel1, optionName1, optionLength)).MakeString();
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("listen(backlog) -> None\n\nListen for connections on the socket. Backlog is the maximum length of the\npending connections queue. The maximum value is system-dependent.")]
    public void listen(int backlog)
    {
      try
      {
        this._socket.Listen(backlog);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("makefile([mode[, bufsize]]) -> file object\n\nReturn a regular file object corresponding to the socket.  The mode\nand bufsize arguments are as for the built-in open() function.")]
    public PythonFile makefile(string mode = "r", int bufSize = 8192 /*0x2000*/)
    {
      Interlocked.Increment(ref this._referenceCount);
      return (PythonFile) new PythonSocket._fileobject(this._context, (object) this, mode, bufSize);
    }

    [Documentation("recv(bufsize[, flags]) -> string\n\nReceive data from the socket, up to bufsize bytes. For connection-oriented\nprotocols (e.g. SOCK_STREAM), you must first call either connect() or\naccept(). Connectionless protocols (e.g. SOCK_DGRAM) may also use recvfrom().\n\nrecv() blocks until data is available, unless a timeout was set using\nsettimeout(). If the timeout was exceeded, socket.timeout is raised.recv() returns immediately with zero bytes when the connection is closed.")]
    public string recv(int maxBytes, int flags = 0)
    {
      byte[] numArray = maxBytes >= 0 ? new byte[maxBytes] : throw PythonOps.ValueError("negative buffersize in recv");
      int maxBytes1;
      try
      {
        maxBytes1 = this._socket.Receive(numArray, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex, SocketError.NotConnected);
      }
      return ((IList<byte>) numArray).MakeString(maxBytes1);
    }

    [Documentation("recv_into(buffer, [nbytes[, flags]]) -> nbytes_read\n\nA version of recv() that stores its data into a buffer rather than creating\na new string.  Receive up to buffersize bytes from the socket.  If buffersize\nis not specified (or 0), receive up to the size available in the given buffer.\n\nSee recv() for documentation about the flags.\n")]
    public int recv_into(PythonBuffer buffer, int nbytes = 0, int flags = 0)
    {
      if (nbytes < 0)
        throw PythonOps.ValueError("negative buffersize in recv_into");
      throw PythonOps.TypeError("buffer is read-only");
    }

    [Documentation("recv_into(buffer, [nbytes[, flags]]) -> nbytes_read\n\nA version of recv() that stores its data into a buffer rather than creating\na new string.  Receive up to buffersize bytes from the socket.  If buffersize\nis not specified (or 0), receive up to the size available in the given buffer.\n\nSee recv() for documentation about the flags.\n")]
    public int recv_into(string buffer, int nbytes = 0, int flags = 0)
    {
      throw PythonOps.TypeError("Cannot use string as modifiable buffer");
    }

    [Documentation("recv_into(buffer, [nbytes[, flags]]) -> nbytes_read\n\nA version of recv() that stores its data into a buffer rather than creating\na new string.  Receive up to buffersize bytes from the socket.  If buffersize\nis not specified (or 0), receive up to the size available in the given buffer.\n\nSee recv() for documentation about the flags.\n")]
    public int recv_into(ArrayModule.array buffer, int nbytes = 0, int flags = 0)
    {
      byte[] buffer1 = new byte[PythonSocket.socket.byteBufferSize(nameof (recv_into), nbytes, buffer.__len__(), buffer.itemsize)];
      int num;
      try
      {
        num = this._socket.Receive(buffer1, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex, SocketError.NotConnected);
      }
      buffer.FromStream((Stream) new MemoryStream(buffer1), 0);
      return num;
    }

    [Documentation("recv_into(bytearray, [nbytes[, flags]]) -> nbytes_read\n\nA version of recv() that stores its data into a bytearray rather than creating\na new string.  Receive up to buffersize bytes from the socket.  If buffersize\nis not specified (or 0), receive up to the size available in the given buffer.\n\nSee recv() for documentation about the flags.\n")]
    public int recv_into(ByteArray buffer, int nbytes = 0, int flags = 0)
    {
      byte[] buffer1 = new byte[PythonSocket.socket.byteBufferSize(nameof (recv_into), nbytes, buffer.Count, 1)];
      int num;
      try
      {
        num = this._socket.Receive(buffer1, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex, SocketError.NotConnected);
      }
      for (int index = 0; index < num; ++index)
        buffer[index] = (object) buffer1[index];
      return num;
    }

    [Documentation("recv_into(memoryview, [nbytes[, flags]]) -> nbytes_read\n\nA version of recv() that stores its data into a bytearray rather than creating\na new string.  Receive up to buffersize bytes from the socket.  If buffersize\nis not specified (or 0), receive up to the size available in the given buffer.\n\nSee recv() for documentation about the flags.\n")]
    public int recv_into(MemoryView buffer, int nbytes = 0, int flags = 0)
    {
      byte[] byteArray = buffer.tobytes().ToByteArray();
      int stop;
      try
      {
        stop = this._socket.Receive(byteArray, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        if (this._socket.SendTimeout == 0)
        {
          SocketException socketException = new SocketException(10057);
          throw PythonExceptions.CreateThrowable(PythonSocket.error(this._context), (object) 10057, (object) socketException.Message);
        }
        throw PythonSocket.MakeException(this._context, ex);
      }
      buffer[new IronPython.Runtime.Slice((object) 0, (object) stop)] = (object) ((IList<byte>) byteArray).Slice(new IronPython.Runtime.Slice((object) 0, (object) stop));
      return stop;
    }

    public int recv_into(object buffer, int nbytes = 0, int flags = 0)
    {
      throw PythonOps.TypeError($"recv_into() argument 1 must be read-write buffer, not {PythonOps.GetPythonTypeName(buffer)}");
    }

    [Documentation("recvfrom(bufsize[, flags]) -> (string, address)\n\nReceive data from the socket, up to bufsize bytes. string is the data\nreceived, and address (whose format is protocol-dependent) is the address of\nthe socket from which the data was received.")]
    public PythonTuple recvfrom(int maxBytes, int flags = 0)
    {
      byte[] numArray = maxBytes >= 0 ? new byte[maxBytes] : throw PythonOps.ValueError("negative buffersize in recvfrom");
      EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      int from;
      try
      {
        from = this._socket.ReceiveFrom(numArray, (SocketFlags) flags, ref remoteEP);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex);
      }
      return PythonTuple.MakeTuple((object) ((IList<byte>) numArray).MakeString(from), (object) PythonSocket.EndPointToTuple((IPEndPoint) remoteEP));
    }

    [Documentation("recvfrom_into(buffer[, nbytes[, flags]]) -> (nbytes, address info)\n\nLike recv_into(buffer[, nbytes[, flags]]) but also return the sender's address info.\n")]
    public PythonTuple recvfrom_into(PythonBuffer buffer, int nbytes = 0, int flags = 0)
    {
      if (nbytes < 0)
        throw PythonOps.ValueError("negative buffersize in recvfrom_into");
      throw PythonOps.TypeError("buffer is read-only");
    }

    [Documentation("recvfrom_into(buffer[, nbytes[, flags]]) -> (nbytes, address info)\n\nLike recv_into(buffer[, nbytes[, flags]]) but also return the sender's address info.\n")]
    public PythonTuple recvfrom_into(string buffer, int nbytes = 0, int flags = 0)
    {
      throw PythonOps.TypeError("Cannot use string as modifiable buffer");
    }

    [Documentation("recvfrom_into(buffer[, nbytes[, flags]]) -> (nbytes, address info)\n\nLike recv_into(buffer[, nbytes[, flags]]) but also return the sender's address info.\n")]
    public PythonTuple recvfrom_into(ArrayModule.array buffer, int nbytes = 0, int flags = 0)
    {
      byte[] buffer1 = new byte[PythonSocket.socket.byteBufferSize(nameof (recvfrom_into), nbytes, buffer.__len__(), buffer.itemsize)];
      EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      int from;
      try
      {
        from = this._socket.ReceiveFrom(buffer1, (SocketFlags) flags, ref remoteEP);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex);
      }
      buffer.FromStream((Stream) new MemoryStream(buffer1), 0);
      PythonTuple tuple = PythonSocket.EndPointToTuple((IPEndPoint) remoteEP);
      return PythonTuple.MakeTuple((object) from, (object) tuple);
    }

    [Documentation("recvfrom_into(buffer[, nbytes[, flags]]) -> (nbytes, address info)\n\nLike recv_into(buffer[, nbytes[, flags]]) but also return the sender's address info.\n")]
    public PythonTuple recvfrom_into(MemoryView buffer, int nbytes = 0, int flags = 0)
    {
      byte[] byteArray = buffer.tobytes().ToByteArray();
      EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      int from;
      try
      {
        from = this._socket.ReceiveFrom(byteArray, (SocketFlags) flags, ref remoteEP);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex);
      }
      buffer[new IronPython.Runtime.Slice((object) 0, (object) from)] = (object) ((IList<byte>) byteArray).Slice(new IronPython.Runtime.Slice((object) 0, (object) from));
      PythonTuple tuple = PythonSocket.EndPointToTuple((IPEndPoint) remoteEP);
      return PythonTuple.MakeTuple((object) from, (object) tuple);
    }

    [Documentation("recvfrom_into(buffer[, nbytes[, flags]]) -> (nbytes, address info)\n\nLike recv_into(buffer[, nbytes[, flags]]) but also return the sender's address info.\n")]
    public PythonTuple recvfrom_into(IList<byte> buffer, int nbytes = 0, int flags = 0)
    {
      byte[] buffer1 = new byte[PythonSocket.socket.byteBufferSize(nameof (recvfrom_into), nbytes, buffer.Count, 1)];
      EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      int from;
      try
      {
        from = this._socket.ReceiveFrom(buffer1, (SocketFlags) flags, ref remoteEP);
      }
      catch (Exception ex)
      {
        throw this.MakeRecvException(ex);
      }
      for (int index = 0; index < buffer1.Length; ++index)
        buffer[index] = buffer1[index];
      PythonTuple tuple = PythonSocket.EndPointToTuple((IPEndPoint) remoteEP);
      return PythonTuple.MakeTuple((object) from, (object) tuple);
    }

    public PythonTuple recvfrom_into(object buffer, int nbytes = 0, int flags = 0)
    {
      throw PythonOps.TypeError($"recvfrom_into() argument 1 must be read-write buffer, not {PythonOps.GetPythonTypeName(buffer)}");
    }

    private static int byteBufferSize(string funcName, int nbytes, int bufLength, int itemSize)
    {
      if (nbytes < 0)
        throw PythonOps.ValueError("negative buffersize in " + funcName);
      if (nbytes == 0)
        return bufLength * itemSize;
      int num = nbytes % itemSize;
      return Math.Min(num == 0 ? nbytes : nbytes + itemSize - num, bufLength * itemSize);
    }

    private Exception MakeRecvException(Exception e, SocketError errorCode = SocketError.InvalidArgument)
    {
      if (this._socket.SendTimeout != 0)
        return PythonSocket.MakeException(this._context, e);
      SocketException socketException = new SocketException((int) errorCode);
      return PythonExceptions.CreateThrowable(PythonSocket.error(this._context), (object) 10022, (object) socketException.Message);
    }

    [Documentation("send(string[, flags]) -> bytes_sent\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept(). Returns the number of bytes\nsent to the remote socket.\n\nNote that the successful completion of a send() call does not mean that all of\nthe data was sent. The caller must keep track of the number of bytes sent and\nretry the operation until all of the data has been sent.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public int send(string data, int flags = 0)
    {
      byte[] buffer = data.MakeByteArray();
      try
      {
        return this._socket.Send(buffer, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("send(string[, flags]) -> bytes_sent\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept(). Returns the number of bytes\nsent to the remote socket.\n\nNote that the successful completion of a send() call does not mean that all of\nthe data was sent. The caller must keep track of the number of bytes sent and\nretry the operation until all of the data has been sent.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public int send(Bytes data, int flags = 0)
    {
      byte[] unsafeByteArray = data.GetUnsafeByteArray();
      try
      {
        return this._socket.Send(unsafeByteArray, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("send(string[, flags]) -> bytes_sent\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept(). Returns the number of bytes\nsent to the remote socket.\n\nNote that the successful completion of a send() call does not mean that all of\nthe data was sent. The caller must keep track of the number of bytes sent and\nretry the operation until all of the data has been sent.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public int send(PythonBuffer data, int flags = 0)
    {
      byte[] byteCache = data.byteCache;
      try
      {
        return this._socket.Send(byteCache, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    public int send(MemoryView data, int flags = 0) => this.send(data.tobytes(), flags);

    [Documentation("sendall(string[, flags]) -> None\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept().\n\nUnlike send(), sendall() blocks until all of the data has been sent or until a\ntimeout or an error occurs. None is returned on success. If an error occurs,\nthere is no way to tell how much data, if any, was sent.\n\nDifference from CPython: timeouts do not function as you would expect. The\nfunction is implemented using multiple calls to send(), so the timeout timer\nis reset after each of those calls. That means that the upper bound on the\ntime that it will take for sendall() to return is the number of bytes in\nstring times the timeout interval.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public void sendall(string data, int flags = 0)
    {
      this.sendallWorker(data.MakeByteArray(), flags);
    }

    [Documentation("sendall(string[, flags]) -> None\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept().\n\nUnlike send(), sendall() blocks until all of the data has been sent or until a\ntimeout or an error occurs. None is returned on success. If an error occurs,\nthere is no way to tell how much data, if any, was sent.\n\nDifference from CPython: timeouts do not function as you would expect. The\nfunction is implemented using multiple calls to send(), so the timeout timer\nis reset after each of those calls. That means that the upper bound on the\ntime that it will take for sendall() to return is the number of bytes in\nstring times the timeout interval.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public void sendall(Bytes data, int flags = 0)
    {
      this.sendallWorker(data.GetUnsafeByteArray(), flags);
    }

    [Documentation("sendall(string[, flags]) -> None\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept().\n\nUnlike send(), sendall() blocks until all of the data has been sent or until a\ntimeout or an error occurs. None is returned on success. If an error occurs,\nthere is no way to tell how much data, if any, was sent.\n\nDifference from CPython: timeouts do not function as you would expect. The\nfunction is implemented using multiple calls to send(), so the timeout timer\nis reset after each of those calls. That means that the upper bound on the\ntime that it will take for sendall() to return is the number of bytes in\nstring times the timeout interval.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public void sendall(PythonBuffer data, int flags = 0)
    {
      this.sendallWorker(data.byteCache, flags);
    }

    [Documentation("sendall(string[, flags]) -> None\n\nSend data to the remote socket. The socket must be connected to a remote\nsocket (by calling either connect() or accept().\n\nUnlike send(), sendall() blocks until all of the data has been sent or until a\ntimeout or an error occurs. None is returned on success. If an error occurs,\nthere is no way to tell how much data, if any, was sent.\n\nDifference from CPython: timeouts do not function as you would expect. The\nfunction is implemented using multiple calls to send(), so the timeout timer\nis reset after each of those calls. That means that the upper bound on the\ntime that it will take for sendall() to return is the number of bytes in\nstring times the timeout interval.\n\nAlso note that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public void sendall(MemoryView data, int flags = 0)
    {
      this.sendallWorker(data.tobytes().GetUnsafeByteArray(), flags);
    }

    private void sendallWorker(byte[] buffer, int flags)
    {
      try
      {
        int length = buffer.Length;
        int size = length;
        while (size > 0)
          size -= this._socket.Send(buffer, length - size, size, (SocketFlags) flags);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("sendto(string[, flags], address) -> bytes_sent\n\nSend data to the remote socket. The socket does not need to be connected to a\nremote socket since the address is specified in the call to sendto(). Returns\nthe number of bytes sent to the remote socket.\n\nBlocking sockets will block until the all of the bytes in the buffer are sent.\nSince a nonblocking Socket completes immediately, it might not send all of the\nbytes in the buffer. It is your application's responsibility to keep track of\nthe number of bytes sent and to retry the operation until the application sends\nall of the bytes in the buffer.\n\nNote that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public int sendto(string data, int flags, PythonTuple address)
    {
      byte[] buffer = data.MakeByteArray();
      EndPoint endPoint = (EndPoint) PythonSocket.TupleToEndPoint(this._context, address, this._socket.AddressFamily, out this._hostName);
      try
      {
        return this._socket.SendTo(buffer, (SocketFlags) flags, endPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("sendto(string[, flags], address) -> bytes_sent\n\nSend data to the remote socket. The socket does not need to be connected to a\nremote socket since the address is specified in the call to sendto(). Returns\nthe number of bytes sent to the remote socket.\n\nBlocking sockets will block until the all of the bytes in the buffer are sent.\nSince a nonblocking Socket completes immediately, it might not send all of the\nbytes in the buffer. It is your application's responsibility to keep track of\nthe number of bytes sent and to retry the operation until the application sends\nall of the bytes in the buffer.\n\nNote that there is no guarantee that the data you send will appear on the\nnetwork immediately. To increase network efficiency, the underlying system may\ndelay transmission until a significant amount of outgoing data is collected. A\nsuccessful completion of the Send method means that the underlying system has\nhad room to buffer your data for a network send")]
    public int sendto(Bytes data, int flags, PythonTuple address)
    {
      byte[] unsafeByteArray = data.GetUnsafeByteArray();
      EndPoint endPoint = (EndPoint) PythonSocket.TupleToEndPoint(this._context, address, this._socket.AddressFamily, out this._hostName);
      try
      {
        return this._socket.SendTo(unsafeByteArray, (SocketFlags) flags, endPoint);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    public int sendto(MemoryView data, int flags, PythonTuple address)
    {
      return this.sendto(data.tobytes(), flags, address);
    }

    [Documentation("")]
    public int sendto(string data, PythonTuple address) => this.sendto(data, 0, address);

    [Documentation("")]
    public int sendto(Bytes data, PythonTuple address) => this.sendto(data, 0, address);

    public int sendto(MemoryView data, PythonTuple address) => this.sendto(data, 0, address);

    [Documentation("setblocking(flag) -> None\n\nSet the blocking mode of the socket. If flag is 0, the socket will be set to\nnon-blocking mode; otherwise, it will be set to blocking mode. If the socket is\nin blocking mode, and a method is called (such as send() or recv() which does\nnot complete immediately, the caller will block execution until the requested\noperation completes. In non-blocking mode, a socket.timeout exception would\nwould be raised in this case.\n\nNote that changing blocking mode also affects the timeout setting:\nsetblocking(0) is equivalent to settimeout(0), and setblocking(1) is equivalent\nto settimeout(None).")]
    public void setblocking(int shouldBlock)
    {
      if (shouldBlock == 0)
        this.settimeout((object) 0);
      else
        this.settimeout((object) null);
    }

    [Documentation("settimeout(value) -> None\n\nSet a timeout on blocking socket methods. value may be either None or a\nnon-negative float, with one of the following meanings:\n - None: disable timeouts and block indefinitely - 0.0: don't block at all (return immediately if the operation can be\n   completed; raise socket.error otherwise)\n - float > 0.0: block for up to the specified number of seconds; raise\n   socket.timeout if the operation cannot be completed in time\n\nsettimeout(None) is equivalent to setblocking(1), and settimeout(0.0) is\nequivalent to setblocking(0).\nIf the timeout is non-zero and is less than 0.5, it will be set to 0.5. This\nlimitation is specific to IronPython.\n")]
    public void settimeout(object timeout)
    {
      try
      {
        if (timeout == null)
        {
          this._socket.Blocking = true;
          this._socket.SendTimeout = 0;
        }
        else
        {
          double num = Converter.ConvertToDouble(timeout);
          if (num < 0.0)
            throw PythonOps.ValueError("Timeout value out of range");
          this._socket.Blocking = num > 0.0;
          this._socket.SendTimeout = (int) (num * 1000.0);
          this._timeout = (int) (num * 1000.0);
        }
      }
      finally
      {
        this._socket.ReceiveTimeout = this._socket.SendTimeout;
      }
    }

    [Documentation("gettimeout() -> value\n\nReturn the timeout duration in seconds for this socket as a float. If no\ntimeout is set, return None. For more details on timeouts and blocking, see the\nPython socket module documentation.")]
    public object gettimeout()
    {
      try
      {
        return this._socket.Blocking && this._socket.SendTimeout == 0 ? (object) null : (object) ((double) this._socket.SendTimeout / 1000.0);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    [Documentation("setsockopt(level, optname[, value]) -> None\n\nSet the value of a socket option. level is one of the SOL_* constants defined\nin this module, and optname is one of the SO_* constants. value may be either\nan integer or a string containing a binary structure. The caller is responsible\nfor properly encoding the byte string.")]
    public void setsockopt(int optionLevel, int optionName, object value)
    {
      SocketOptionLevel optionLevel1 = (SocketOptionLevel) System.Enum.ToObject(typeof (SocketOptionLevel), optionLevel);
      if (!System.Enum.IsDefined(typeof (SocketOptionLevel), (object) optionLevel1))
        throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10022));
      SocketOptionName optionName1 = (SocketOptionName) System.Enum.ToObject(typeof (SocketOptionName), optionName);
      if (!System.Enum.IsDefined(typeof (SocketOptionName), (object) optionName1))
        throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10042));
      try
      {
        int result1;
        if (Converter.TryConvertToInt32(value, out result1))
        {
          this._socket.SetSocketOption(optionLevel1, optionName1, result1);
          return;
        }
        string result2;
        if (Converter.TryConvertToString(value, out result2))
        {
          this._socket.SetSocketOption(optionLevel1, optionName1, result2.MakeByteArray());
          return;
        }
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
      throw PythonOps.TypeError("setsockopt() argument 3 must be int or string");
    }

    [Documentation("shutdown() -> None\n\nReturn the timeout duration in seconds for this socket as a float. If no\ntimeout is set, return None. For more details on timeouts and blocking, see the\nPython socket module documentation.")]
    public void shutdown(int how)
    {
      SocketShutdown how1 = (SocketShutdown) System.Enum.ToObject(typeof (SocketShutdown), how);
      if (!System.Enum.IsDefined(typeof (SocketShutdown), (object) how1))
        throw PythonSocket.MakeException(this._context, (Exception) new SocketException(10022));
      try
      {
        this._socket.Shutdown(how1);
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(this._context, ex);
      }
    }

    public int family => (int) this._socket.AddressFamily;

    public int type => (int) this._socket.SocketType;

    public int proto => (int) this._socket.ProtocolType;

    public int ioctl(BigInteger cmd, object option)
    {
      if (cmd == PythonSocket.SIO_KEEPALIVE_VALS)
      {
        PythonTuple pythonTuple = option is PythonTuple ? (PythonTuple) option : throw PythonOps.TypeError("option must be 3-item sequence, not int");
        if (pythonTuple.Count != 3)
          throw PythonOps.TypeError($"option must be sequence of length 3, not {pythonTuple.Count}");
        int num1 = pythonTuple[0] is int || pythonTuple[1] is int || pythonTuple[2] is int ? (int) pythonTuple[0] : throw PythonOps.TypeError("option integer required");
        int num2 = (int) pythonTuple[1];
        int num3 = (int) pythonTuple[2];
        int num4 = 4;
        byte[] numArray = new byte[num4 * 3];
        Array.Copy((Array) BitConverter.GetBytes(num1), 0, (Array) numArray, 0, num4);
        Array.Copy((Array) BitConverter.GetBytes(num2), 0, (Array) numArray, num4, num4);
        Array.Copy((Array) BitConverter.GetBytes(num4), 0, (Array) numArray, num4 * 2, num4);
        return this._socket.IOControl((IOControlCode) (long) cmd, numArray, (byte[]) null);
      }
      if (!(cmd == PythonSocket.SIO_RCVALL))
        throw PythonOps.ValueError($"invalid ioctl command {cmd}");
      if (!(option is int num))
        throw PythonOps.TypeError("option integer required");
      return this._socket.IOControl((IOControlCode) (long) cmd, BitConverter.GetBytes(num), (byte[]) null);
    }

    public override string ToString()
    {
      try
      {
        return $"<socket object, fd={this.fileno()}, family={this.family}, type={this.type}, protocol={this.proto}>";
      }
      catch
      {
        return "<socket object, fd=?, family=?, type=, protocol=>";
      }
    }

    internal static Socket HandleToSocket(long handle)
    {
      lock (PythonSocket.socket._handleToSocket)
      {
        WeakReference weakReference;
        if (PythonSocket.socket._handleToSocket.TryGetValue((IntPtr) handle, out weakReference))
          return weakReference.Target as Socket;
      }
      return (Socket) null;
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakRefTracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._weakRefTracker = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._weakRefTracker = value;

    private socket(CodeContext context, Socket socket) => this.Initialize(context, socket);

    private void Initialize(CodeContext context, Socket socket)
    {
      this._socket = socket;
      this._context = context;
      int? defaultTimeout = PythonSocket.GetDefaultTimeout(context);
      if (!defaultTimeout.HasValue)
        this.settimeout((object) null);
      else
        this.settimeout((object) ((double) defaultTimeout.Value / 1000.0));
      lock (PythonSocket.socket._handleToSocket)
        PythonSocket.socket._handleToSocket[socket.Handle] = new WeakReference((object) socket);
    }
  }

  private class PythonUserSocketStream : Stream
  {
    private readonly object _userSocket;
    private List<string> _data = new List<string>();
    private int _dataSize;
    private readonly int _bufSize;
    private readonly bool _close;

    public PythonUserSocketStream(object userSocket, int bufferSize, bool close)
    {
      this._userSocket = userSocket;
      this._bufSize = bufferSize;
      this._close = close;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override void Flush()
    {
      if (this._data.Count <= 0)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in this._data)
        stringBuilder.Append(str);
      DefaultContext.DefaultPythonContext.CallSplat(PythonOps.GetBoundAttr(DefaultContext.Default, this._userSocket, "sendall"), (object) stringBuilder.ToString());
      this._data.Clear();
    }

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      string s = Converter.ConvertToString(DefaultContext.DefaultPythonContext.CallSplat(PythonOps.GetBoundAttr(DefaultContext.Default, this._userSocket, "recv"), (object) count));
      return PythonAsciiEncoding.Instance.GetBytes(s, 0, s.Length, buffer, offset);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      string str = new string(PythonAsciiEncoding.Instance.GetChars(buffer, offset, count));
      this._data.Add(str);
      this._dataSize += str.Length;
      if (this._dataSize <= this._bufSize)
        return;
      this.Flush();
    }

    protected override void Dispose(bool disposing)
    {
      object ret;
      if (disposing && PythonOps.TryGetBoundAttr(this._userSocket, "close", out ret))
        PythonCalls.Call(ret);
      base.Dispose(disposing);
    }
  }

  [PythonType]
  public class _fileobject : PythonFile
  {
    public const string name = "<socket>";
    private readonly PythonSocket.socket _socket;
    public const string __module__ = "socket";
    private bool _close;
    public object _sock;
    public object bufsize = (object) 8192 /*0x2000*/;

    public _fileobject(CodeContext context, object socket, string mode = "rb", int bufsize = -1, bool close = false)
      : base(context.LanguageContext)
    {
      this._close = close;
      Stream stream;
      if (socket != null && socket.GetType() == typeof (PythonSocket.socket) && ((PythonSocket.socket) socket)._socket.Connected)
      {
        PythonSocket.socket socket1 = socket as PythonSocket.socket;
        this._socket = socket1;
        stream = (Stream) new NetworkStream(socket1._socket, false);
      }
      else
      {
        this._socket = (PythonSocket.socket) null;
        stream = (Stream) new PythonSocket.PythonUserSocketStream(socket, PythonSocket._fileobject.GetBufferSize(context, bufsize), close);
      }
      this._sock = socket;
      this.__init__(stream, Encoding.GetEncoding(0), mode);
      this._isOpen = socket != null;
      this._close = socket != null && close;
    }

    public void __init__(params object[] args)
    {
    }

    public void __init__([ParamDictionary] IDictionary<object, object> kwargs, params object[] args)
    {
    }

    public void __del__()
    {
      if (this._socket == null || !this._isOpen)
        return;
      if (this._close)
        this._socket.close();
      this._isOpen = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (this._socket != null && this._isOpen)
      {
        if (this._close)
          this._socket.close();
        this._isOpen = false;
      }
      base.Dispose(disposing);
    }

    public override object close()
    {
      if (!this._isOpen)
        return (object) null;
      if (this._socket != null && this._close)
        this._socket.close();
      else if (this._stream != null && this._close)
        this._stream.Dispose();
      this._isOpen = false;
      return base.close();
    }

    private static int GetBufferSize(CodeContext context, int size)
    {
      return size == -1 ? Converter.ConvertToInt32(PythonSocket._fileobject.Getdefault_bufsize(context)) : size;
    }

    [PropertyMethod]
    [StaticExtensionMethod]
    [SpecialName]
    public static object Getdefault_bufsize(CodeContext context)
    {
      return context.LanguageContext.GetModuleState(PythonSocket._defaultBufsizeKey);
    }

    [PropertyMethod]
    [StaticExtensionMethod]
    [SpecialName]
    public static void Setdefault_bufsize(CodeContext context, object value)
    {
      context.LanguageContext.SetModuleState(PythonSocket._defaultBufsizeKey, value);
    }
  }

  public class ssl
  {
    private SslStream _sslStream;
    private PythonSocket.socket _socket;
    private readonly X509Certificate2Collection _certCollection;
    private readonly X509Certificate _cert;
    private readonly int _protocol;
    private readonly int _certsMode;
    private readonly bool _validate;
    private readonly bool _serverSide;
    private readonly CodeContext _context;
    private readonly RemoteCertificateValidationCallback _callback;
    private Exception _validationFailure;
    internal string _serverHostName;

    public ssl(
      CodeContext context,
      PythonSocket.socket sock,
      string keyfile = null,
      string certfile = null,
      X509Certificate2Collection certs = null)
    {
      this._context = context;
      this._sslStream = new SslStream((Stream) new NetworkStream(sock._socket, false), true, new RemoteCertificateValidationCallback(this.CertValidationCallback));
      this._socket = sock;
      this._protocol = 50331650 /*0x03000002*/;
      this._validate = false;
      this._certCollection = certs ?? new X509Certificate2Collection();
    }

    internal ssl(
      CodeContext context,
      PythonSocket.socket sock,
      bool server_side,
      string keyfile = null,
      string certfile = null,
      int certs_mode = 0,
      int protocol = 50331650 /*0x03000002*/,
      string cacertsfile = null,
      X509Certificate2Collection certs = null)
    {
      if (sock == null)
        throw PythonOps.TypeError("expected socket object, got None");
      this._serverSide = server_side;
      this._certsMode = certs_mode;
      bool flag;
      RemoteCertificateValidationCallback validationCallback;
      switch (certs_mode)
      {
        case 0:
          flag = false;
          validationCallback = new RemoteCertificateValidationCallback(this.CertValidationCallback);
          break;
        case 1:
          flag = true;
          validationCallback = new RemoteCertificateValidationCallback(this.CertValidationCallbackOptional);
          break;
        case 2:
          flag = true;
          validationCallback = new RemoteCertificateValidationCallback(this.CertValidationCallbackRequired);
          break;
        default:
          throw new InvalidOperationException($"bad certs_mode: {certs_mode}");
      }
      this._callback = validationCallback;
      if (certs != null)
        this._certCollection = certs;
      if (certfile != null)
        this._cert = (X509Certificate) PythonSsl.ReadCertificate(context, certfile);
      if (cacertsfile != null)
        this._certCollection = new X509Certificate2Collection(new X509Certificate2[1]
        {
          PythonSsl.ReadCertificate(context, cacertsfile)
        });
      this._socket = sock;
      this.EnsureSslStream(false);
      this._protocol = protocol;
      this._validate = flag;
      this._context = context;
    }

    private void EnsureSslStream(bool throwWhenNotConnected)
    {
      if (this._sslStream == null && this._socket._socket.Connected)
        this._sslStream = !this._serverSide ? new SslStream((Stream) new NetworkStream(this._socket._socket, false), true, this._callback, new LocalCertificateSelectionCallback(this.CertSelectLocal)) : new SslStream((Stream) new NetworkStream(this._socket._socket, false), true, this._callback);
      if (throwWhenNotConnected && this._sslStream == null)
        throw PythonExceptions.CreateThrowable(PythonSocket.GetSocketError(this._context.LanguageContext, this._context.LanguageContext.GetBuiltinModule("socket").__dict__), (object) 10057, (object) "A request to send or receive data was disallowed because the socket is not connected.");
    }

    internal bool CertValidationCallback(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      return true;
    }

    internal bool CertValidationCallbackOptional(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      if (!this._serverSide && certificate != null && sslPolicyErrors != SslPolicyErrors.None)
        this.ValidateCertificate(certificate, chain, sslPolicyErrors);
      return true;
    }

    internal X509Certificate CertSelectLocal(
      object sender,
      string targetHost,
      X509CertificateCollection collection,
      X509Certificate remoteCertificate,
      string[] acceptableIssuers)
    {
      if (acceptableIssuers != null && acceptableIssuers.Length != 0 && collection != null && collection.Count > 0)
      {
        foreach (X509Certificate x509Certificate in collection)
        {
          string issuer = x509Certificate.Issuer;
          if (Array.IndexOf<string>(acceptableIssuers, issuer) != -1)
            return x509Certificate;
        }
      }
      return collection != null && collection.Count > 0 ? collection[0] : (X509Certificate) null;
    }

    internal bool CertValidationCallbackRequired(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      if (!this._serverSide)
      {
        if (certificate == null)
          this.ValidationError((object) SslPolicyErrors.None);
        else if (sslPolicyErrors != SslPolicyErrors.None)
          this.ValidateCertificate(certificate, chain, sslPolicyErrors);
      }
      return true;
    }

    private void ValidateCertificate(
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      chain = new X509Chain();
      X509Certificate2Collection certificates = new X509Certificate2Collection();
      foreach (object cert in this._certCollection)
      {
        if (cert is X509Certificate2)
          certificates.Add((X509Certificate2) cert);
        else if (cert is X509Certificate)
          certificates.Add(new X509Certificate2((X509Certificate) cert));
      }
      chain.ChainPolicy.ExtraStore.AddRange(certificates);
      chain.Build(new X509Certificate2(certificate));
      if (chain.ChainStatus.Length == 0)
        return;
      foreach (X509ChainStatus chainStatu in chain.ChainStatus)
      {
        if (chainStatu.Status == X509ChainStatusFlags.UntrustedRoot)
        {
          bool flag = false;
          foreach (X509Certificate2 cert in this._certCollection)
          {
            if (certificate.Issuer == cert.Subject)
              flag = true;
          }
          if (flag)
            continue;
        }
        this.ValidationError((object) sslPolicyErrors);
        break;
      }
    }

    private void ValidationError(object reason)
    {
      this._validationFailure = PythonExceptions.CreateThrowable(PythonSsl.SSLError(this._context), (object) "errors while validating certificate chain: ", (object) reason.ToString());
    }

    public void do_handshake()
    {
      try
      {
        int available = this._socket._socket.Available;
      }
      catch (SocketException ex)
      {
        throw PythonExceptions.CreateThrowable(PythonExceptions.IOError, (object) "socket closed before handshake");
      }
      this.EnsureSslStream(true);
      SslProtocols protocolType = PythonSocket.ssl.GetProtocolType(this._protocol);
      try
      {
        if (this._serverSide)
        {
          this._sslStream.AuthenticateAsServer(this._cert, this._certsMode == 2, protocolType, false);
        }
        else
        {
          X509CertificateCollection clientCertificates = new X509CertificateCollection();
          if (this._cert != null)
            clientCertificates.Add(this._cert);
          this._sslStream.AuthenticateAsClient(this._serverHostName ?? this._socket._hostName, clientCertificates, protocolType, false);
        }
      }
      catch (AuthenticationException ex)
      {
        this._socket._socket.Dispose();
        throw PythonExceptions.CreateThrowable(PythonSsl.SSLError(this._context), (object) "errors while performing handshake: ", (object) ex.ToString());
      }
      if (this._validationFailure != null)
        throw this._validationFailure;
    }

    public PythonSocket.socket shutdown()
    {
      this._sslStream.Dispose();
      return this._socket;
    }

    private static SslProtocols GetProtocolType(int type)
    {
      SslProtocols sslProtocols;
      switch (type & -520224769)
      {
        case 0:
          sslProtocols = SslProtocols.Ssl2;
          break;
        case 1:
          sslProtocols = SslProtocols.Ssl3;
          break;
        case 2:
          sslProtocols = (SslProtocols) 4092;
          break;
        case 3:
          sslProtocols = SslProtocols.Tls;
          break;
        case 4:
          sslProtocols = (SslProtocols) 768 /*0x0300*/;
          break;
        case 5:
          sslProtocols = (SslProtocols) 3072 /*0x0C00*/;
          break;
        default:
          throw new InvalidOperationException("bad ssl protocol type: " + (object) type);
      }
      return sslProtocols & ((type & 33554432 /*0x02000000*/) != 0 ? ~SslProtocols.Ssl3 : ~SslProtocols.None) & ((type & 16777216 /*0x01000000*/) != 0 ? ~SslProtocols.Ssl2 : ~SslProtocols.None) & ((type & 67108864 /*0x04000000*/) != 0 ? ~SslProtocols.Tls : ~SslProtocols.None) & ((type & 268435456 /*0x10000000*/) != 0 ? (SslProtocols) -769 : ~SslProtocols.None) & ((type & 134217728 /*0x08000000*/) != 0 ? (SslProtocols) -3073 : ~SslProtocols.None);
    }

    public PythonTuple cipher()
    {
      if (this._sslStream == null || !this._sslStream.IsAuthenticated)
        return (PythonTuple) null;
      return PythonTuple.MakeTuple((object) this._sslStream.CipherAlgorithm.ToString(), (object) this.ProtocolToPython(), (object) this._sslStream.CipherStrength);
    }

    private string ProtocolToPython()
    {
      switch (this._sslStream.SslProtocol)
      {
        case SslProtocols.Ssl2:
          return "SSLv2";
        case SslProtocols.Ssl3:
          return "TLSv1/SSLv3";
        case SslProtocols.Tls:
          return "TLSv1";
        default:
          return this._sslStream.SslProtocol.ToString();
      }
    }

    public object peer_certificate(bool binary_form)
    {
      if (this._sslStream != null)
      {
        X509Certificate remoteCertificate = this._sslStream.RemoteCertificate;
        if (remoteCertificate != null)
        {
          if (binary_form)
            return (object) ((IList<byte>) remoteCertificate.GetRawCertData()).MakeString();
          if (this._validate)
            return (object) PythonSsl.CertificateToPython(this._context, remoteCertificate);
        }
      }
      return (object) null;
    }

    public int pending() => this._socket._socket.Available;

    [Documentation("issuer() -> issuer_certificate\n\nReturns a string that describes the issuer of the server's certificate. Only useful for debugging purposes.")]
    public string issuer()
    {
      if (this._sslStream == null || !this._sslStream.IsAuthenticated)
        return string.Empty;
      X509Certificate remoteCertificate = this._sslStream.RemoteCertificate;
      return remoteCertificate != null ? remoteCertificate.Issuer : string.Empty;
    }

    [Documentation("read([len]) -> string\r\n\r\nRead up to len bytes from the SSL socket.")]
    public string read(CodeContext context, int len)
    {
      this.EnsureSslStream(true);
      try
      {
        byte[] buffer = new byte[2048 /*0x0800*/];
        MemoryStream memoryStream = new MemoryStream(len);
        int count1;
        int count2;
        do
        {
          count1 = len < buffer.Length ? len : buffer.Length;
          count2 = this._sslStream.Read(buffer, 0, count1);
          if (count2 > 0)
          {
            memoryStream.Write(buffer, 0, count2);
            len -= count2;
          }
        }
        while (count2 != 0 && len != 0 && count2 >= count1);
        return ((IList<byte>) memoryStream.ToArray()).MakeString();
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(context, ex);
      }
    }

    [Documentation("server() -> server_certificate\n\nReturns a string that describes the server's certificate. Only useful for debugging purposes.")]
    public string server()
    {
      if (this._sslStream != null && this._sslStream.IsAuthenticated)
      {
        X509Certificate remoteCertificate = this._sslStream.RemoteCertificate;
        if (remoteCertificate != null)
          return remoteCertificate.Subject;
      }
      return string.Empty;
    }

    [Documentation("write(s) -> len\r\n\r\nWrites the string s into the SSL object.  Returns the number\r\nof bytes written.")]
    public int write(CodeContext context, string data)
    {
      this.EnsureSslStream(true);
      byte[] buffer = data.MakeByteArray();
      try
      {
        this._sslStream.Write(buffer);
        return buffer.Length;
      }
      catch (Exception ex)
      {
        throw PythonSocket.MakeException(context, ex);
      }
    }
  }
}
