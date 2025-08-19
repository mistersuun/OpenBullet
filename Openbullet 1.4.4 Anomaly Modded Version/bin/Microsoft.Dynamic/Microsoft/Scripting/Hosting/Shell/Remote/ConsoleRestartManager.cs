// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.ConsoleRestartManager
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

public abstract class ConsoleRestartManager
{
  private RemoteConsoleHost _remoteConsoleHost;
  private Thread _consoleThread;
  private bool _exitOnNormalExit;
  private bool _terminating;
  private object _accessLock = new object();

  public ConsoleRestartManager(bool exitOnNormalExit)
  {
    this._exitOnNormalExit = exitOnNormalExit;
    this._consoleThread = new Thread(new ThreadStart(this.Run));
    this._consoleThread.Name = "Console thread";
  }

  protected object AccessLock => this._accessLock;

  public Thread ConsoleThread => this._consoleThread;

  protected RemoteConsoleHost CurrentConsoleHost => this._remoteConsoleHost;

  public abstract RemoteConsoleHost CreateRemoteConsoleHost();

  public void Start()
  {
    if (this._consoleThread.IsAlive)
      throw new InvalidOperationException("Console thread is already running.");
    this._consoleThread.Start();
  }

  private void Run() => this.RunWorker();

  private void RunWorker()
  {
    while (true)
    {
      RemoteConsoleHost remoteConsoleHost = this.CreateRemoteConsoleHost();
      lock (this._accessLock)
      {
        if (this._terminating)
          break;
        this._remoteConsoleHost = remoteConsoleHost;
      }
      try
      {
        int num = remoteConsoleHost.Run(new string[0]);
        if (this._exitOnNormalExit)
        {
          if (num == 0)
            break;
        }
      }
      catch (RemoteRuntimeStartupException ex)
      {
      }
      finally
      {
        lock (this._accessLock)
        {
          remoteConsoleHost.Dispose();
          this._remoteConsoleHost = (RemoteConsoleHost) null;
        }
      }
    }
  }

  public IList<string> GetMemberNames(string expression)
  {
    lock (this._accessLock)
    {
      if (this._remoteConsoleHost == null)
        return (IList<string>) null;
      ScriptEngine engine = this._remoteConsoleHost.Engine;
      try
      {
        ScriptScope scriptScope = this._remoteConsoleHost.ScriptScope;
        return engine.CreateOperations(scriptScope).GetMemberNames(engine.CreateScriptSourceFromString(expression, SourceCodeKind.Expression).ExecuteAndWrap(scriptScope));
      }
      catch
      {
        return (IList<string>) null;
      }
    }
  }

  public void BreakExecution()
  {
    lock (this._accessLock)
    {
      if (this._remoteConsoleHost == null)
        return;
      try
      {
        this._remoteConsoleHost.AbortCommand();
      }
      catch (RemotingException ex)
      {
      }
    }
  }

  public void RestartConsole()
  {
    lock (this._accessLock)
    {
      if (this._remoteConsoleHost == null)
        return;
      this._remoteConsoleHost.Terminate(0);
    }
  }

  public void Terminate()
  {
    lock (this._accessLock)
    {
      this._terminating = true;
      this._remoteConsoleHost.Terminate(0);
    }
    this._consoleThread.Join();
  }
}
