// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.Internal.SocketLock
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Firefox.Internal;

internal class SocketLock : ILock, IDisposable
{
  private static int delayBetweenSocketChecks = 100;
  private int lockPort;
  private Socket lockSocket;

  public SocketLock(int lockPort)
  {
    this.lockPort = lockPort;
    this.lockSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    this.PreventSocketInheritance();
  }

  public void LockObject(TimeSpan timeout)
  {
    IPHostEntry hostEntry = Dns.GetHostEntry("localhost");
    IPAddress address1 = IPAddress.Parse("127.0.0.1");
    foreach (IPAddress address2 in hostEntry.AddressList)
    {
      if (address2.AddressFamily == AddressFamily.InterNetwork)
      {
        address1 = address2;
        break;
      }
    }
    IPEndPoint address3 = new IPEndPoint(address1, this.lockPort);
    DateTime dateTime = DateTime.Now.Add(timeout);
    do
    {
      try
      {
        if (this.IsLockFree(address3))
          return;
        Thread.Sleep(SocketLock.delayBetweenSocketChecks);
      }
      catch (ThreadInterruptedException ex)
      {
        throw new WebDriverException("the thread was interrupted", (Exception) ex);
      }
      catch (IOException ex)
      {
        throw new WebDriverException("An unexpected error occurred", (Exception) ex);
      }
    }
    while (DateTime.Now < dateTime);
    throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to bind to locking port {0} within {1} milliseconds", (object) this.lockPort, (object) timeout.TotalMilliseconds));
  }

  public void UnlockObject()
  {
    try
    {
      this.lockSocket.Close();
    }
    catch (IOException ex)
    {
      throw new WebDriverException("An error occured unlocking the object", (Exception) ex);
    }
  }

  public void Dispose()
  {
    if (this.lockSocket != null && this.lockSocket.Connected)
      this.lockSocket.Close();
    GC.SuppressFinalize((object) this);
  }

  private bool IsLockFree(IPEndPoint address)
  {
    try
    {
      this.lockSocket.Bind((EndPoint) address);
      return true;
    }
    catch (SocketException ex)
    {
      return false;
    }
  }

  private void PreventSocketInheritance()
  {
    if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
      return;
    NativeMethods.SetHandleInformation(this.lockSocket.Handle, NativeMethods.HandleInformation.Inherit | NativeMethods.HandleInformation.ProtectFromClose, NativeMethods.HandleInformation.None);
  }
}
