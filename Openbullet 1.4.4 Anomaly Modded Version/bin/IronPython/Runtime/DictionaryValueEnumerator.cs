// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryValueEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("dictionary-valueiterator")]
public sealed class DictionaryValueEnumerator : IEnumerator, IEnumerator<object>, IDisposable
{
  private readonly int _size;
  private DictionaryStorage _dict;
  private readonly object[] _values;
  private int _pos;

  internal DictionaryValueEnumerator(DictionaryStorage dict)
  {
    this._dict = dict;
    this._size = dict.Count;
    this._values = new object[this._size];
    int num = 0;
    foreach (KeyValuePair<object, object> keyValuePair in dict.GetItems())
      this._values[num++] = keyValuePair.Value;
    this._pos = -1;
  }

  public bool MoveNext()
  {
    if (this._size != this._dict.Count)
    {
      this._pos = this._size - 1;
      throw PythonOps.RuntimeError("dictionary changed size during iteration");
    }
    if (this._pos + 1 >= this._size)
      return false;
    ++this._pos;
    return true;
  }

  public void Reset() => this._pos = -1;

  public object Current => this._values[this._pos];

  public void Dispose()
  {
  }

  public object __iter__() => (object) this;

  public int __len__() => this._size - this._pos - 1;
}
