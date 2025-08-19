// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ParserSink
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class ParserSink
{
  public static readonly ParserSink Null = new ParserSink();

  public virtual void MatchPair(SourceSpan opening, SourceSpan closing, int priority)
  {
  }

  public virtual void MatchTriple(
    SourceSpan opening,
    SourceSpan middle,
    SourceSpan closing,
    int priority)
  {
  }

  public virtual void EndParameters(SourceSpan span)
  {
  }

  public virtual void NextParameter(SourceSpan span)
  {
  }

  public virtual void QualifyName(SourceSpan selector, SourceSpan span, string name)
  {
  }

  public virtual void StartName(SourceSpan span, string name)
  {
  }

  public virtual void StartParameters(SourceSpan context)
  {
  }
}
