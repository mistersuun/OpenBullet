// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.TokenInfo
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public struct TokenInfo(SourceSpan span, TokenCategory category, TokenTriggers trigger) : 
  IEquatable<TokenInfo>
{
  public TokenCategory Category { get; set; } = category;

  public TokenTriggers Trigger { get; set; } = trigger;

  public SourceSpan SourceSpan { get; set; } = span;

  public bool Equals(TokenInfo other)
  {
    return this.Category == other.Category && this.Trigger == other.Trigger && this.SourceSpan == other.SourceSpan;
  }

  public override string ToString()
  {
    return $"TokenInfo: {this.SourceSpan}, {this.Category}, {this.Trigger}";
  }
}
