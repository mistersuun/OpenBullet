// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxDriverCommandExecutor
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxDriverCommandExecutor : ICommandExecutor, IDisposable
{
  private FirefoxDriverServer server;
  private HttpCommandExecutor internalExecutor;
  private TimeSpan commandTimeout;
  private bool isDisposed;

  public FirefoxDriverCommandExecutor(
    FirefoxBinary binary,
    FirefoxProfile profile,
    string host,
    TimeSpan commandTimeout)
  {
    this.server = new FirefoxDriverServer(binary, profile, host);
    this.commandTimeout = commandTimeout;
  }

  public CommandInfoRepository CommandInfoRepository => this.internalExecutor.CommandInfoRepository;

  public Response Execute(Command commandToExecute)
  {
    if (commandToExecute == null)
      throw new ArgumentNullException(nameof (commandToExecute), "Command may not be null");
    if (commandToExecute.Name == DriverCommand.NewSession)
    {
      this.server.Start();
      this.internalExecutor = new HttpCommandExecutor(this.server.ExtensionUri, this.commandTimeout);
    }
    try
    {
      return this.internalExecutor.Execute(commandToExecute);
    }
    finally
    {
      if (commandToExecute.Name == DriverCommand.Quit)
        this.Dispose();
    }
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.isDisposed)
      return;
    if (disposing)
      this.server.Dispose();
    this.isDisposed = true;
  }
}
