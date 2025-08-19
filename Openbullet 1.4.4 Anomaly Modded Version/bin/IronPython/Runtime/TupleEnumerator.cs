// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.TupleEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("tupleiterator")]
public sealed class TupleEnumerator : IEnumerable, IEnumerator, IEnumerator<object>, IDisposable
{
  private int _curIndex;
  private PythonTuple _tuple;

  public TupleEnumerator(PythonTuple t)
  {
    this._tuple = t;
    this._curIndex = -1;
  }

  public object Current => this._tuple._data[this._curIndex];

  public bool MoveNext()
  {
    if (this._curIndex + 1 >= this._tuple.Count)
      return false;
    ++this._curIndex;
    return true;
  }

  public void Reset() => this._curIndex = -1;

  public void Dispose() => GC.SuppressFinalize((object) this);

  public IEnumerator GetEnumerator() => (IEnumerator) this;
}
