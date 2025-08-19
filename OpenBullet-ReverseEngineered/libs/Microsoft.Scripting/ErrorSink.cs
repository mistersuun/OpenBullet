// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ErrorSink
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting;

public class ErrorSink
{
  public static readonly ErrorSink Default = new ErrorSink();
  public static readonly ErrorSink Null = (ErrorSink) new NullErrorSink();

  protected ErrorSink()
  {
  }

  public virtual void Add(
    SourceUnit source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    if (severity == Severity.FatalError || severity == Severity.Error)
      throw new SyntaxErrorException(message, source, span, errorCode, severity);
  }

  public virtual void Add(
    string message,
    string path,
    string code,
    string line,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    if (severity == Severity.FatalError || severity == Severity.Error)
      throw new SyntaxErrorException(message, path, code, line, span, errorCode, severity);
  }
}
