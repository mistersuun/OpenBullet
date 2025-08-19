// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.XRangeIterator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("rangeiterator")]
public sealed class XRangeIterator : IEnumerable, IEnumerator, IEnumerator<int>, IDisposable
{
  private XRange _xrange;
  private int _value;
  private int _position;

  public XRangeIterator(XRange xrange)
  {
    this._xrange = xrange;
    this._value = xrange.Start - xrange.Step;
  }

  public object Current => ScriptingRuntimeHelpers.Int32ToObject(this._value);

  public bool MoveNext()
  {
    if (this._position >= this._xrange.__len__())
      return false;
    ++this._position;
    this._value += this._xrange.Step;
    return true;
  }

  public void Reset()
  {
    this._value = this._xrange.Start - this._xrange.Step;
    this._position = 0;
  }

  int IEnumerator<int>.Current => this._value;

  public void Dispose()
  {
  }

  public IEnumerator GetEnumerator() => (IEnumerator) this;
}
