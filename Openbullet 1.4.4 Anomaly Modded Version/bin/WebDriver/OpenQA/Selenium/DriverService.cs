// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.DriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Permissions;

#nullable disable
namespace OpenQA.Selenium;

public abstract class DriverService : ICommandServer, IDisposable
{
  private string driverServicePath;
  private string driverServiceExecutableName;
  private string driverServiceHostName = "localhost";
  private int driverServicePort;
  private bool silent;
  private bool hideCommandPromptWindow;
  private bool isDisposed;
  private Process driverServiceProcess;

  protected DriverService(
    string servicePath,
    int port,
    string driverServiceExecutableName,
    Uri driverServiceDownloadUrl)
  {
    string path = !string.IsNullOrEmpty(servicePath) ? Path.Combine(servicePath, driverServiceExecutableName) : throw new ArgumentException("Path to locate driver executable cannot be null or empty.", nameof (servicePath));
    if (!System.IO.File.Exists(path))
      throw new DriverServiceNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The file {0} does not exist. The driver can be downloaded at {1}", (object) path, (object) driverServiceDownloadUrl));
    this.driverServicePath = servicePath;
    this.driverServiceExecutableName = driverServiceExecutableName;
    this.driverServicePort = port;
  }

  public Uri ServiceUrl
  {
    get
    {
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://{0}:{1}", (object) this.driverServiceHostName, (object) this.driverServicePort));
    }
  }

  public string HostName
  {
    get => this.driverServiceHostName;
    set => this.driverServiceHostName = value;
  }

  public int Port
  {
    get => this.driverServicePort;
    set => this.driverServicePort = value;
  }

  public bool SuppressInitialDiagnosticInformation
  {
    get => this.silent;
    set => this.silent = value;
  }

  public bool IsRunning
  {
    [SecurityPermission(SecurityAction.Demand)] get
    {
      return this.driverServiceProcess != null && !this.driverServiceProcess.HasExited;
    }
  }

  public bool HideCommandPromptWindow
  {
    get => this.hideCommandPromptWindow;
    set => this.hideCommandPromptWindow = value;
  }

  public int ProcessId
  {
    get
    {
      if (this.IsRunning)
      {
        try
        {
          return this.driverServiceProcess.Id;
        }
        catch (InvalidOperationException ex)
        {
        }
      }
      return 0;
    }
  }

  protected string DriverServiceExecutableName => this.driverServiceExecutableName;

  protected virtual string CommandLineArguments
  {
    get
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "--port={0}", (object) this.driverServicePort);
    }
  }

  protected virtual TimeSpan InitializationTimeout => TimeSpan.FromSeconds(20.0);

  protected virtual TimeSpan TerminationTimeout => TimeSpan.FromSeconds(10.0);

  protected virtual bool HasShutdown => true;

  protected virtual bool IsInitialized
  {
    get
    {
      bool isInitialized = false;
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(this.ServiceUrl, new Uri(DriverCommand.Status, UriKind.Relative))) as HttpWebRequest;
        httpWebRequest.KeepAlive = false;
        httpWebRequest.Timeout = 5000;
        HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
        isInitialized = response.StatusCode == HttpStatusCode.OK && response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        response.Close();
      }
      catch (WebException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return isInitialized;
    }
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  [SecurityPermission(SecurityAction.Demand)]
  public void Start()
  {
    this.driverServiceProcess = new Process();
    this.driverServiceProcess.StartInfo.FileName = Path.Combine(this.driverServicePath, this.driverServiceExecutableName);
    this.driverServiceProcess.StartInfo.Arguments = this.CommandLineArguments;
    this.driverServiceProcess.StartInfo.UseShellExecute = false;
    this.driverServiceProcess.StartInfo.CreateNoWindow = this.hideCommandPromptWindow;
    this.driverServiceProcess.Start();
    if (!this.WaitForServiceInitialization())
      throw new WebDriverException("Cannot start the driver service on " + (object) this.ServiceUrl);
  }

  protected static string FindDriverServiceExecutable(string executableName, Uri downloadUrl)
  {
    string file = FileUtilities.FindFile(executableName);
    return !string.IsNullOrEmpty(file) ? file : throw new DriverServiceNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The {0} file does not exist in the current directory or in a directory on the PATH environment variable. The driver can be downloaded at {1}.", (object) executableName, (object) downloadUrl));
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.isDisposed)
      return;
    if (disposing)
      this.Stop();
    this.isDisposed = true;
  }

  [SecurityPermission(SecurityAction.Demand)]
  private void Stop()
  {
    if (!this.IsRunning)
      return;
    if (this.HasShutdown)
    {
      Uri requestUri = new Uri(this.ServiceUrl, "/shutdown");
      DateTime dateTime = DateTime.Now.Add(this.TerminationTimeout);
      while (this.IsRunning)
      {
        if (DateTime.Now < dateTime)
        {
          try
          {
            HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
            httpWebRequest.KeepAlive = false;
            (httpWebRequest.GetResponse() as HttpWebResponse).Close();
            this.driverServiceProcess.WaitForExit(3000);
          }
          catch (WebException ex)
          {
          }
        }
        else
          break;
      }
    }
    if (this.IsRunning)
    {
      this.driverServiceProcess.WaitForExit(Convert.ToInt32(this.TerminationTimeout.TotalMilliseconds));
      if (!this.driverServiceProcess.HasExited)
        this.driverServiceProcess.Kill();
    }
    this.driverServiceProcess.Dispose();
    this.driverServiceProcess = (Process) null;
  }

  private bool WaitForServiceInitialization()
  {
    bool flag = false;
    DateTime dateTime = DateTime.Now.Add(this.InitializationTimeout);
    while (!flag && DateTime.Now < dateTime && this.IsRunning)
      flag = this.IsInitialized;
    return flag;
  }
}
