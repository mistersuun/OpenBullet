// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ThrowingErrorSink
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting;

#nullable disable
namespace IronPython.Runtime;

internal class ThrowingErrorSink : ErrorSink
{
  public static readonly ThrowingErrorSink Default = new ThrowingErrorSink();

  private ThrowingErrorSink()
  {
  }

  public override void Add(
    SourceUnit sourceUnit,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    if (severity != Severity.Warning)
      throw PythonOps.SyntaxError(message, sourceUnit, span, errorCode);
    PythonOps.SyntaxWarning(message, sourceUnit, span, errorCode);
  }
}
