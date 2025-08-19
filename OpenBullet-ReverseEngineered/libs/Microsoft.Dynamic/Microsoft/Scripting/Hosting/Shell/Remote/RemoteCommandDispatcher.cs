// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.RemoteCommandDispatcher
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

public class RemoteCommandDispatcher : MarshalByRefObject, ICommandDispatcher
{
  internal const string OutputCompleteMarker = "{7FF032BB-DB03-4255-89DE-641CA195E5FA}";
  private Thread _executingThread;

  public RemoteCommandDispatcher(ScriptScope scope) => this.ScriptScope = scope;

  public ScriptScope ScriptScope { get; }

  public object Execute(CompiledCode compiledCode, ScriptScope scope)
  {
    this._executingThread = Thread.CurrentThread;
    try
    {
      object obj = compiledCode.Execute(scope);
      Console.WriteLine("{7FF032BB-DB03-4255-89DE-641CA195E5FA}");
      return obj;
    }
    catch (ThreadAbortException ex)
    {
      if (ex.ExceptionState is KeyboardInterruptException exceptionState)
      {
        Thread.ResetAbort();
        throw exceptionState;
      }
      throw;
    }
    finally
    {
      this._executingThread = (Thread) null;
    }
  }

  public bool AbortCommand()
  {
    Thread executingThread = this._executingThread;
    if (executingThread == null)
      return false;
    executingThread.Abort((object) new KeyboardInterruptException(""));
    return true;
  }

  public override object InitializeLifetimeService() => (object) null;
}
