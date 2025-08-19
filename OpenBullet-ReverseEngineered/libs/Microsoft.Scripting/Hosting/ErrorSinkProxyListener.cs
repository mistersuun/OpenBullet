// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ErrorSinkProxyListener
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.IO;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class ErrorSinkProxyListener : ErrorListener
{
  private ErrorSink _errorSink;

  public ErrorSinkProxyListener(ErrorSink errorSink) => this._errorSink = errorSink;

  public override void ErrorReported(
    ScriptSource source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    string code = (string) null;
    string line = (string) null;
    try
    {
      code = source.GetCode();
      line = source.GetCodeLine(span.Start.Line);
    }
    catch (IOException ex)
    {
    }
    this._errorSink.Add(message, source.Path, code, line, span, errorCode, severity);
  }
}
