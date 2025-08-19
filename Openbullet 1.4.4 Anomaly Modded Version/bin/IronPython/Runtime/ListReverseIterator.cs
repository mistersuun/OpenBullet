// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ListReverseIterator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("listreverseiterator")]
public sealed class ListReverseIterator : 
  IEnumerator,
  IEnumerable,
  IEnumerable<object>,
  IEnumerator<object>,
  IDisposable
{
  private int _index;
  private readonly List _list;
  private bool _iterating;

  public ListReverseIterator(List l)
  {
    this._list = l;
    this.Reset();
  }

  public void Reset()
  {
    this._index = 0;
    this._iterating = true;
  }

  public object Current => this._list._data[this._list._size - this._index];

  public bool MoveNext()
  {
    if (this._iterating)
    {
      ++this._index;
      this._iterating = this._index <= this._list._size;
    }
    return this._iterating;
  }

  public IEnumerator GetEnumerator() => (IEnumerator) this;

  public void Dispose()
  {
  }

  IEnumerator<object> IEnumerable<object>.GetEnumerator() => (IEnumerator<object>) this;
}
