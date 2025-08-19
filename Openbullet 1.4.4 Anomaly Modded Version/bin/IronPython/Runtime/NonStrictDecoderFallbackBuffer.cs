// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NonStrictDecoderFallbackBuffer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class NonStrictDecoderFallbackBuffer : DecoderFallbackBuffer
{
  private List<byte> _bytes = new List<byte>();
  private int _index;

  public override bool Fallback(byte[] bytesUnknown, int index)
  {
    this._bytes.AddRange((IEnumerable<byte>) bytesUnknown);
    return true;
  }

  public override char GetNextChar()
  {
    return this._index == this._bytes.Count ? char.MinValue : (char) this._bytes[this._index++];
  }

  public override bool MovePrevious()
  {
    if (this._index <= 0)
      return false;
    --this._index;
    return true;
  }

  public override int Remaining => this._bytes.Count - this._index;
}
