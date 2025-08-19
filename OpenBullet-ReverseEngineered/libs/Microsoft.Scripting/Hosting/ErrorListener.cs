// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ErrorListener
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public abstract class ErrorListener : MarshalByRefObject
{
  internal void ReportError(
    ScriptSource source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
    this.ErrorReported(source, message, span, errorCode, severity);
  }

  public abstract void ErrorReported(
    ScriptSource source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity);

  public override object InitializeLifetimeService() => (object) null;
}
