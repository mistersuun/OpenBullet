// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.HttpProxyClient
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#nullable disable
namespace Leaf.xNet;

public sealed class HttpProxyClient : ProxyClient
{
  private const int BufferSize = 50;
  private const int DefaultPort = 8080;

  public HttpProxyClient()
    : this((string) null)
  {
  }

  public HttpProxyClient(string host)
    : this(host, 8080)
  {
  }

  public HttpProxyClient(string host, int port)
    : this(host, port, string.Empty, string.Empty)
  {
  }

  public HttpProxyClient(string host, int port, string username, string password)
    : base(ProxyType.HTTP, host, port, username, password)
  {
  }

  public static string ProtocolVersion { get; set; } = "1.1";

  public static HttpProxyClient Parse(string proxyAddress)
  {
    return ProxyClient.Parse(ProxyType.HTTP, proxyAddress) as HttpProxyClient;
  }

  public static bool TryParse(string proxyAddress, out HttpProxyClient result)
  {
    ProxyClient result1;
    if (!ProxyClient.TryParse(ProxyType.HTTP, proxyAddress, out result1))
    {
      result = (HttpProxyClient) null;
      return false;
    }
    result = result1 as HttpProxyClient;
    return true;
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
          throw ExceptionHelper.WrongTcpPort(nameof (destinationHost));
        TcpClient connection = tcpClient ?? this.CreateConnectionToProxy();
        if (destinationPort == 80 /*0x50*/)
          return connection;
        HttpStatusCode response;
        try
        {
          NetworkStream stream = connection.GetStream();
          this.SendConnectionCommand((Stream) stream, destinationHost, destinationPort);
          response = this.ReceiveResponse(stream);
        }
        catch (Exception ex)
        {
          connection.Close();
          switch (ex)
          {
            case IOException _:
            case SocketException _:
              throw this.NewProxyException(Resources.ProxyException_Error, ex);
            default:
              throw;
          }
        }
        if (response == HttpStatusCode.OK)
          return connection;
        connection.Close();
        throw new ProxyException(string.Format(Resources.ProxyException_ReceivedWrongStatusCode, (object) response, (object) this.ToString()), (ProxyClient) this);
    }
  }

  private string GenerateAuthorizationHeader()
  {
    return string.IsNullOrEmpty(this._username) && string.IsNullOrEmpty(this._password) ? string.Empty : $"Proxy-Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._username}:{this._password}"))}\r\n";
  }

  private void SendConnectionCommand(Stream nStream, string destinationHost, int destinationPort)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("CONNECT {0}:{1} HTTP/{2}\r\n", (object) destinationHost, (object) destinationPort, (object) HttpProxyClient.ProtocolVersion);
    stringBuilder.AppendFormat(this.GenerateAuthorizationHeader());
    stringBuilder.AppendLine();
    byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
    nStream.Write(bytes, 0, bytes.Length);
  }

  private HttpStatusCode ReceiveResponse(NetworkStream nStream)
  {
    byte[] numArray = new byte[50];
    StringBuilder stringBuilder = new StringBuilder();
    this.WaitData(nStream);
    do
    {
      int count = nStream.Read(numArray, 0, 50);
      stringBuilder.Append(Encoding.ASCII.GetString(numArray, 0, count));
    }
    while (nStream.DataAvailable);
    string self = stringBuilder.ToString();
    string str1 = self.Length != 0 ? self.Substring(" ", "\r\n") : throw this.NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);
    int length = str1 != null ? str1.IndexOf(' ') : throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
    string str2 = length != -1 ? str1.Substring(0, length) : throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
    if (str2.Length == 0)
      throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
    HttpStatusCode result;
    return !Enum.TryParse<HttpStatusCode>(str2, out result) ? HttpStatusCode.InvalidStatusCode : result;
  }

  private void WaitData(NetworkStream nStream)
  {
    int num1 = 0;
    int num2 = nStream.ReadTimeout < 10 ? 10 : nStream.ReadTimeout;
    while (!nStream.DataAvailable)
    {
      if (num1 >= num2)
        throw this.NewProxyException(Resources.ProxyException_WaitDataTimeout);
      num1 += 10;
      Thread.Sleep(10);
    }
  }
}
