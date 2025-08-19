// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.XRange
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("xrange")]
[DontMapIEnumerableToContains]
public sealed class XRange : 
  ICollection,
  IEnumerable,
  IEnumerable<int>,
  ICodeFormattable,
  IList,
  IReversible
{
  private int _start;
  private int _stop;
  private int _step;
  private int _length;

  public XRange(int stop)
    : this(0, stop, 1)
  {
  }

  public XRange(int start, int stop)
    : this(start, stop, 1)
  {
  }

  public XRange(int start, int stop, int step) => this.Initialize(start, stop, step);

  private void Initialize(int start, int stop, int step)
  {
    if (step == 0)
      throw PythonOps.ValueError("step must not be zero");
    if (step > 0)
    {
      if (start > stop)
        stop = start;
    }
    else if (start < stop)
      stop = start;
    this._start = start;
    this._stop = stop;
    this._step = step;
    this._length = this.GetLengthHelper();
    this._stop = start + step * this._length;
  }

  public int Start
  {
    [PythonHidden(new PlatformID[] {})] get => this._start;
  }

  public int Stop
  {
    [PythonHidden(new PlatformID[] {})] get => this._stop;
  }

  public int Step
  {
    [PythonHidden(new PlatformID[] {})] get => this._step;
  }

  public int __len__() => this._length;

  private int GetLengthHelper()
  {
    long num = this._step <= 0 ? ((long) this._stop - (long) this._start + (long) this._step + 1L) / (long) this._step : ((long) this._stop - (long) this._start + (long) this._step - 1L) / (long) this._step;
    return num <= (long) int.MaxValue ? (int) num : throw PythonOps.OverflowError("xrange() result has too many items");
  }

  public object this[int index]
  {
    get
    {
      if (index < 0)
        index += this._length;
      if (index >= this._length || index < 0)
        throw PythonOps.IndexError("xrange object index out of range");
      return ScriptingRuntimeHelpers.Int32ToObject(index * this._step + this._start);
    }
  }

  public object this[object index] => this[Converter.ConvertToIndex(index)];

  public object this[Slice slice] => throw PythonOps.TypeError("sequence index must be integer");

  public IEnumerator __reversed__()
  {
    return (IEnumerator) new XRangeIterator(new XRange(this._stop - this._step, this._start - this._step, -this._step));
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new XRangeIterator(this);

  IEnumerator<int> IEnumerable<int>.GetEnumerator() => (IEnumerator<int>) new XRangeIterator(this);

  public string __repr__(CodeContext context)
  {
    if (this._step != 1)
      return $"xrange({this._start}, {this._stop}, {this._step})";
    return this._start == 0 ? $"xrange({this._stop})" : $"xrange({this._start}, {this._stop})";
  }

  void ICollection.CopyTo(Array array, int index)
  {
    foreach (int num in (IEnumerable<int>) this)
    {
      object obj = (object) num;
      array.SetValue(obj, index++);
    }
  }

  int ICollection.Count => this._length;

  bool ICollection.IsSynchronized => false;

  object ICollection.SyncRoot => (object) null;

  int IList.Add(object value) => throw new InvalidOperationException();

  void IList.Clear() => throw new InvalidOperationException();

  bool IList.Contains(object value) => ((IList) this).IndexOf(value) != -1;

  int IList.IndexOf(object value)
  {
    int num1 = 0;
    foreach (int num2 in (IEnumerable<int>) this)
    {
      if ((ValueType) num2 == value)
        return num1;
      ++num1;
    }
    return -1;
  }

  void IList.Insert(int index, object value) => throw new InvalidOperationException();

  bool IList.IsFixedSize => true;

  bool IList.IsReadOnly => true;

  void IList.Remove(object value) => throw new InvalidOperationException();

  void IList.RemoveAt(int index) => throw new InvalidOperationException();

  object IList.this[int index]
  {
    get
    {
      int num1 = 0;
      foreach (int num2 in (IEnumerable<int>) this)
      {
        object obj = (object) num2;
        if (num1 == index)
          return obj;
        ++num1;
      }
      throw new IndexOutOfRangeException();
    }
    set => throw new InvalidOperationException();
  }
}
