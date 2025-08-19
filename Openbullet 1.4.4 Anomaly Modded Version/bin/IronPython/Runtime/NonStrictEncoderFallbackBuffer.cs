// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NonStrictEncoderFallbackBuffer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class NonStrictEncoderFallbackBuffer : EncoderFallbackBuffer
{
  private List<char> _buffer = new List<char>();
  private int _index;

  public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
  {
    throw PythonOps.UnicodeEncodeError("'ascii' codec can't encode character '\\u{0:X}{1:04X}' in position {1}: ordinal not in range(128)", (object) (int) charUnknownHigh, (object) (int) charUnknownLow, (object) index);
  }

  public override bool Fallback(char charUnknown, int index)
  {
    if (charUnknown > 'ÿ')
      throw PythonOps.UnicodeEncodeError("'ascii' codec can't encode character '\\u{0:X}' in position {1}: ordinal not in range(128)", (object) (int) charUnknown, (object) index);
    this._buffer.Add(charUnknown);
    return true;
  }

  public override char GetNextChar() => this._buffer[this._index++];

  public override bool MovePrevious()
  {
    if (this._index <= 0)
      return false;
    --this._index;
    return true;
  }

  public override int Remaining => this._buffer.Count - this._index;
}
