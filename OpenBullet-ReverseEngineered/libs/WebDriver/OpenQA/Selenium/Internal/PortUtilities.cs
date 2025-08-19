// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.PortUtilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Net;
using System.Net.Sockets;

#nullable disable
namespace OpenQA.Selenium.Internal;

internal static class PortUtilities
{
  public static int FindFreePort()
  {
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    try
    {
      IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
      socket.Bind((EndPoint) localEP);
      return ((IPEndPoint) socket.LocalEndPoint).Port;
    }
    finally
    {
      socket.Close();
    }
  }
}
