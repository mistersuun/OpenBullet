// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.StringBufferProtocol
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable disable
namespace IronPython.Runtime.Operations;

[PythonHidden(new PlatformID[] {})]
public class StringBufferProtocol : IBufferProtocol
{
  private string _wrapped;

  public StringBufferProtocol(string wrapped)
  {
    this._wrapped = !wrapped.Any<char>((Func<char, bool>) (x => x > 'ÿ')) ? wrapped : throw PythonOps.TypeError("cannot make memory view because object does not have the buffer interface");
  }

  int IBufferProtocol.ItemCount => this._wrapped.Length;

  string IBufferProtocol.Format => "B";

  BigInteger IBufferProtocol.ItemSize => (BigInteger) 1;

  BigInteger IBufferProtocol.NumberDimensions => (BigInteger) 1;

  bool IBufferProtocol.ReadOnly => true;

  PythonTuple IBufferProtocol.Strides => PythonTuple.MakeTuple((object) 1);

  object IBufferProtocol.SubOffsets => (object) null;

  Bytes IBufferProtocol.GetItem(int index)
  {
    lock (this)
      return Bytes.Make(new byte[1]
      {
        (byte) this._wrapped[PythonOps.FixIndex(index, this._wrapped.Length)]
      });
  }

  IList<BigInteger> IBufferProtocol.GetShape(int start, int? end)
  {
    return end.HasValue ? (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) end.Value - (BigInteger) start
    } : (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) this._wrapped.Length - (BigInteger) start
    };
  }

  void IBufferProtocol.SetItem(int index, object value)
  {
    throw PythonOps.TypeError("cannot modify read-only memory");
  }

  void IBufferProtocol.SetSlice(Slice index, object value)
  {
    throw PythonOps.TypeError("cannot modify read-only memory");
  }

  Bytes IBufferProtocol.ToBytes(int start, int? end)
  {
    return start == 0 && !end.HasValue ? new Bytes((IList<byte>) this._wrapped.MakeByteArray()) : new Bytes((IList<byte>) StringOps.GetItem(this._wrapped, new Slice((object) start, (object) end)).MakeByteArray());
  }

  List IBufferProtocol.ToList(int start, int? end)
  {
    string s = StringOps.GetItem(this._wrapped, new Slice((object) start, (object) end));
    return string.IsNullOrEmpty(s) ? new List() : new List((ICollection) s.MakeByteArray());
  }
}
