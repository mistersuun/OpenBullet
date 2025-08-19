// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxDriverServer
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Firefox.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxDriverServer : ICommandServer, IDisposable
{
  private string host;
  private List<IPEndPoint> addresses = new List<IPEndPoint>();
  private FirefoxProfile profile;
  private FirefoxBinary process;
  private Uri extensionUri;

  public FirefoxDriverServer(FirefoxBinary binary, FirefoxProfile profile, string host)
  {
    this.host = host;
    this.profile = profile != null ? profile : new FirefoxProfile();
    if (binary == null)
      this.process = new FirefoxBinary();
    else
      this.process = binary;
  }

  public Uri ExtensionUri => this.extensionUri;

  public void Start()
  {
    using (ILock @lock = (ILock) new SocketLock(this.profile.Port - 1))
    {
      @lock.LockObject(this.process.Timeout);
      try
      {
        int nextFreePort = FirefoxDriverServer.DetermineNextFreePort(this.host, this.profile.Port);
        this.profile.Port = nextFreePort;
        this.profile.WriteToDisk();
        this.process.StartProfile(this.profile, "-foreground");
        this.SetAddress(nextFreePort);
        this.extensionUri = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://{0}:{1}/hub/", (object) this.host, (object) nextFreePort));
        this.ConnectToBrowser(this.process.Timeout);
      }
      finally
      {
        @lock.UnlockObject();
      }
    }
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.process.Dispose();
    this.profile.Clean();
  }

  private static int DetermineNextFreePort(string host, int port)
  {
    int port1;
    for (port1 = port; port1 < port + 200; ++port1)
    {
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.ExclusiveAddressUse = true;
        IPHostEntry hostEntry = Dns.GetHostEntry(host);
        IPAddress address1 = IPAddress.Parse("127.0.0.1");
        foreach (IPAddress address2 in hostEntry.AddressList)
        {
          if (address2.AddressFamily == AddressFamily.InterNetwork)
          {
            address1 = address2;
            break;
          }
        }
        IPEndPoint localEP = new IPEndPoint(address1, port1);
        try
        {
          socket.Bind((EndPoint) localEP);
          return port1;
        }
        catch (SocketException ex)
        {
        }
      }
    }
    throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find free port in the range {0} to {1} ", (object) port, (object) port1));
  }

  private static List<IPEndPoint> ObtainLoopbackAddresses(int port)
  {
    List<IPEndPoint> loopbackAddresses = new List<IPEndPoint>();
    foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
    {
      foreach (IPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
      {
        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && IPAddress.IsLoopback(unicastAddress.Address))
          loopbackAddresses.Add(new IPEndPoint(unicastAddress.Address, port));
      }
    }
    return loopbackAddresses;
  }

  private static bool IsSocketConnected(Socket extensionSocket)
  {
    return extensionSocket != null && extensionSocket.Connected;
  }

  private void SetAddress(int port)
  {
    if (string.Compare("localhost", this.host, StringComparison.OrdinalIgnoreCase) == 0)
    {
      this.addresses = FirefoxDriverServer.ObtainLoopbackAddresses(port);
    }
    else
    {
      IPHostEntry hostEntry = Dns.GetHostEntry(this.host);
      IPAddress address1 = IPAddress.Parse("127.0.0.1");
      foreach (IPAddress address2 in hostEntry.AddressList)
      {
        if (address2.AddressFamily == AddressFamily.InterNetwork)
        {
          address1 = address2;
          break;
        }
      }
      this.addresses.Add(new IPEndPoint(address1, port));
    }
    if (this.addresses.Count == 0)
      throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find any IPv4 addresses for host '{0}'", (object) this.host));
  }

  private void ConnectToBrowser(TimeSpan timeToWait)
  {
    Socket extensionSocket = (Socket) null;
    DateTime dateTime = DateTime.Now.AddMilliseconds(timeToWait.TotalMilliseconds);
    try
    {
      while (!FirefoxDriverServer.IsSocketConnected(extensionSocket) && dateTime > DateTime.Now)
      {
        foreach (IPEndPoint address in this.addresses)
        {
          try
          {
            extensionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            extensionSocket.Connect((EndPoint) address);
            break;
          }
          catch (SocketException ex)
          {
            Thread.Sleep(250);
          }
        }
      }
      if (FirefoxDriverServer.IsSocketConnected(extensionSocket))
        return;
      if (extensionSocket == null || extensionSocket.RemoteEndPoint == null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (IPEndPoint address in this.addresses)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(", ");
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) address.Address.ToString(), (object) address.Port.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to start up socket within {0} milliseconds. Attempted to connect to the following addresses: {1}", (object) timeToWait.TotalMilliseconds, (object) stringBuilder.ToString()));
      }
      IPEndPoint remoteEndPoint = (IPEndPoint) extensionSocket.RemoteEndPoint;
      throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to connect to host {0} on port {1} after {2} milliseconds", (object) remoteEndPoint.Address.ToString(), (object) remoteEndPoint.Port.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) timeToWait.TotalMilliseconds));
    }
    finally
    {
      extensionSocket.Close();
    }
  }
}
