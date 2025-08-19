// Decompiled with JetBrains decompiler
// Type: Extreme.Net.Socks4ProxyClient
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Extreme.Net;

public class Socks4ProxyClient : ProxyClient
{
  protected internal const int DefaultPort = 1080;
  protected internal const byte VersionNumber = 4;
  protected internal const byte CommandConnect = 1;
  protected internal const byte CommandBind = 2;
  protected internal const byte CommandReplyRequestGranted = 90;
  protected internal const byte CommandReplyRequestRejectedOrFailed = 91;
  protected internal const byte CommandReplyRequestRejectedCannotConnectToIdentd = 92;
  protected internal const byte CommandReplyRequestRejectedDifferentIdentd = 93;

  public Socks4ProxyClient()
    : this((string) null)
  {
  }

  public Socks4ProxyClient(string host)
    : this(host, 1080)
  {
  }

  public Socks4ProxyClient(string host, int port)
    : this(host, port, string.Empty)
  {
  }

  public Socks4ProxyClient(string host, int port, string username)
    : base(ProxyType.Socks4, host, port, username, (string) null)
  {
  }

  public static Socks4ProxyClient Parse(string proxyAddress)
  {
    return ProxyClient.Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
  }

  public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
  {
    ProxyClient result1;
    if (ProxyClient.TryParse(ProxyType.Socks4, proxyAddress, out result1))
    {
      result = result1 as Socks4ProxyClient;
      return true;
    }
    result = (Socks4ProxyClient) null;
    return false;
  }

  public override TcpClient CreateConnection(
    string destinationHost,
    int destinationPort,
    TcpClient tcpClient = null)
  {
    this.CheckState();
    switch (destinationHost)
    {
      case null:
        throw new ArgumentNullException(nameof (destinationHost));
      case "":
        throw ExceptionHelper.EmptyString(nameof (destinationHost));
      default:
        if (!ExceptionHelper.ValidateTcpPort(destinationPort))
          throw ExceptionHelper.WrongTcpPort(nameof (destinationPort));
        TcpClient connection = tcpClient ?? this.CreateConnectionToProxy();
        try
        {
          this.SendCommand(connection.GetStream(), (byte) 1, destinationHost, destinationPort);
        }
        catch (Exception ex)
        {
          connection.Close();
          if (ex is IOException || ex is SocketException)
            throw this.NewProxyException(Resources.ProxyException_Error, ex);
          throw;
        }
        return connection;
    }
  }

  protected internal virtual void SendCommand(
    NetworkStream nStream,
    byte command,
    string destinationHost,
    int destinationPort)
  {
    byte[] ipAddressBytes = this.GetIPAddressBytes(destinationHost);
    byte[] portBytes = this.GetPortBytes(destinationPort);
    byte[] numArray = string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username);
    byte[] buffer1 = new byte[9 + numArray.Length];
    buffer1[0] = (byte) 4;
    buffer1[1] = command;
    portBytes.CopyTo((Array) buffer1, 2);
    ipAddressBytes.CopyTo((Array) buffer1, 4);
    numArray.CopyTo((Array) buffer1, 8);
    buffer1[8 + numArray.Length] = (byte) 0;
    nStream.Write(buffer1, 0, buffer1.Length);
    byte[] buffer2 = new byte[8];
    nStream.Read(buffer2, 0, buffer2.Length);
    byte command1 = buffer2[1];
    if (command1 == (byte) 90)
      return;
    this.HandleCommandError(command1);
  }

  protected internal byte[] GetIPAddressBytes(string destinationHost)
  {
    IPAddress address = (IPAddress) null;
    if (!IPAddress.TryParse(destinationHost, out address))
    {
      try
      {
        IPAddress[] hostAddresses = Dns.GetHostAddresses(destinationHost);
        if (hostAddresses.Length != 0)
          address = hostAddresses[0];
      }
      catch (Exception ex)
      {
        if (ex is SocketException || ex is ArgumentException)
          throw new ProxyException(string.Format(Resources.ProxyException_FailedGetHostAddresses, (object) destinationHost), (ProxyClient) this, ex);
        throw;
      }
    }
    return address.GetAddressBytes();
  }

  protected internal byte[] GetPortBytes(int port)
  {
    return new byte[2]
    {
      (byte) (port / 256 /*0x0100*/),
      (byte) (port % 256 /*0x0100*/)
    };
  }

  protected internal void HandleCommandError(byte command)
  {
    string str;
    switch (command)
    {
      case 91:
        str = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
        break;
      case 92:
        str = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
        break;
      case 93:
        str = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
        break;
      default:
        str = Resources.Socks_UnknownError;
        break;
    }
    throw new ProxyException(string.Format(Resources.ProxyException_CommandError, (object) str, (object) this.ToString()), (ProxyClient) this);
  }
}
