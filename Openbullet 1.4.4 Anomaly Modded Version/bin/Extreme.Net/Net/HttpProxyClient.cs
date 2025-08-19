// Decompiled with JetBrains decompiler
// Type: Extreme.Net.HttpProxyClient
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#nullable disable
namespace Extreme.Net;

public class HttpProxyClient : ProxyClient
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
    : base(ProxyType.Http, host, port, username, password)
  {
  }

  public static HttpProxyClient Parse(string proxyAddress)
  {
    return ProxyClient.Parse(ProxyType.Http, proxyAddress) as HttpProxyClient;
  }

  public static bool TryParse(string proxyAddress, out HttpProxyClient result)
  {
    ProxyClient result1;
    if (ProxyClient.TryParse(ProxyType.Http, proxyAddress, out result1))
    {
      result = result1 as HttpProxyClient;
      return true;
    }
    result = (HttpProxyClient) null;
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
        if (destinationPort != 80 /*0x50*/)
        {
          HttpStatusCode response;
          try
          {
            NetworkStream stream = connection.GetStream();
            this.SendConnectionCommand(stream, destinationHost, destinationPort);
            response = this.ReceiveResponse(stream);
          }
          catch (Exception ex)
          {
            connection.Close();
            if (ex is IOException || ex is SocketException)
              throw this.NewProxyException(Resources.ProxyException_Error, ex);
            throw;
          }
          if (response != HttpStatusCode.OK)
          {
            connection.Close();
            throw new ProxyException(string.Format(Resources.ProxyException_ReceivedWrongStatusCode, (object) response, (object) this.ToString()), (ProxyClient) this);
          }
        }
        return connection;
    }
  }

  private string GenerateAuthorizationHeader()
  {
    return !string.IsNullOrEmpty(this._username) || !string.IsNullOrEmpty(this._password) ? $"Proxy-Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._username}:{this._password}"))}\r\n" : string.Empty;
  }

  private void SendConnectionCommand(
    NetworkStream nStream,
    string destinationHost,
    int destinationPort)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", (object) destinationHost, (object) destinationPort);
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
    string str1 = stringBuilder.ToString();
    string str2 = str1.Length != 0 ? str1.Substring(" ", "\r\n") : throw this.NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);
    int length = str2.IndexOf(' ');
    string str3 = length != -1 ? str2.Substring(0, length) : throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
    return str3.Length != 0 ? (HttpStatusCode) Enum.Parse(typeof (HttpStatusCode), str3) : throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
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
