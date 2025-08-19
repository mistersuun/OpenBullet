// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.RemoteConsoleHost
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

public abstract class RemoteConsoleHost : ConsoleHost, IDisposable
{
  private Process _remoteRuntimeProcess;
  internal RemoteCommandDispatcher _remoteCommandDispatcher;
  private string _channelName = RemoteConsoleHost.GetChannelName();
  private IpcChannel _clientChannel;
  private AutoResetEvent _remoteOutputReceived = new AutoResetEvent(false);
  private ScriptScope _scriptScope;

  private static string GetChannelName() => "RemoteRuntime-" + Guid.NewGuid().ToString();

  private ProcessStartInfo GetProcessStartInfo()
  {
    ProcessStartInfo processInfo = new ProcessStartInfo();
    processInfo.Arguments = "-X:RemoteRuntimeChannel " + this._channelName;
    processInfo.CreateNoWindow = true;
    processInfo.UseShellExecute = false;
    processInfo.RedirectStandardError = true;
    processInfo.RedirectStandardOutput = true;
    processInfo.RedirectStandardInput = true;
    this.CustomizeRemoteRuntimeStartInfo(processInfo);
    return processInfo;
  }

  private void StartRemoteRuntimeProcess()
  {
    Process process = new Process();
    process.StartInfo = this.GetProcessStartInfo();
    process.OutputDataReceived += new DataReceivedEventHandler(this.OnOutputDataReceived);
    process.ErrorDataReceived += new DataReceivedEventHandler(this.OnErrorDataReceived);
    process.Exited += new EventHandler(this.OnRemoteRuntimeExited);
    this._remoteRuntimeProcess = process;
    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    process.EnableRaisingEvents = true;
    this._remoteOutputReceived.WaitOne();
    if (process.HasExited)
      throw new RemoteRuntimeStartupException("Remote runtime terminated during startup with exitcode " + (object) process.ExitCode);
  }

  private T GetRemoteObject<T>(string uri)
  {
    return (T) Activator.GetObject(typeof (T), $"ipc://{this._channelName}/{uri}");
  }

  private void InitializeRemoteScriptEngine()
  {
    this.StartRemoteRuntimeProcess();
    this._remoteCommandDispatcher = this.GetRemoteObject<RemoteCommandDispatcher>("CommandDispatcherUri");
    this._scriptScope = this._remoteCommandDispatcher.ScriptScope;
    this.Engine = this._scriptScope.Engine;
    string str = this._channelName.Replace("RemoteRuntime", "RemoteConsole");
    this._clientChannel = RemoteRuntimeServer.CreateChannel(str, str);
    ChannelServices.RegisterChannel((IChannel) this._clientChannel, false);
  }

  protected virtual void OnRemoteRuntimeExited(object sender, EventArgs args)
  {
    EventHandler remoteRuntimeExited = this.RemoteRuntimeExited;
    if (remoteRuntimeExited != null)
      remoteRuntimeExited(sender, args);
    this._remoteOutputReceived.Set();
    this.Terminate(this._remoteRuntimeProcess.ExitCode);
  }

  protected virtual void OnOutputDataReceived(object sender, DataReceivedEventArgs eventArgs)
  {
    if (string.IsNullOrEmpty(eventArgs.Data))
      return;
    string data = eventArgs.Data;
    if (data.Contains("{7FF032BB-DB03-4255-89DE-641CA195E5FA}"))
      this._remoteOutputReceived.Set();
    else
      this.ConsoleIO.WriteLine(data, Style.Out);
  }

  private void OnErrorDataReceived(object sender, DataReceivedEventArgs eventArgs)
  {
    if (string.IsNullOrEmpty(eventArgs.Data))
      return;
    this.ConsoleIO.WriteLine(eventArgs.Data, Style.Error);
  }

  public override void Terminate(int exitCode)
  {
    if (this.CommandLine == null)
      return;
    base.Terminate(exitCode);
  }

  protected override CommandLine CreateCommandLine()
  {
    return (CommandLine) new RemoteConsoleCommandLine(this._scriptScope, this._remoteCommandDispatcher, this._remoteOutputReceived);
  }

  public ScriptScope ScriptScope => this.CommandLine.ScriptScope;

  public Process RemoteRuntimeProcess => this._remoteRuntimeProcess;

  protected override void UnhandledException(ScriptEngine engine, Exception e)
  {
    ((RemoteConsoleCommandLine) this.CommandLine).UnhandledExceptionWorker(e);
  }

  internal event EventHandler RemoteRuntimeExited;

  public abstract void CustomizeRemoteRuntimeStartInfo(ProcessStartInfo processInfo);

  public bool AbortCommand() => this._remoteCommandDispatcher.AbortCommand();

  public override int Run(string[] args)
  {
    this.ConsoleHostOptionsParser = new ConsoleHostOptionsParser(new ConsoleHostOptions(), this.CreateRuntimeSetup());
    try
    {
      this.ParseHostOptions(args);
    }
    catch (InvalidOptionException ex)
    {
      Console.Error.WriteLine("Invalid argument: " + ex.Message);
      return this.ExitCode = 1;
    }
    this.ConsoleIO = this.CreateConsole((ScriptEngine) null, (CommandLine) null, new ConsoleOptions());
    this.InitializeRemoteScriptEngine();
    this.Runtime = this.Engine.Runtime;
    this.ExecuteInternal();
    return this.ExitCode;
  }

  public virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this._remoteOutputReceived.Close();
    if (this._clientChannel != null)
    {
      ChannelServices.UnregisterChannel((IChannel) this._clientChannel);
      this._clientChannel = (IpcChannel) null;
    }
    if (this._remoteRuntimeProcess == null)
      return;
    this._remoteRuntimeProcess.Exited -= new EventHandler(this.OnRemoteRuntimeExited);
    this._remoteRuntimeProcess.StandardInput.Close();
    this._remoteRuntimeProcess.WaitForExit(5000);
    if (!this._remoteRuntimeProcess.HasExited)
    {
      this._remoteRuntimeProcess.Kill();
      this._remoteRuntimeProcess.WaitForExit();
    }
    this._remoteRuntimeProcess = (Process) null;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
