// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.NullErrorSink
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting;

internal sealed class NullErrorSink : ErrorSink
{
  public override void Add(
    SourceUnit source,
    string message,
    SourceSpan span,
    int errorCode,
    Severity severity)
  {
  }
}
