// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.DriverServiceCommandExecutor
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class DriverServiceCommandExecutor : ICommandExecutor, IDisposable
{
  private DriverService service;
  private HttpCommandExecutor internalExecutor;
  private bool isDisposed;

  public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout)
    : this(driverService, commandTimeout, true)
  {
  }

  public DriverServiceCommandExecutor(
    DriverService driverService,
    TimeSpan commandTimeout,
    bool enableKeepAlive)
  {
    this.service = driverService;
    this.internalExecutor = new HttpCommandExecutor(driverService.ServiceUrl, commandTimeout, enableKeepAlive);
  }

  public CommandInfoRepository CommandInfoRepository => this.internalExecutor.CommandInfoRepository;

  public HttpCommandExecutor HttpExecutor => this.internalExecutor;

  public Response Execute(Command commandToExecute)
  {
    if (commandToExecute == null)
      throw new ArgumentNullException(nameof (commandToExecute), "Command to execute cannot be null");
    if (commandToExecute.Name == DriverCommand.NewSession)
      this.service.Start();
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

  public void Dispose() => this.Dispose(true);

  protected virtual void Dispose(bool disposing)
  {
    if (this.isDisposed)
      return;
    if (disposing)
      this.service.Dispose();
    this.isDisposed = true;
  }
}
