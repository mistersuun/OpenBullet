// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SetIterator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("setiterator")]
public sealed class SetIterator : 
  IEnumerable,
  IEnumerable<object>,
  IEnumerator,
  IEnumerator<object>,
  IDisposable
{
  private readonly SetStorage _items;
  private readonly int _version;
  private readonly int _maxIndex;
  private int _index = -2;

  internal SetIterator(SetStorage items, bool mutable)
  {
    this._items = items;
    if (mutable)
    {
      lock (items)
      {
        this._version = items.Version;
        this._maxIndex = items._count > 0 ? items._buckets.Length : 0;
      }
    }
    else
    {
      this._version = items.Version;
      this._maxIndex = items._count > 0 ? items._buckets.Length : 0;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public void Dispose()
  {
  }

  public object Current
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      if (this._index < 0)
        return (object) null;
      object current = this._items._buckets[this._index].Item;
      if (this._items.Version != this._version)
        throw PythonOps.RuntimeError("set changed during iteration");
      return current;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public bool MoveNext()
  {
    if (this._index == this._maxIndex)
      return false;
    ++this._index;
    if (this._index < 0)
    {
      if (this._items._hasNull)
        return true;
      ++this._index;
    }
    if (this._maxIndex > 0)
    {
      for (SetStorage.Bucket[] buckets = this._items._buckets; this._index < buckets.Length; ++this._index)
      {
        object obj = buckets[this._index].Item;
        if (obj != null && obj != SetStorage.Removed)
          return true;
      }
    }
    return false;
  }

  [PythonHidden(new PlatformID[] {})]
  public void Reset() => this._index = -2;

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator GetEnumerator() => (IEnumerator) this;

  IEnumerator<object> IEnumerable<object>.GetEnumerator() => (IEnumerator<object>) this;
}
