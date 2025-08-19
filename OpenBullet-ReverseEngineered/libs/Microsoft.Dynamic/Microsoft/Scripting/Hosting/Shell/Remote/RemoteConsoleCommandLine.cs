// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.RemoteConsoleCommandLine
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.Remoting;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

public class RemoteConsoleCommandLine : CommandLine
{
  private RemoteConsoleCommandLine.RemoteConsoleCommandDispatcher _remoteConsoleCommandDispatcher;

  public RemoteConsoleCommandLine(
    ScriptScope scope,
    RemoteCommandDispatcher remoteCommandDispatcher,
    AutoResetEvent remoteOutputReceived)
  {
    this._remoteConsoleCommandDispatcher = new RemoteConsoleCommandLine.RemoteConsoleCommandDispatcher(remoteCommandDispatcher, remoteOutputReceived);
    this.ScriptScope = scope;
  }

  protected override ICommandDispatcher CreateCommandDispatcher()
  {
    return (ICommandDispatcher) this._remoteConsoleCommandDispatcher;
  }

  internal void UnhandledExceptionWorker(Exception e)
  {
    try
    {
      base.UnhandledException(e);
    }
    catch (Exception ex)
    {
      if (!(ex is RemotingException))
        this.Console.WriteLine($"({ex.GetType()} thrown while trying to display unhandled exception)", Style.Error);
      this.Console.WriteLine(e.ToString(), Style.Error);
    }
  }

  protected override void UnhandledException(Exception e) => this.UnhandledExceptionWorker(e);

  private class RemoteConsoleCommandDispatcher : ICommandDispatcher
  {
    private RemoteCommandDispatcher _remoteCommandDispatcher;
    private AutoResetEvent _remoteOutputReceived;

    internal RemoteConsoleCommandDispatcher(
      RemoteCommandDispatcher remoteCommandDispatcher,
      AutoResetEvent remoteOutputReceived)
    {
      this._remoteCommandDispatcher = remoteCommandDispatcher;
      this._remoteOutputReceived = remoteOutputReceived;
    }

    public object Execute(CompiledCode compiledCode, ScriptScope scope)
    {
      object obj = this._remoteCommandDispatcher.Execute(compiledCode, scope);
      this._remoteOutputReceived.WaitOne();
      return obj;
    }
  }
}
