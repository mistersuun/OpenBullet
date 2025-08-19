// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ErrorCounter
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting;

public class ErrorCounter : ErrorSink
{
  private readonly ErrorSink _sink;
  private int _fatalErrorCount;
  private int _errorCount;
  private int _warningCount;

  public int FatalErrorCount => this._fatalErrorCount;

  public int ErrorCount => this._errorCount;

  public int WarningCount => this._warningCount;

  public bool AnyError => this._errorCount > 0 || this._fatalErrorCount > 0;

  public ErrorCounter()
    : this(ErrorSink.Null)
  {
  }

  public ErrorCounter(ErrorSink sink)
  {
    ContractUtils.RequiresNotNull((object) sink, nameof (sink));
    this._sink = sink;
  }

  protected virtual void CountError(Severity severity)
  {
    switch (severity)
    {
      case Severity.Warning:
        Interlocked.Increment(ref this._warningCount);
        break;
      case Severity.Error:
        Interlocked.Increment(ref this._errorCount);
        break;
      case Severity.FatalError:
        Interlocked.Increment(ref this._fatalErrorCount);
        break;
    }
  }

  public void ClearCounters() => this._warningCount = this._errorCount = this._fatalErrorCount = 0;

  public override void Add(
    SourceUnit source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    this.CountError(severity);
    this._sink.Add(source, message, span, errorCode, severity);
  }
}
