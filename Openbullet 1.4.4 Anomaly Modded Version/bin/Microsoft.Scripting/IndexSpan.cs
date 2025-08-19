// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.IndexSpan
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting;

public readonly struct IndexSpan : IEquatable<IndexSpan>
{
  public IndexSpan(int start, int length)
  {
    ContractUtils.Requires(length >= 0, nameof (length));
    ContractUtils.Requires(start >= 0, nameof (start));
    this.Start = start;
    this.Length = length;
  }

  public int Start { get; }

  public int End => this.Start + this.Length;

  public int Length { get; }

  public bool IsEmpty => this.Length == 0;

  public override int GetHashCode()
  {
    int num = this.Length;
    int hashCode1 = num.GetHashCode();
    num = this.Start;
    int hashCode2 = num.GetHashCode();
    return hashCode1 ^ hashCode2;
  }

  public override bool Equals(object obj) => obj is IndexSpan other && this.Equals(other);

  public static bool operator ==(IndexSpan self, IndexSpan other) => self.Equals(other);

  public static bool operator !=(IndexSpan self, IndexSpan other) => !self.Equals(other);

  public bool Equals(IndexSpan other) => this.Length == other.Length && this.Start == other.Start;
}
