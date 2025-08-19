// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ErrorListenerProxySink
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting.Hosting;

internal sealed class ErrorListenerProxySink : ErrorSink
{
  private readonly ErrorListener _listener;
  private readonly ScriptSource _source;

  public ErrorListenerProxySink(ScriptSource source, ErrorListener listener)
  {
    this._listener = listener;
    this._source = source;
  }

  public override void Add(
    SourceUnit sourceUnit,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    if (this._listener != null)
      this._listener.ErrorReported(sourceUnit == this._source.SourceUnit ? this._source : new ScriptSource(this._source.Engine.Runtime.GetEngine(sourceUnit.LanguageContext), sourceUnit), message, span, errorCode, severity);
    else if (severity == Severity.FatalError || severity == Severity.Error)
      throw new SyntaxErrorException(message, sourceUnit, span, errorCode, severity);
  }
}
