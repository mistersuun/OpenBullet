// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryKeyEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("dictionary-keyiterator")]
public sealed class DictionaryKeyEnumerator : IEnumerator, IEnumerator<object>, IDisposable
{
  private readonly int _size;
  private readonly DictionaryStorage _dict;
  private readonly IEnumerator<object> _keys;
  private int _pos;

  internal DictionaryKeyEnumerator(DictionaryStorage dict)
  {
    this._dict = dict;
    this._size = dict.Count;
    this._keys = dict.GetKeys().GetEnumerator();
    this._pos = -1;
  }

  bool IEnumerator.MoveNext()
  {
    if (this._size != this._dict.Count)
    {
      this._pos = this._size - 1;
      throw PythonOps.RuntimeError("dictionary changed size during iteration");
    }
    if (!this._keys.MoveNext())
      return false;
    ++this._pos;
    return true;
  }

  void IEnumerator.Reset()
  {
    this._keys.Reset();
    this._pos = -1;
  }

  object IEnumerator.Current => this._keys.Current;

  object IEnumerator<object>.Current => this._keys.Current;

  void IDisposable.Dispose()
  {
  }

  public object __iter__() => (object) this;

  public int __length_hint__() => this._size - this._pos - 1;
}
