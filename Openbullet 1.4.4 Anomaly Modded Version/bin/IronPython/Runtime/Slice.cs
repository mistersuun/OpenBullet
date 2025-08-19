// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Slice
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;

#nullable disable
namespace IronPython.Runtime;

[PythonType("slice")]
public sealed class Slice : ICodeFormattable, IComparable, ISlice
{
  private readonly object _start;
  private readonly object _stop;
  private readonly object _step;

  public Slice(object stop)
    : this((object) null, stop, (object) null)
  {
  }

  public Slice(object start, object stop)
    : this(start, stop, (object) null)
  {
  }

  public Slice(object start, object stop, object step)
  {
    this._start = start;
    this._stop = stop;
    this._step = step;
  }

  public object start => this._start;

  public object stop => this._stop;

  public object step => this._step;

  public int __cmp__(Slice obj)
  {
    return PythonOps.CompareArrays(new object[3]
    {
      this._start,
      this._stop,
      this._step
    }, 3, new object[3]{ obj._start, obj._stop, obj._step }, 3);
  }

  public void indices(int len, out int ostart, out int ostop, out int ostep)
  {
    PythonOps.FixSlice(len, this._start, this._stop, this._step, out ostart, out ostop, out ostep);
  }

  public void indices(object len, out int ostart, out int ostop, out int ostep)
  {
    PythonOps.FixSlice(Converter.ConvertToIndex(len), this._start, this._stop, this._step, out ostart, out ostop, out ostep);
  }

  public PythonTuple __reduce__()
  {
    return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonTypeFromType(typeof (Slice)), (object) PythonTuple.MakeTuple(this._start, this._stop, this._step));
  }

  int IComparable.CompareTo(object obj)
  {
    return obj is Slice slice ? this.__cmp__(slice) : throw new ValueErrorException("expected slice");
  }

  public int __hash__() => throw PythonOps.TypeErrorForUnhashableType("slice");

  object ISlice.Start => this.start;

  object ISlice.Stop => this.stop;

  object ISlice.Step => this.step;

  public string __repr__(CodeContext context)
  {
    return $"slice({PythonOps.Repr(context, this._start)}, {PythonOps.Repr(context, this._stop)}, {PythonOps.Repr(context, this._step)})";
  }

  internal static void FixSliceArguments(int size, ref int start, ref int stop)
  {
    start = start < 0 ? 0 : (start > size ? size : start);
    stop = stop < 0 ? 0 : (stop > size ? size : stop);
  }

  internal static void FixSliceArguments(long size, ref long start, ref long stop)
  {
    start = start < 0L ? 0L : (start > size ? size : start);
    stop = stop < 0L ? 0L : (stop > size ? size : stop);
  }

  internal void DeprecatedFixed(object self, out int newStart, out int newStop)
  {
    bool flag = false;
    int num = 0;
    if (this._start != null)
    {
      newStart = Converter.ConvertToIndex(this._start);
      if (newStart < 0)
      {
        flag = true;
        num = PythonOps.Length(self);
        newStart += num;
      }
    }
    else
      newStart = 0;
    if (this._stop != null)
    {
      newStop = Converter.ConvertToIndex(this._stop);
      if (newStop >= 0)
        return;
      if (!flag)
        num = PythonOps.Length(self);
      newStop += num;
    }
    else
      newStop = int.MaxValue;
  }

  internal void DoSliceAssign(Slice.SliceAssign assign, int size, object value)
  {
    int ostart;
    int ostop;
    int ostep;
    this.indices(size, out ostart, out ostop, out ostep);
    Slice.DoSliceAssign(assign, ostart, ostop, ostep, value);
  }

  private static void DoSliceAssign(
    Slice.SliceAssign assign,
    int start,
    int stop,
    int step,
    object value)
  {
    stop = step > 0 ? Math.Max(stop, start) : Math.Min(stop, start);
    int n = Math.Max(0, (step > 0 ? stop - start + step - 1 : stop - start + step + 1) / step);
    if (value is IList)
      Slice.ListSliceAssign(assign, start, n, step, value as IList);
    else
      Slice.OtherSliceAssign(assign, start, stop, step, value);
  }

  private static void ListSliceAssign(
    Slice.SliceAssign assign,
    int start,
    int n,
    int step,
    IList lst)
  {
    if (lst.Count < n)
      throw PythonOps.ValueError("too few items in the enumerator. need {0} have {1}", (object) n, (object) lst.Count);
    if (lst.Count != n)
      throw PythonOps.ValueError("too many items in the enumerator need {0} have {1}", (object) n, (object) lst.Count);
    int index1 = 0;
    int index2 = start;
    while (index1 < n)
    {
      assign(index2, lst[index1]);
      ++index1;
      index2 += step;
    }
  }

  private static void OtherSliceAssign(
    Slice.SliceAssign assign,
    int start,
    int stop,
    int step,
    object value)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(value);
    List list = new List();
    while (enumerator.MoveNext())
      list.AddNoLock(enumerator.Current);
    Slice.DoSliceAssign(assign, start, stop, step, (object) list);
  }

  internal delegate void SliceAssign(int index, object value);
}
