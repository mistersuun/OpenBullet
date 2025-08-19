// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.IListOfBytesBufferProtocol
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace IronPython.Runtime.Operations;

[PythonHidden(new PlatformID[] {})]
internal class IListOfBytesBufferProtocol : IBufferProtocol
{
  private static readonly PythonTuple strides = PythonTuple.MakeTuple((object) 1);
  private IList<byte> _wrapped;

  public IListOfBytesBufferProtocol(IList<byte> wrapped) => this._wrapped = wrapped;

  int IBufferProtocol.ItemCount => this._wrapped.Count;

  string IBufferProtocol.Format => "B";

  BigInteger IBufferProtocol.ItemSize => (BigInteger) 1;

  BigInteger IBufferProtocol.NumberDimensions => (BigInteger) 1;

  bool IBufferProtocol.ReadOnly => true;

  PythonTuple IBufferProtocol.Strides => IListOfBytesBufferProtocol.strides;

  object IBufferProtocol.SubOffsets => (object) null;

  Bytes IBufferProtocol.GetItem(int index)
  {
    lock (this)
      return Bytes.Make(new byte[1]
      {
        this._wrapped[PythonOps.FixIndex(index, this._wrapped.Count)]
      });
  }

  IList<BigInteger> IBufferProtocol.GetShape(int start, int? end)
  {
    return end.HasValue ? (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) end.Value - (BigInteger) start
    } : (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) this._wrapped.Count - (BigInteger) start
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
    return start == 0 && !end.HasValue ? new Bytes(this._wrapped) : new Bytes((IList<byte>) this._wrapped.Slice(new Slice((object) start, (object) end)));
  }

  List IBufferProtocol.ToList(int start, int? end)
  {
    return new List((ICollection) this._wrapped.Slice(new Slice((object) start, (object) end)));
  }
}
