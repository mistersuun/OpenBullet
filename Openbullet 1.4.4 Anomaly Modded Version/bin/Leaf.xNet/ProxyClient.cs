// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.ProxyClient
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;

#nullable disable
namespace Leaf.xNet;

public abstract class ProxyClient : IEquatable<ProxyClient>
{
  protected ProxyType _type;
  protected readonly string _host;
  protected readonly int _port = 1;
  protected string _username;
  protected string _password;
  protected int _connectTimeout = 9000;
  protected int _readWriteTimeout = 30000;
  private static HttpProxyClient _debugHttpProxy;
  private static Socks5ProxyClient _debugSocksProxy;
  public static readonly Dictionary<string, ProxyType> ProxyProtocol = new Dictionary<string, ProxyType>()
  {
    {
      "http",
      ProxyType.HTTP
    },
    {
      "https",
      ProxyType.HTTP
    },
    {
      "socks4",
      ProxyType.Socks4
    },
    {
      "socks4a",
      ProxyType.Socks4A
    },
    {
      "socks5",
      ProxyType.Socks5
    },
    {
      "socks",
      ProxyType.Socks5
    }
  };

  public virtual ProxyType Type => this._type;

  public virtual string Host => this._host;

  public virtual int Port => this._port;

  public virtual string Username
  {
    get => this._username;
    set
    {
      this._username = value == null || value.Length <= (int) byte.MaxValue ? value : throw new ArgumentOutOfRangeException(nameof (Username), string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, (object) (int) byte.MaxValue));
    }
  }

  public virtual string Password
  {
    get => this._password;
    set
    {
      this._password = value == null || value.Length <= (int) byte.MaxValue ? value : throw new ArgumentOutOfRangeException(nameof (Password), string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, (object) (int) byte.MaxValue));
    }
  }

  public virtual int ConnectTimeout
  {
    get => this._connectTimeout;
    set
    {
      this._connectTimeout = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ConnectTimeout), 0);
    }
  }

  public virtual int ReadWriteTimeout
  {
    get => this._readWriteTimeout;
    set
    {
      this._readWriteTimeout = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ReadWriteTimeout), 0);
    }
  }

  public bool AbsoluteUriInStartingLine { get; set; } = true;

  protected internal ProxyClient(ProxyType proxyType) => this._type = proxyType;

  protected internal ProxyClient(ProxyType proxyType, string address, int port)
  {
    this._type = proxyType;
    this._host = address;
    this._port = port;
  }

  protected internal ProxyClient(
    ProxyType proxyType,
    string address,
    int port,
    string username,
    string password)
  {
    this._type = proxyType;
    this._host = address;
    this._port = port;
    this._username = username;
    this._password = password;
  }

  public static HttpProxyClient DebugHttpProxy
  {
    get
    {
      if (ProxyClient._debugHttpProxy != null)
        return ProxyClient._debugHttpProxy;
      ProxyClient._debugHttpProxy = HttpProxyClient.Parse("127.0.0.1:8888");
      ProxyClient._debugHttpProxy.AbsoluteUriInStartingLine = false;
      return ProxyClient._debugHttpProxy;
    }
  }

  public static Socks5ProxyClient DebugSocksProxy
  {
    get
    {
      return ProxyClient._debugSocksProxy ?? (ProxyClient._debugSocksProxy = Socks5ProxyClient.Parse("127.0.0.1:8889"));
    }
  }

  public static ProxyClient Parse(ProxyType proxyType, string proxyAddress)
  {
    switch (proxyAddress)
    {
      case null:
        throw new ArgumentNullException(nameof (proxyAddress));
      case "":
        throw ExceptionHelper.EmptyString(nameof (proxyAddress));
      default:
        string[] strArray = proxyAddress.Split(':');
        int port = 0;
        string host = strArray[0];
        if (strArray.Length >= 2)
        {
          try
          {
            port = int.Parse(strArray[1]);
          }
          catch (Exception ex)
          {
            switch (ex)
            {
              case FormatException _:
              case OverflowException _:
                throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort, ex);
              default:
                throw;
            }
          }
          if (!ExceptionHelper.ValidateTcpPort(port))
            throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort);
        }
        string username = (string) null;
        string password = (string) null;
        if (strArray.Length >= 3)
          username = strArray[2];
        if (strArray.Length >= 4)
          password = strArray[3];
        return ProxyHelper.CreateProxyClient(proxyType, host, port, username, password);
    }
  }

  public static ProxyClient Parse(string protoProxyAddress)
  {
    string[] strArray = protoProxyAddress.Split(new string[1]
    {
      "://"
    }, StringSplitOptions.RemoveEmptyEntries);
    if (strArray.Length < 2)
      return (ProxyClient) null;
    string key = strArray[0];
    return !ProxyClient.ProxyProtocol.ContainsKey(key) ? (ProxyClient) null : ProxyClient.Parse(ProxyClient.ProxyProtocol[key], strArray[1]);
  }

  public static bool TryParse(ProxyType proxyType, string proxyAddress, out ProxyClient result)
  {
    result = (ProxyClient) null;
    if (string.IsNullOrEmpty(proxyAddress))
      return false;
    string[] strArray = proxyAddress.Split(':');
    int result1 = 0;
    string host = strArray[0];
    if (strArray.Length >= 2 && (!int.TryParse(strArray[1], out result1) || !ExceptionHelper.ValidateTcpPort(result1)))
      return false;
    string username = (string) null;
    string password = (string) null;
    if (strArray.Length >= 3)
      username = strArray[2];
    if (strArray.Length >= 4)
      password = strArray[3];
    try
    {
      result = ProxyHelper.CreateProxyClient(proxyType, host, result1, username, password);
    }
    catch (InvalidOperationException ex)
    {
      return false;
    }
    return true;
  }

  public static bool TryParse(string protoProxyAddress, out ProxyClient result)
  {
    string[] strArray = protoProxyAddress.Split(new string[1]
    {
      "://"
    }, StringSplitOptions.RemoveEmptyEntries);
    if (strArray.Length >= 2 && ProxyClient.ProxyProtocol.ContainsKey(strArray[0]))
      return ProxyClient.TryParse(ProxyClient.ProxyProtocol[strArray[0]], strArray[1], out result);
    result = (ProxyClient) null;
    return false;
  }

  public abstract TcpClient CreateConnection(
    string destinationHost,
    int destinationPort,
    TcpClient tcpClient = null);

  public override string ToString() => $"{this._host}:{this._port}";

  public virtual string ToExtendedString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("{0}:{1}", (object) this._host, (object) this._port);
    if (string.IsNullOrEmpty(this._username))
      return stringBuilder.ToString();
    stringBuilder.AppendFormat(":{0}", (object) this._username);
    if (!string.IsNullOrEmpty(this._password))
      stringBuilder.AppendFormat(":{0}", (object) this._password);
    return stringBuilder.ToString();
  }

  public override int GetHashCode()
  {
    return string.IsNullOrEmpty(this._host) ? 0 : this._host.GetHashCode() ^ this._port;
  }

  public bool Equals(ProxyClient proxy)
  {
    return proxy != null && this._host != null && this._host.Equals(proxy._host, StringComparison.OrdinalIgnoreCase) && this._port == proxy._port;
  }

  public override bool Equals(object obj) => obj is ProxyClient proxy && this.Equals(proxy);

  protected TcpClient CreateConnectionToProxy()
  {
    TcpClient tcpClient = new TcpClient();
    Exception connectException = (Exception) null;
    ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
    try
    {
      tcpClient.BeginConnect(this._host, this._port, (AsyncCallback) (ar =>
      {
        if (tcpClient.Client == null)
          return;
        try
        {
          tcpClient.EndConnect(ar);
        }
        catch (Exception ex)
        {
          connectException = ex;
        }
        connectDoneEvent.Set();
      }), (object) tcpClient);
    }
    catch (Exception ex)
    {
      tcpClient.Close();
      switch (ex)
      {
        case SocketException _:
        case SecurityException _:
          throw this.NewProxyException(Resources.ProxyException_FailedConnect, ex);
        default:
          throw;
      }
    }
    if (!connectDoneEvent.Wait(this._connectTimeout))
    {
      tcpClient.Close();
      throw this.NewProxyException(Resources.ProxyException_ConnectTimeout);
    }
    if (connectException != null)
    {
      tcpClient.Close();
      if (connectException is SocketException)
        throw this.NewProxyException(Resources.ProxyException_FailedConnect, connectException);
      throw connectException;
    }
    if (!tcpClient.Connected)
    {
      tcpClient.Close();
      throw this.NewProxyException(Resources.ProxyException_FailedConnect);
    }
    tcpClient.SendTimeout = this._readWriteTimeout;
    tcpClient.ReceiveTimeout = this._readWriteTimeout;
    return tcpClient;
  }

  protected void CheckState()
  {
    if (string.IsNullOrEmpty(this._host))
      throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongHost);
    if (!ExceptionHelper.ValidateTcpPort(this._port))
      throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPort);
    if (this._username != null && this._username.Length > (int) byte.MaxValue)
      throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongUsername);
    if (this._password != null && this._password.Length > (int) byte.MaxValue)
      throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPassword);
  }

  protected ProxyException NewProxyException(string message, Exception innerException = null)
  {
    return new ProxyException(string.Format(message, (object) this.ToString()), this, innerException);
  }
}
