// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTuple
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[PythonType("tuple")]
[DebuggerTypeProxy(typeof (CollectionDebugProxy))]
[DebuggerDisplay("tuple, {Count} items")]
[Serializable]
public class PythonTuple : 
  IList,
  ICollection,
  IEnumerable,
  IList<object>,
  ICollection<object>,
  IEnumerable<object>,
  ICodeFormattable,
  IExpressionSerializable,
  IStructuralEquatable,
  IStructuralComparable,
  IReadOnlyList<object>,
  IReadOnlyCollection<object>
{
  internal readonly object[] _data;
  internal static readonly PythonTuple EMPTY = new PythonTuple();

  public PythonTuple(object o) => this._data = PythonTuple.MakeItems(o);

  protected PythonTuple(object[] items) => this._data = items;

  public PythonTuple() => this._data = ArrayUtils.EmptyObjects;

  internal PythonTuple(PythonTuple other, object o) => this._data = other.Expand(o);

  public static PythonTuple __new__(CodeContext context, PythonType cls)
  {
    if (cls == TypeCache.PythonTuple)
      return PythonTuple.EMPTY;
    if (!(cls.CreateInstance(context) is PythonTuple instance))
      throw PythonOps.TypeError("{0} is not a subclass of tuple", (object) cls);
    return instance;
  }

  public static PythonTuple __new__(CodeContext context, PythonType cls, object sequence)
  {
    if (sequence == null)
      throw PythonOps.TypeError("iteration over a non-sequence");
    if (cls == TypeCache.PythonTuple)
      return sequence.GetType() == typeof (PythonTuple) ? (PythonTuple) sequence : new PythonTuple(PythonTuple.MakeItems(sequence));
    if (!(cls.CreateInstance(context, sequence) is PythonTuple instance))
      throw PythonOps.TypeError("{0} is not a subclass of tuple", (object) cls);
    return instance;
  }

  public int index(object obj, object start)
  {
    return this.index(obj, Converter.ConvertToIndex(start), this._data.Length);
  }

  public int index(object obj, int start = 0) => this.index(obj, start, this._data.Length);

  public int index(object obj, object start, object end)
  {
    return this.index(obj, Converter.ConvertToIndex(start), Converter.ConvertToIndex(end));
  }

  public int index(object obj, int start, int end)
  {
    start = PythonOps.FixSliceIndex(start, this._data.Length);
    end = PythonOps.FixSliceIndex(end, this._data.Length);
    for (int index = start; index < end; ++index)
    {
      if (PythonOps.EqualRetBool(obj, this._data[index]))
        return index;
    }
    throw PythonOps.ValueError("tuple.index(x): x not in list");
  }

  public int count(object obj)
  {
    int num = 0;
    foreach (object y in this._data)
    {
      if (PythonOps.EqualRetBool(obj, y))
        ++num;
    }
    return num;
  }

  internal static PythonTuple Make(object o)
  {
    return o is PythonTuple ? (PythonTuple) o : new PythonTuple(PythonTuple.MakeItems(o));
  }

  internal static PythonTuple MakeTuple(params object[] items)
  {
    return items.Length == 0 ? PythonTuple.EMPTY : new PythonTuple(items);
  }

  private static object[] MakeItems(object o)
  {
    switch (o)
    {
      case PythonTuple _:
        return ((PythonTuple) o)._data;
      case string _:
        string str = (string) o;
        object[] objArray = new object[str.Length];
        for (int index = 0; index < objArray.Length; ++index)
          objArray[index] = (object) ScriptingRuntimeHelpers.CharToString(str[index]);
        return objArray;
      case List _:
        return ((List) o).GetObjectArray();
      case object[] data:
        return ArrayOps.CopyArray(data, data.Length);
      default:
        List<object> objectList = new List<object>();
        IEnumerator enumerator = PythonOps.GetEnumerator(o);
        while (enumerator.MoveNext())
          objectList.Add(enumerator.Current);
        return objectList.ToArray();
    }
  }

  internal object[] ToArray() => ArrayOps.CopyArray(this._data, this._data.Length);

  public virtual int __len__() => this._data.Length;

  public virtual object this[int index] => this._data[PythonOps.FixIndex(index, this._data.Length)];

  public virtual object this[object index] => this[Converter.ConvertToIndex(index)];

  public virtual object this[BigInteger index] => this[(int) index];

  public virtual object __getslice__(int start, int stop)
  {
    Slice.FixSliceArguments(this._data.Length, ref start, ref stop);
    return start == 0 && stop == this._data.Length && this.GetType() == typeof (PythonTuple) ? (object) this : (object) PythonTuple.MakeTuple(ArrayOps.GetSlice(this._data, start, stop));
  }

  public virtual object this[Slice slice]
  {
    get
    {
      int ostart;
      int ostop;
      int ostep;
      slice.indices(this._data.Length, out ostart, out ostop, out ostep);
      return ostart == 0 && ostop == this._data.Length && ostep == 1 && this.GetType() == typeof (PythonTuple) ? (object) this : (object) PythonTuple.MakeTuple(ArrayOps.GetSlice(this._data, ostart, ostop, ostep));
    }
  }

  public static PythonTuple operator +([NotNull] PythonTuple x, [NotNull] PythonTuple y)
  {
    return PythonTuple.MakeTuple(ArrayOps.Add(x._data, x._data.Length, y._data, y._data.Length));
  }

  private static PythonTuple MultiplyWorker(PythonTuple self, int count)
  {
    if (count <= 0)
      return PythonTuple.EMPTY;
    return count == 1 && self.GetType() == typeof (PythonTuple) ? self : PythonTuple.MakeTuple(ArrayOps.Multiply(self._data, self._data.Length, count));
  }

  public static PythonTuple operator *(PythonTuple x, int n) => PythonTuple.MultiplyWorker(x, n);

  public static PythonTuple operator *(int n, PythonTuple x) => PythonTuple.MultiplyWorker(x, n);

  public static object operator *([NotNull] PythonTuple self, [NotNull] Index count)
  {
    return PythonOps.MultiplySequence<PythonTuple>(new PythonOps.MultiplySequenceWorker<PythonTuple>(PythonTuple.MultiplyWorker), self, count, true);
  }

  public static object operator *([NotNull] Index count, [NotNull] PythonTuple self)
  {
    return PythonOps.MultiplySequence<PythonTuple>(new PythonOps.MultiplySequenceWorker<PythonTuple>(PythonTuple.MultiplyWorker), self, count, false);
  }

  public static object operator *([NotNull] PythonTuple self, object count)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) (self * index);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  public static object operator *(object count, [NotNull] PythonTuple self)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) (index * self);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  bool ICollection.IsSynchronized => false;

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._data.Length;
  }

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(Array array, int index)
  {
    Array.Copy((Array) this._data, 0, array, index, this._data.Length);
  }

  object ICollection.SyncRoot => (object) this;

  public virtual IEnumerator __iter__() => (IEnumerator) new TupleEnumerator(this);

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator GetEnumerator() => this.__iter__();

  private object[] Expand(object value)
  {
    int length = this._data.Length;
    object[] objArray = value != null ? new object[length + 1] : new object[length];
    for (int index = 0; index < length; ++index)
      objArray[index] = this._data[index];
    if (value != null)
      objArray[length] = value;
    return objArray;
  }

  public object __getnewargs__()
  {
    return (object) PythonTuple.MakeTuple((object) new PythonTuple((object) this));
  }

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new TupleEnumerator(this);
  }

  [PythonHidden(new PlatformID[] {})]
  public int IndexOf(object item)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      if (PythonOps.EqualRetBool(this[index], item))
        return index;
    }
    return -1;
  }

  void IList<object>.Insert(int index, object item)
  {
    throw new InvalidOperationException("Tuple is readonly");
  }

  void IList<object>.RemoveAt(int index)
  {
    throw new InvalidOperationException("Tuple is readonly");
  }

  object IList<object>.this[int index]
  {
    get => this[index];
    set => throw new InvalidOperationException("Tuple is readonly");
  }

  void ICollection<object>.Add(object item)
  {
    throw new InvalidOperationException("Tuple is readonly");
  }

  void ICollection<object>.Clear() => throw new InvalidOperationException("Tuple is readonly");

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(object item)
  {
    for (int index = 0; index < this._data.Length; ++index)
    {
      if (PythonOps.EqualRetBool(this._data[index], item))
        return true;
    }
    return false;
  }

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(object[] array, int arrayIndex)
  {
    for (int index = 0; index < this.Count; ++index)
      array[arrayIndex + index] = this[index];
  }

  bool ICollection<object>.IsReadOnly => true;

  bool ICollection<object>.Remove(object item)
  {
    throw new InvalidOperationException("Tuple is readonly");
  }

  internal int CompareTo(PythonTuple other)
  {
    return PythonOps.CompareArrays(this._data, this._data.Length, other._data, other._data.Length);
  }

  public static bool operator >([NotNull] PythonTuple self, [NotNull] PythonTuple other)
  {
    return self.CompareTo(other) > 0;
  }

  public static bool operator <([NotNull] PythonTuple self, [NotNull] PythonTuple other)
  {
    return self.CompareTo(other) < 0;
  }

  public static bool operator >=([NotNull] PythonTuple self, [NotNull] PythonTuple other)
  {
    return self.CompareTo(other) >= 0;
  }

  public static bool operator <=([NotNull] PythonTuple self, [NotNull] PythonTuple other)
  {
    return self.CompareTo(other) <= 0;
  }

  int IStructuralComparable.CompareTo(object obj, IComparer comparer)
  {
    if (!(obj is PythonTuple pythonTuple))
      throw new ValueErrorException("expected tuple");
    return PythonOps.CompareArrays(this._data, this._data.Length, pythonTuple._data, pythonTuple._data.Length, comparer);
  }

  public override bool Equals(object obj)
  {
    if (this != obj)
    {
      if (!(obj is PythonTuple pythonTuple) || this._data.Length != pythonTuple._data.Length)
        return false;
      for (int index = 0; index < this._data.Length; ++index)
      {
        object obj1 = this[index];
        object obj2 = pythonTuple[index];
        if (obj1 != obj2 && (obj1 == null || !obj1.Equals(obj2)))
          return false;
      }
    }
    return true;
  }

  public override int GetHashCode()
  {
    int num1 = 6551;
    int num2 = num1;
    for (int index = 0; index < this._data.Length; index += 2)
    {
      num1 = (num1 << 27) + (num2 + 1 << 1) + (num1 >> 5) ^ (this._data[index] == null ? 505032256 : this._data[index].GetHashCode());
      if (index != this._data.Length - 1)
        num2 = (num2 << 5) + (num1 - 1 >> 1) + (num2 >> 27) ^ (this._data[index + 1] == null ? 505032256 : this._data[index + 1].GetHashCode());
      else
        break;
    }
    return num1 + num2 * 1566083941;
  }

  private int GetHashCode(HashDelegate dlg)
  {
    int num1 = 6551;
    int num2 = num1;
    for (int index = 0; index < this._data.Length; index += 2)
    {
      num1 = (num1 << 27) + (num2 + 1 << 1) + (num1 >> 5) ^ dlg(this._data[index], ref dlg);
      if (index != this._data.Length - 1)
        num2 = (num2 << 5) + (num1 - 1 >> 1) + (num2 >> 27) ^ dlg(this._data[index + 1], ref dlg);
      else
        break;
    }
    return num1 + num2 * 1566083941;
  }

  private int GetHashCode(IEqualityComparer comparer)
  {
    int num1 = 6551;
    int num2 = num1;
    for (int index = 0; index < this._data.Length; index += 2)
    {
      num1 = (num1 << 27) + (num2 + 1 << 1) + (num1 >> 5) ^ comparer.GetHashCode(this._data[index]);
      if (index != this._data.Length - 1)
        num2 = (num2 << 5) + (num1 - 1 >> 1) + (num2 >> 27) ^ comparer.GetHashCode(this._data[index + 1]);
      else
        break;
    }
    return num1 + num2 * 1566083941;
  }

  public override string ToString() => this.__repr__(DefaultContext.Default);

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    return comparer is PythonContext.PythonEqualityComparer equalityComparer ? this.GetHashCode(equalityComparer.Context.InitialHasher) : this.GetHashCode(comparer);
  }

  bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
  {
    if (other != this)
    {
      if (!(other is PythonTuple pythonTuple) || this._data.Length != pythonTuple._data.Length)
        return false;
      for (int index = 0; index < this._data.Length; ++index)
      {
        object x = this._data[index];
        object y = pythonTuple._data[index];
        if (x != y && !comparer.Equals(x, y))
          return false;
      }
    }
    return true;
  }

  public virtual string __repr__(CodeContext context)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("(");
    for (int index = 0; index < this._data.Length; ++index)
    {
      if (index > 0)
        stringBuilder.Append(", ");
      stringBuilder.Append(PythonOps.Repr(context, this._data[index]));
    }
    if (this._data.Length == 1)
      stringBuilder.Append(",");
    stringBuilder.Append(")");
    return stringBuilder.ToString();
  }

  int IList.Add(object value) => throw new InvalidOperationException("Tuple is readonly");

  void IList.Clear() => throw new InvalidOperationException("Tuple is readonly");

  void IList.Insert(int index, object value)
  {
    throw new InvalidOperationException("Tuple is readonly");
  }

  bool IList.IsFixedSize => true;

  bool IList.IsReadOnly => true;

  void IList.Remove(object value) => throw new InvalidOperationException("Tuple is readonly");

  void IList.RemoveAt(int index) => throw new InvalidOperationException("Tuple is readonly");

  object IList.this[int index]
  {
    get => this[index];
    set => throw new InvalidOperationException("Tuple is readonly");
  }

  public System.Linq.Expressions.Expression CreateExpression()
  {
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[this.Count];
    for (int index = 0; index < expressionArray.Length; ++index)
      expressionArray[index] = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(Microsoft.Scripting.Ast.Utils.Constant(this[index]), typeof (object));
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeTuple, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), expressionArray));
  }
}
