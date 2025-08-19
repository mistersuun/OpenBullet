// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.OffsetInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class OffsetInstruction : Instruction
{
  internal const int Unknown = -2147483648 /*0x80000000*/;
  internal const int CacheSize = 32 /*0x20*/;
  protected int _offset = int.MinValue;

  public int Offset => this._offset;

  public abstract Instruction[] Cache { get; }

  public Instruction Fixup(int offset)
  {
    this._offset = offset;
    Instruction[] cache = this.Cache;
    return cache != null && offset >= 0 && offset < cache.Length ? cache[offset] ?? (cache[offset] = (Instruction) this) : (Instruction) this;
  }

  public override string ToDebugString(
    int instructionIndex,
    object cookie,
    Func<int, int> labelIndexer,
    IList<object> objects)
  {
    return this.ToString() + (this._offset != int.MinValue ? " -> " + (instructionIndex + this._offset).ToString() : "");
  }

  public override string ToString()
  {
    return this.InstructionName + (this._offset == int.MinValue ? "(?)" : $"({(object) this._offset})");
  }
}
