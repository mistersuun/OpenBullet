// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Socks4AProxyClient
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Leaf.xNet;

public sealed class Socks4AProxyClient : Socks4ProxyClient
{
  public Socks4AProxyClient()
    : this((string) null)
  {
  }

  public Socks4AProxyClient(string host)
    : this(host, 1080)
  {
  }

  public Socks4AProxyClient(string host, int port)
    : this(host, port, string.Empty)
  {
  }

  public Socks4AProxyClient(string host, int port, string username)
    : base(host, port, username)
  {
    this._type = ProxyType.Socks4A;
  }

  public static Socks4AProxyClient Parse(string proxyAddress)
  {
    return ProxyClient.Parse(ProxyType.Socks4A, proxyAddress) as Socks4AProxyClient;
  }

  public static bool TryParse(string proxyAddress, out Socks4AProxyClient result)
  {
    ProxyClient result1;
    if (!ProxyClient.TryParse(ProxyType.Socks4A, proxyAddress, out result1))
    {
      result = (Socks4AProxyClient) null;
      return false;
    }
    result = result1 as Socks4AProxyClient;
    return true;
  }

  internal new void SendCommand(
    NetworkStream nStream,
    byte command,
    string destinationHost,
    int destinationPort)
  {
    byte[] portBytes = this.GetPortBytes(destinationPort);
    byte[] numArray1 = new byte[4]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1
    };
    byte[] numArray2 = string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username);
    byte[] bytes = Encoding.ASCII.GetBytes(destinationHost);
    byte[] buffer1 = new byte[10 + numArray2.Length + bytes.Length];
    buffer1[0] = (byte) 4;
    buffer1[1] = command;
    portBytes.CopyTo((Array) buffer1, 2);
    numArray1.CopyTo((Array) buffer1, 4);
    numArray2.CopyTo((Array) buffer1, 8);
    buffer1[8 + numArray2.Length] = (byte) 0;
    bytes.CopyTo((Array) buffer1, 9 + numArray2.Length);
    buffer1[9 + numArray2.Length + bytes.Length] = (byte) 0;
    nStream.Write(buffer1, 0, buffer1.Length);
    byte[] buffer2 = new byte[8];
    nStream.Read(buffer2, 0, 8);
    byte command1 = buffer2[1];
    if (command1 == (byte) 90)
      return;
    this.HandleCommandError(command1);
  }
}
