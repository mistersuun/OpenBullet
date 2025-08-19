// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.List
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[PythonType("list")]
[DebuggerTypeProxy(typeof (ObjectCollectionDebugProxy))]
[DebuggerDisplay("list, {Count} items")]
[Serializable]
public class List : 
  IList,
  ICollection,
  IEnumerable,
  ICodeFormattable,
  IList<object>,
  ICollection<object>,
  IEnumerable<object>,
  IReversible,
  IStructuralEquatable,
  IStructuralComparable,
  IReadOnlyList<object>,
  IReadOnlyCollection<object>
{
  private const int INITIAL_SIZE = 20;
  internal int _size;
  internal volatile object[] _data;
  private static readonly object _boxedOne = ScriptingRuntimeHelpers.Int32ToObject(1);
  public const object __hash__ = null;

  public void __init__()
  {
    this._data = new object[8];
    this._size = 0;
  }

  public void __init__([NotNull] IEnumerable enumerable)
  {
    this.__init__();
    foreach (object obj in enumerable)
      this.AddNoLock(obj);
  }

  public void __init__([NotNull] ICollection sequence)
  {
    this._data = new object[sequence.Count];
    int num = 0;
    foreach (object obj in (IEnumerable) sequence)
      this._data[num++] = obj;
    this._size = num;
  }

  public void __init__([NotNull] SetCollection sequence)
  {
    List items = sequence._items.GetItems();
    this._size = items._size;
    this._data = items._data;
  }

  public void __init__([NotNull] FrozenSetCollection sequence)
  {
    List items = sequence._items.GetItems();
    this._size = items._size;
    this._data = items._data;
  }

  public void __init__([NotNull] List sequence)
  {
    if (this == sequence)
    {
      this._size = 0;
    }
    else
    {
      this._data = new object[sequence._size];
      object[] data = sequence._data;
      for (int index = 0; index < this._data.Length; ++index)
        this._data[index] = data[index];
      this._size = this._data.Length;
    }
  }

  public void __init__([NotNull] string sequence)
  {
    this._data = new object[sequence.Length];
    this._size = sequence.Length;
    for (int index = 0; index < sequence.Length; ++index)
      this._data[index] = (object) ScriptingRuntimeHelpers.CharToString(sequence[index]);
  }

  public void __init__(CodeContext context, object sequence)
  {
    try
    {
      object obj;
      if (PythonTypeOps.TryInvokeUnaryOperator(context, sequence, "__len__", out obj))
      {
        this._data = new object[context.LanguageContext.ConvertToInt32(obj)];
        this._size = 0;
        this.extend(sequence);
      }
      else
      {
        this._data = new object[20];
        this._size = 0;
        this.extend(sequence);
      }
    }
    catch (MissingMemberException ex)
    {
      this._data = new object[20];
      this._size = 0;
      this.extend(sequence);
    }
  }

  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.List ? (object) new List() : cls.CreateInstance(context);
  }

  public static object __new__(CodeContext context, PythonType cls, object arg)
  {
    return List.__new__(context, cls);
  }

  public static object __new__(CodeContext context, PythonType cls, params object[] argsø)
  {
    return List.__new__(context, cls);
  }

  public static object __new__(
    CodeContext context,
    PythonType cls,
    [ParamDictionary] IDictionary<object, object> kwArgsø,
    params object[] argsø)
  {
    return List.__new__(context, cls);
  }

  private List(IEnumerator e)
    : this(10)
  {
    while (e.MoveNext())
      this.AddNoLock(e.Current);
  }

  internal List(int capacity)
  {
    if (capacity == 0)
      this._data = ArrayUtils.EmptyObjects;
    else
      this._data = new object[capacity];
  }

  private List(params object[] items)
  {
    this._data = items;
    this._size = this._data.Length;
  }

  public List()
    : this(0)
  {
  }

  internal List(object sequence)
  {
    if (sequence is ICollection collection)
    {
      this._data = new object[collection.Count];
      int num = 0;
      foreach (object obj in (IEnumerable) collection)
        this._data[num++] = obj;
      this._size = num;
    }
    else
    {
      object obj;
      if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, sequence, "__len__", out obj))
      {
        this._data = new object[Converter.ConvertToInt32(obj)];
        this.extend(sequence);
      }
      else
      {
        this._data = new object[20];
        this.extend(sequence);
      }
    }
  }

  internal List(ICollection items)
    : this(items.Count)
  {
    int num = 0;
    foreach (object obj in (IEnumerable) items)
      this._data[num++] = obj;
    this._size = num;
  }

  internal static List FromArrayNoCopy(params object[] data) => new List(data);

  internal object[] GetObjectArray()
  {
    lock (this)
      return ArrayOps.CopyArray(this._data, this._size);
  }

  public static List operator +([NotNull] List l1, [NotNull] List l2)
  {
    object[] objArray;
    int size;
    lock (l1)
    {
      objArray = ArrayOps.CopyArray(l1._data, List.GetAddSize(l1._size, l2._size));
      size = l1._size;
    }
    lock (l2)
    {
      if (l2._size + size > objArray.Length)
        objArray = ArrayOps.CopyArray(objArray, List.GetAddSize(size, l2._size));
      Array.Copy((Array) l2._data, 0, (Array) objArray, size, l2._size);
      return new List(objArray) { _size = size + l2._size };
    }
  }

  private static int GetAddSize(int s1, int s2) => List.GetNewSize(s1 + s2);

  private static int GetNewSize(int length)
  {
    return length > 256 /*0x0100*/ ? length + (int) sbyte.MaxValue & (int) sbyte.MinValue : length + 15 & -16;
  }

  public static List operator *([NotNull] List l, int count) => List.MultiplyWorker(l, count);

  public static List operator *(int count, List l) => List.MultiplyWorker(l, count);

  public static object operator *([NotNull] List self, [NotNull] Index count)
  {
    return PythonOps.MultiplySequence<List>(new PythonOps.MultiplySequenceWorker<List>(List.MultiplyWorker), self, count, true);
  }

  public static object operator *([NotNull] Index count, [NotNull] List self)
  {
    return PythonOps.MultiplySequence<List>(new PythonOps.MultiplySequenceWorker<List>(List.MultiplyWorker), self, count, false);
  }

  public static object operator *([NotNull] List self, object count)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) (self * index);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  public static object operator *(object count, [NotNull] List self)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) (index * self);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  private static List MultiplyWorker(List self, int count)
  {
    if (count <= 0)
      return PythonOps.MakeEmptyList(0);
    int size;
    int newSize;
    object[] objArray;
    lock (self)
    {
      size = self._size;
      newSize = checked (size * count);
      objArray = ArrayOps.CopyArray(self._data, newSize);
    }
    int val1 = size;
    int destinationIndex = size;
    while (destinationIndex < newSize)
    {
      Array.Copy((Array) objArray, 0, (Array) objArray, destinationIndex, Math.Min(val1, newSize - destinationIndex));
      destinationIndex += val1;
      val1 *= 2;
    }
    return new List(objArray);
  }

  public virtual int __len__() => this._size;

  public virtual IEnumerator __iter__() => (IEnumerator) new ListIterator(this);

  public virtual IEnumerator __reversed__() => (IEnumerator) new ListReverseIterator(this);

  public virtual bool __contains__(object value) => this.ContainsWorker(value);

  internal bool ContainsWorker(object value)
  {
    bool lockTaken = false;
    try
    {
      MonitorUtils.Enter((object) this, ref lockTaken);
      for (int index = 0; index < this._size; ++index)
      {
        object x = this._data[index];
        MonitorUtils.Exit((object) this, ref lockTaken);
        try
        {
          if (PythonOps.EqualRetBool(x, value))
            return true;
        }
        finally
        {
          MonitorUtils.Enter((object) this, ref lockTaken);
        }
      }
    }
    finally
    {
      if (lockTaken)
        Monitor.Exit((object) this);
    }
    return false;
  }

  internal void AddRange<T>(ICollection<T> otherList)
  {
    foreach (T other in (IEnumerable<T>) otherList)
      this.append((object) other);
  }

  [SpecialName]
  public virtual object InPlaceAdd(object other)
  {
    if (this != other)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(other);
      while (enumerator.MoveNext())
        this.append(enumerator.Current);
    }
    else
      this.InPlaceMultiply(2);
    return (object) this;
  }

  [SpecialName]
  public List InPlaceMultiply(int count)
  {
    lock (this)
    {
      int size = this._size;
      int needed = checked (size * count);
      this.EnsureSize(needed);
      int val1 = size;
      int destinationIndex = size;
      while (destinationIndex < needed)
      {
        Array.Copy((Array) this._data, 0, (Array) this._data, destinationIndex, Math.Min(val1, needed - destinationIndex));
        destinationIndex += val1;
        val1 *= 2;
      }
      this._size = needed;
    }
    return this;
  }

  [SpecialName]
  public object InPlaceMultiply(Index count)
  {
    return PythonOps.MultiplySequence<List>(new PythonOps.MultiplySequenceWorker<List>(List.InPlaceMultiplyWorker), this, count, true);
  }

  [SpecialName]
  public object InPlaceMultiply(object count)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) this.InPlaceMultiply(index);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  private static List InPlaceMultiplyWorker(List self, int count) => self.InPlaceMultiply(count);

  public virtual object __getslice__(int start, int stop)
  {
    lock (this)
    {
      Slice.FixSliceArguments(this._size, ref start, ref stop);
      return (object) new List(ArrayOps.GetSlice(this._data, start, stop));
    }
  }

  internal object[] GetSliceAsArray(int start, int stop)
  {
    if (start < 0)
      start = 0;
    if (stop > this.Count)
      stop = this.Count;
    lock (this)
      return ArrayOps.GetSlice(this._data, start, stop);
  }

  public virtual void __setslice__(int start, int stop, object value)
  {
    Slice.FixSliceArguments(this._size, ref start, ref stop);
    if (value is List)
      this.SliceNoStep(start, stop, (List) value);
    else
      this.SliceNoStep(start, stop, value);
  }

  public virtual void __delslice__(int start, int stop)
  {
    lock (this)
    {
      Slice.FixSliceArguments(this._size, ref start, ref stop);
      if (start > stop)
        return;
      int index1 = start;
      int index2 = stop;
      while (index2 < this._size)
      {
        this._data[index1] = this._data[index2];
        ++index2;
        ++index1;
      }
      this._size -= stop - start;
    }
  }

  public virtual object this[Slice slice]
  {
    get
    {
      if (slice == null)
        throw PythonOps.TypeError("list indices must be integer or slice, not None");
      int ostart;
      int ostop;
      int ostep;
      slice.indices(this._size, out ostart, out ostop, out ostep);
      if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
        return (object) new List();
      if (ostep == 1)
      {
        object[] slice1;
        lock (this)
          slice1 = ArrayOps.GetSlice(this._data, ostart, ostop);
        return (object) new List(slice1);
      }
      int length = ostep > 0 ? (int) (((long) ostop - (long) ostart + (long) ostep - 1L) / (long) ostep) : (int) (((long) ostop - (long) ostart + (long) ostep + 1L) / (long) ostep);
      object[] objArray = new object[length];
      lock (this)
      {
        int num1 = 0;
        int num2 = 0;
        int index = ostart;
        while (num2 < length)
        {
          objArray[num1++] = this._data[index];
          ++num2;
          index += ostep;
        }
      }
      return (object) new List(objArray);
    }
    set
    {
      if (slice == null)
        throw PythonOps.TypeError("list indices must be integer or slice, not None");
      if (slice.step != null && (!(slice.step is int) || !slice.step.Equals(List._boxedOne)))
      {
        if (this == value)
          value = (object) new List(value);
        if (List.ValueRequiresNoLocks(value))
        {
          lock (this)
            slice.DoSliceAssign(new Slice.SliceAssign(this.SliceAssignNoLock), this._size, value);
        }
        else
          slice.DoSliceAssign(new Slice.SliceAssign(this.SliceAssign), this._size, value);
      }
      else
      {
        int ostart;
        int ostop;
        slice.indices(this._size, out ostart, out ostop, out int _);
        if (value is List other)
          this.SliceNoStep(ostart, ostop, other);
        else
          this.SliceNoStep(ostart, ostop, value);
      }
    }
  }

  private static bool ValueRequiresNoLocks(object value)
  {
    switch (value)
    {
      case PythonTuple _:
      case Array _:
        return true;
      default:
        return value is FrozenSetCollection;
    }
  }

  private void SliceNoStep(int start, int stop, List other)
  {
    int size = other._size;
    object[] data = other._data;
    lock (this)
    {
      if (stop - start == size)
      {
        for (int index = 0; index < size; ++index)
          this._data[index + start] = data[index];
      }
      else
      {
        stop = Math.Max(stop, start);
        int length = this._size - (stop - start) + size;
        object[] objArray = new object[List.GetNewSize(length)];
        for (int index = 0; index < start; ++index)
          objArray[index] = this._data[index];
        for (int index = 0; index < size; ++index)
          objArray[index + start] = data[index];
        int num = size - (stop - start);
        for (int index = stop; index < this._size; ++index)
          objArray[index + num] = this._data[index];
        this._size = length;
        this._data = objArray;
      }
    }
  }

  private void SliceNoStep(int start, int stop, object value)
  {
    if (!(value is IList<object> objectList1))
      objectList1 = (IList<object>) new List(PythonOps.GetEnumerator(value));
    IList<object> objectList2 = objectList1;
    lock (this)
    {
      if (stop - start == objectList2.Count)
      {
        for (int index = 0; index < objectList2.Count; ++index)
          this._data[index + start] = objectList2[index];
      }
      else
      {
        stop = Math.Max(stop, start);
        int length = this._size - (stop - start) + objectList2.Count;
        object[] objArray = new object[List.GetNewSize(length)];
        for (int index = 0; index < start; ++index)
          objArray[index] = this._data[index];
        for (int index = 0; index < objectList2.Count; ++index)
          objArray[index + start] = objectList2[index];
        int num = objectList2.Count - (stop - start);
        for (int index = stop; index < this._size; ++index)
          objArray[index + num] = this._data[index];
        this._size = length;
        this._data = objArray;
      }
    }
  }

  private void SliceAssign(int index, object value) => this[index] = value;

  private void SliceAssignNoLock(int index, object value) => this._data[index] = value;

  public virtual void __delitem__(int index)
  {
    lock (this)
      this.RawDelete(PythonOps.FixIndex(index, this._size));
  }

  public virtual void __delitem__(object index)
  {
    this.__delitem__(Converter.ConvertToIndex(index));
  }

  public void __delitem__(Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("list indices must be integers or slices");
    lock (this)
    {
      int ostart;
      int ostop;
      int ostep;
      slice.indices(this._size, out ostart, out ostop, out ostep);
      if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
        return;
      switch (ostep)
      {
        case -1:
          int index1 = ostop + 1;
          int index2 = ostart + 1;
          while (index2 < this._size)
          {
            this._data[index1] = this._data[index2];
            ++index2;
            ++index1;
          }
          this._size -= ostart - ostop;
          break;
        case 1:
          int index3 = ostart;
          int index4 = ostop;
          while (index4 < this._size)
          {
            this._data[index3] = this._data[index4];
            ++index4;
            ++index3;
          }
          this._size -= ostop - ostart;
          break;
        default:
          if (ostep < 0)
          {
            int num1 = ostart;
            while (num1 > ostop)
              num1 += ostep;
            int num2 = num1 - ostep;
            ostop = ostart + 1;
            ostart = num2;
            ostep = -ostep;
          }
          int num3;
          int index5 = num3 = ostart;
          int num4 = num3;
          int num5;
          for (num5 = num3; num5 < ostop && index5 < ostop; ++index5)
          {
            if (index5 != num4)
              this._data[num5++] = this._data[index5];
            else
              num4 += ostep;
          }
          while (ostop < this._size)
            this._data[num5++] = this._data[ostop++];
          this._size = num5;
          break;
      }
    }
  }

  private void RawDelete(int index)
  {
    int index1 = --this._size;
    object[] data = this._data;
    for (int index2 = index; index2 < index1; ++index2)
      data[index2] = data[index2 + 1];
    data[index1] = (object) null;
  }

  internal void EnsureSize(int needed)
  {
    if (this._data.Length >= needed)
      return;
    if (this._data.Length == 0)
    {
      this._data = new object[4];
    }
    else
    {
      int newSize = Math.Max(this._size * 3, 10);
      while (newSize < needed)
        newSize *= 2;
      this._data = ArrayOps.CopyArray(this._data, newSize);
    }
  }

  public void append(object item)
  {
    lock (this)
      this.AddNoLock(item);
  }

  internal void AddNoLock(object item)
  {
    this.EnsureSize(this._size + 1);
    this._data[this._size] = item;
    ++this._size;
  }

  internal void AddNoLockNoDups(object item)
  {
    for (int index = 0; index < this._size; ++index)
    {
      if (PythonOps.EqualRetBool(this._data[index], item))
        return;
    }
    this.AddNoLock(item);
  }

  internal void AppendListNoLockNoDups(List list)
  {
    if (list == null)
      return;
    foreach (object obj in list)
      this.AddNoLockNoDups(obj);
  }

  public int count(object item)
  {
    bool lockTaken = false;
    try
    {
      MonitorUtils.Enter((object) this, ref lockTaken);
      int num = 0;
      int index = 0;
      for (int size = this._size; index < size; ++index)
      {
        object x = this._data[index];
        MonitorUtils.Exit((object) this, ref lockTaken);
        try
        {
          if (PythonOps.EqualRetBool(x, item))
            ++num;
        }
        finally
        {
          MonitorUtils.Enter((object) this, ref lockTaken);
        }
      }
      return num;
    }
    finally
    {
      if (lockTaken)
        Monitor.Exit((object) this);
    }
  }

  public void extend([NotNull] List seq)
  {
    using (new OrderedLocker((object) this, (object) seq))
    {
      int count = seq.Count;
      this.EnsureSize(this.Count + count);
      for (int index = 0; index < count; ++index)
        this.AddNoLock(seq[index]);
    }
  }

  public void extend([NotNull] PythonTuple seq)
  {
    lock (this)
    {
      this.EnsureSize(this.Count + seq.Count);
      for (int index = 0; index < seq.Count; ++index)
        this.AddNoLock(seq[index]);
    }
  }

  public void extend(object seq)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(seq);
    if (seq == this)
      enumerator = new List(enumerator).GetEnumerator();
    while (enumerator.MoveNext())
      this.append(enumerator.Current);
  }

  public int index(object item) => this.index(item, 0, this._size);

  public int index(object item, int start) => this.index(item, start, this._size);

  public int index(object item, int start, int stop)
  {
    object[] data;
    int size;
    lock (this)
    {
      data = this._data;
      size = this._size;
    }
    start = PythonOps.FixSliceIndex(start, size);
    stop = PythonOps.FixSliceIndex(stop, size);
    for (int index = start; index < Math.Min(stop, Math.Min(size, this._size)); ++index)
    {
      if (PythonOps.EqualRetBool(data[index], item))
        return index;
    }
    throw PythonOps.ValueError("list.index(item): item not in list");
  }

  public int index(object item, object start)
  {
    return this.index(item, Converter.ConvertToIndex(start), this._size);
  }

  public int index(object item, object start, object stop)
  {
    return this.index(item, Converter.ConvertToIndex(start), Converter.ConvertToIndex(stop));
  }

  public void insert(int index, object value)
  {
    if (index >= this._size)
    {
      this.append(value);
    }
    else
    {
      lock (this)
      {
        index = PythonOps.FixSliceIndex(index, this._size);
        this.EnsureSize(this._size + 1);
        ++this._size;
        for (int index1 = this._size - 1; index1 > index; --index1)
          this._data[index1] = this._data[index1 - 1];
        this._data[index] = value;
      }
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public void Insert(int index, object value) => this.insert(index, value);

  public object pop()
  {
    if (this._size == 0)
      throw PythonOps.IndexError("pop off of empty list");
    lock (this)
    {
      --this._size;
      return this._data[this._size];
    }
  }

  public object pop(int index)
  {
    lock (this)
    {
      index = PythonOps.FixIndex(index, this._size);
      if (this._size == 0)
        throw PythonOps.IndexError("pop off of empty list");
      object obj = this._data[index];
      --this._size;
      for (int index1 = index; index1 < this._size; ++index1)
        this._data[index1] = this._data[index1 + 1];
      return obj;
    }
  }

  public void remove(object value)
  {
    lock (this)
      this.RawDelete(this.index(value));
  }

  void IList.Remove(object value) => this.remove(value);

  public void reverse()
  {
    lock (this)
      Array.Reverse((Array) this._data, 0, this._size);
  }

  internal void reverse(int index, int count)
  {
    lock (this)
      Array.Reverse((Array) this._data, index, count);
  }

  public void sort(CodeContext context) => this.sort(context, (object) null, (object) null, false);

  public void sort(CodeContext context, object cmp)
  {
    this.sort(context, cmp, (object) null, false);
  }

  public void sort(CodeContext context, object cmp, object key)
  {
    this.sort(context, cmp, key, false);
  }

  public void sort(CodeContext context, object cmp = null, object key = null, bool reverse = false)
  {
    if (cmp != null)
      PythonOps.Warn3k(context, "the cmp argument is not supported in 3.x");
    if (this._size == 0)
      return;
    IComparer comparer = context.LanguageContext.GetComparer(cmp, this.GetComparisonType());
    this.DoSort(context, comparer, key, reverse, 0, this._size);
  }

  private Type GetComparisonType()
  {
    if (this._size >= 4000)
      return (Type) null;
    return this._data.Length != 0 ? CompilerHelpers.GetType(this._data[0]) : typeof (object);
  }

  internal void DoSort(
    CodeContext context,
    IComparer cmp,
    object key,
    bool reverse,
    int index,
    int count)
  {
    lock (this)
    {
      object[] sortData = this._data;
      int size = this._size;
      try
      {
        this._data = ArrayUtils.EmptyObjects;
        this._size = 0;
        if (key != null)
        {
          object[] keys = new object[size];
          for (int index1 = 0; index1 < size; ++index1)
          {
            keys[index1] = PythonCalls.Call(context, key, sortData[index1]);
            if (this._data.Length != 0)
              throw PythonOps.ValueError("list mutated while determing keys");
          }
          sortData = this.ListMergeSort(sortData, keys, cmp, index, count, reverse);
        }
        else
          sortData = this.ListMergeSort(sortData, cmp, index, count, reverse);
      }
      finally
      {
        this._data = sortData;
        this._size = size;
      }
    }
  }

  internal object[] ListMergeSort(
    object[] sortData,
    IComparer cmp,
    int index,
    int count,
    bool reverse)
  {
    return this.ListMergeSort(sortData, (object[]) null, cmp, index, count, reverse);
  }

  internal object[] ListMergeSort(
    object[] sortData,
    object[] keys,
    IComparer cmp,
    int index,
    int count,
    bool reverse)
  {
    if (count - index < 2)
      return sortData;
    if (keys == null)
      keys = sortData;
    int length = count - index;
    int[] numArray = new int[length + 2];
    numArray[0] = 1;
    numArray[length + 1] = 2;
    for (int index1 = 1; index1 <= length - 2; ++index1)
      numArray[index1] = -(index1 + 2);
    numArray[length - 1] = numArray[length] = 0;
    while (true)
    {
      int index2 = 0;
      int index3 = length + 1;
      int index4 = numArray[index2];
      int index5 = numArray[index3];
      if (index5 != 0)
      {
        do
        {
          if (index4 < 1 || index5 <= length && this.DoCompare(keys, cmp, index4 + index - 1, index5 + index - 1, reverse))
          {
            numArray[index2] = numArray[index2] >= 0 ? Math.Abs(index4) : Math.Abs(index4) * -1;
            index2 = index4;
            index4 = numArray[index4];
            if (index4 <= 0)
            {
              numArray[index2] = index5;
              index2 = index3;
              do
              {
                index3 = index5;
                index5 = numArray[index5];
              }
              while (index5 > 0);
            }
            else
              continue;
          }
          else
          {
            numArray[index2] = numArray[index2] >= 0 ? Math.Abs(index5) : Math.Abs(index5) * -1;
            index2 = index5;
            index5 = numArray[index5];
            if (index5 <= 0)
            {
              numArray[index2] = index4;
              index2 = index3;
              do
              {
                index3 = index4;
                index4 = numArray[index4];
              }
              while (index4 > 0);
            }
            else
              continue;
          }
          index4 *= -1;
          index5 *= -1;
        }
        while (index5 != 0);
        numArray[index2] = numArray[index2] >= 0 ? Math.Abs(index4) : Math.Abs(index4) * -1;
        numArray[index3] = 0;
      }
      else
        break;
    }
    object[] objArray = new object[length];
    int index6 = numArray[0];
    int num = 0;
    for (; index6 != 0; index6 = numArray[index6])
      objArray[num++] = sortData[index6 + index - 1];
    if (sortData.Length != count || index != 0)
    {
      for (int index7 = 0; index7 < count; ++index7)
        sortData[index7 + index] = objArray[index7];
    }
    else
      sortData = objArray;
    return sortData;
  }

  private bool DoCompare(object[] keys, IComparer cmp, int p, int q, bool reverse)
  {
    int num = cmp.Compare(keys[p], keys[q]);
    bool flag = reverse ? num >= 0 : num <= 0;
    if (this._data.Length != 0)
      throw PythonOps.ValueError("list mutated during sort");
    return flag;
  }

  internal int BinarySearch(int index, int count, object value, IComparer comparer)
  {
    lock (this)
      return Array.BinarySearch((Array) this._data, index, count, value, comparer);
  }

  internal bool EqualsWorker(List l, IEqualityComparer comparer)
  {
    using (new OrderedLocker((object) this, (object) l))
      return comparer == null ? PythonOps.ArraysEqual(this._data, this._size, l._data, l._size) : PythonOps.ArraysEqual(this._data, this._size, l._data, l._size, comparer);
  }

  internal int CompareToWorker(List l) => this.CompareToWorker(l, (IComparer) null);

  internal int CompareToWorker(List l, IComparer comparer)
  {
    using (new OrderedLocker((object) this, (object) l))
      return comparer == null ? PythonOps.CompareArrays(this._data, this._size, l._data, l._size) : PythonOps.CompareArrays(this._data, this._size, l._data, l._size, comparer);
  }

  internal bool FastSwap(int i, int j)
  {
    if (i > j)
    {
      int num = i;
      i = j;
      j = num;
    }
    if (i < 0 || j >= this._size)
      return false;
    if (i == j)
      return true;
    object obj = this._data[i];
    this._data[i] = this._data[j];
    this._data[j] = obj;
    return true;
  }

  bool IList.IsReadOnly => false;

  public virtual object this[int index]
  {
    get => this.GetData()[PythonOps.FixIndex(index, this._size)];
    set
    {
      lock (this)
        this._data[PythonOps.FixIndex(index, this._size)] = value;
    }
  }

  public virtual object this[BigInteger index]
  {
    get => this[(int) index];
    set => this[(int) index] = value;
  }

  public virtual object this[object index]
  {
    get => this[Converter.ConvertToIndex(index)];
    set => this[Converter.ConvertToIndex(index)] = value;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private object[] GetData() => this._data;

  [PythonHidden(new PlatformID[] {})]
  public void RemoveAt(int index)
  {
    lock (this)
      this.RawDelete(index);
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(object value) => this.__contains__(value);

  [PythonHidden(new PlatformID[] {})]
  public void Clear()
  {
    lock (this)
      this._size = 0;
  }

  [PythonHidden(new PlatformID[] {})]
  public int IndexOf(object value)
  {
    object[] data;
    int size;
    lock (this)
    {
      data = this._data;
      size = this._size;
    }
    for (int index = 0; index < Math.Min(size, this._size); ++index)
    {
      if (PythonOps.EqualRetBool(data[index], value))
        return index;
    }
    return -1;
  }

  [PythonHidden(new PlatformID[] {})]
  public int Add(object value)
  {
    lock (this)
    {
      this.AddNoLock(value);
      return this._size - 1;
    }
  }

  bool IList.IsFixedSize => false;

  bool ICollection.IsSynchronized => false;

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._size;
  }

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(Array array, int index)
  {
    Array.Copy((Array) this._data, 0, array, index, this._size);
  }

  internal void CopyTo(Array array, int index, int arrayIndex, int count)
  {
    Array.Copy((Array) this._data, index, array, arrayIndex, count);
  }

  object ICollection.SyncRoot => (object) this;

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator GetEnumerator() => this.__iter__();

  public virtual string __repr__(CodeContext context)
  {
    List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) this);
    if (andCheckInfinite == null)
      return "[...]";
    int count = andCheckInfinite.Count;
    andCheckInfinite.Add((object) this);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      for (int index = 0; index < this._size; ++index)
      {
        if (index > 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(PythonOps.Repr(context, this._data[index]));
      }
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
    finally
    {
      andCheckInfinite.RemoveAt(count);
    }
  }

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    if (CompareUtil.Check((object) this))
      return 0;
    CompareUtil.Push((object) this);
    try
    {
      return ((IStructuralEquatable) new PythonTuple((object) this)).GetHashCode(comparer);
    }
    finally
    {
      CompareUtil.Pop((object) this);
    }
  }

  bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
  {
    if (this == other)
      return true;
    return other is List other1 && other1.Count == this.Count && this.Equals(other1, comparer);
  }

  void ICollection<object>.Add(object item) => this.append(item);

  public void CopyTo(object[] array, int arrayIndex)
  {
    for (int index = 0; index < this.Count; ++index)
      array[arrayIndex + index] = this[index];
  }

  bool ICollection<object>.IsReadOnly => ((IList) this).IsReadOnly;

  [PythonHidden(new PlatformID[] {})]
  public bool Remove(object item)
  {
    if (!this.__contains__(item))
      return false;
    this.remove(item);
    return true;
  }

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new IEnumeratorOfTWrapper<object>(this.GetEnumerator());
  }

  private bool Equals(List other) => this.Equals(other, (IEqualityComparer) null);

  private bool Equals(List other, IEqualityComparer comparer)
  {
    CompareUtil.Push((object) this, (object) other);
    try
    {
      return this.EqualsWorker(other, comparer);
    }
    finally
    {
      CompareUtil.Pop((object) this, (object) other);
    }
  }

  internal int CompareTo(List other) => this.CompareTo(other, (IComparer) null);

  internal int CompareTo(List other, IComparer comparer)
  {
    CompareUtil.Push((object) this, (object) other);
    try
    {
      return this.CompareToWorker(other, comparer);
    }
    finally
    {
      CompareUtil.Pop((object) this, (object) other);
    }
  }

  public static object operator >(List self, object other)
  {
    if (!(other is List other1))
      return (object) NotImplementedType.Value;
    return self.CompareTo(other1) <= 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object operator <(List self, object other)
  {
    if (!(other is List other1))
      return (object) NotImplementedType.Value;
    return self.CompareTo(other1) >= 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object operator >=(List self, object other)
  {
    if (!(other is List other1))
      return (object) NotImplementedType.Value;
    return self.CompareTo(other1) < 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object operator <=(List self, object other)
  {
    if (!(other is List other1))
      return (object) NotImplementedType.Value;
    return self.CompareTo(other1) > 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  int IStructuralComparable.CompareTo(object other, IComparer comparer)
  {
    return other is List other1 ? this.CompareTo(other1, comparer) : throw new ValueErrorException("expected List");
  }
}
