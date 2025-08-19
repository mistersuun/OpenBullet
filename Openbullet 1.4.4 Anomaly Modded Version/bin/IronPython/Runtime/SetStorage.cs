// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SetStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal sealed class SetStorage : 
  IEnumerable,
  IEnumerable<object>,
  ISerializable,
  IDeserializationCallback
{
  internal SetStorage.Bucket[] _buckets;
  internal int _count;
  private int _version;
  internal bool _hasNull;
  private Func<object, int> _hashFunc;
  private Func<object, object, bool> _eqFunc;
  private Type _itemType;
  private int _maxCount;
  private const int InitialBuckets = 8;
  private const double Load = 0.7;
  private const double MinLoad = 0.5;
  private static readonly Type HeterogeneousType = typeof (SetStorage);
  internal static readonly object Removed = new object();
  private static readonly Func<object, int> _primitiveHash = new Func<object, int>(SetStorage.PrimitiveHash);
  private static readonly Func<object, int> _intHash = new Func<object, int>(SetStorage.IntHash);
  private static readonly Func<object, int> _doubleHash = new Func<object, int>(SetStorage.DoubleHash);
  private static readonly Func<object, int> _tupleHash = new Func<object, int>(SetStorage.TupleHash);
  private static readonly Func<object, int> _genericHash = new Func<object, int>(SetStorage.GenericHash);
  private static readonly Func<object, object, bool> _stringEquals = new Func<object, object, bool>(SetStorage.StringEquals);
  private static readonly Func<object, object, bool> _intEquals = new Func<object, object, bool>(SetStorage.IntEquals);
  private static readonly Func<object, object, bool> _doubleEquals = new Func<object, object, bool>(SetStorage.DoubleEquals);
  private static readonly Func<object, object, bool> _tupleEquals = new Func<object, object, bool>(SetStorage.TupleEquals);
  private static readonly Func<object, object, bool> _genericEquals = new Func<object, object, bool>(SetStorage.GenericEquals);
  private static readonly Func<object, object, bool> _objectEquals = new Func<object, object, bool>(object.ReferenceEquals);

  public SetStorage()
  {
  }

  public SetStorage(int count) => this.Initialize(count);

  private SetStorage(SerializationInfo info, StreamingContext context)
  {
    this._buckets = new SetStorage.Bucket[1]
    {
      new SetStorage.Bucket(0, (object) info)
    };
  }

  private void Initialize()
  {
    this._maxCount = 5;
    this._buckets = new SetStorage.Bucket[8];
  }

  private void Initialize(int count)
  {
    int length = 1 << SetStorage.CeilLog2(Math.Max((int) ((double) count / 0.7) + 1, 8));
    this._maxCount = (int) ((double) length * 0.7);
    this._buckets = new SetStorage.Bucket[length];
  }

  public int Count
  {
    get
    {
      int count = this._count;
      if (this._hasNull)
        ++count;
      return count;
    }
  }

  public int Version => this._version;

  public void Add(object item)
  {
    lock (this)
      this.AddNoLock(item);
  }

  public void AddNoLock(object item)
  {
    if (item != null)
    {
      if (this._buckets == null)
        this.Initialize();
      if (item.GetType() != this._itemType && this._itemType != SetStorage.HeterogeneousType)
        this.UpdateHelperFunctions(item.GetType(), item);
      this.AddWorker(item, this.Hash(item));
    }
    else
      this._hasNull = true;
  }

  private void AddWorker(object item, int hashCode)
  {
    if (!SetStorage.AddWorker(this._buckets, item, hashCode, this._eqFunc, ref this._version))
      return;
    ++this._count;
    if (this._count <= this._maxCount)
      return;
    this.Grow();
  }

  private static bool AddWorker(
    SetStorage.Bucket[] buckets,
    object item,
    int hashCode,
    Func<object, object, bool> eqFunc,
    ref int version)
  {
    int index1 = -1;
    int num = 0;
    int index2 = hashCode & buckets.Length - 1;
    while (num < buckets.Length)
    {
      SetStorage.Bucket bucket = buckets[index2];
      if (bucket.Item == null)
      {
        ++version;
        if (index1 != -1)
          index2 = index1;
        buckets[index2].HashCode = hashCode;
        buckets[index2].Item = item;
        return true;
      }
      if (bucket.Item == SetStorage.Removed && index1 == -1)
        index1 = index2;
      else if (bucket.Item != SetStorage.Removed && bucket.HashCode == hashCode && eqFunc(item, bucket.Item))
        return false;
      ++num;
      SetStorage.ProbeNext(buckets, ref index2);
    }
    ++version;
    buckets[index1].HashCode = hashCode;
    buckets[index1].Item = item;
    return true;
  }

  private void AddOrRemoveWorker(object item, int hashCode)
  {
    int index = hashCode & this._buckets.Length - 1;
    while (true)
    {
      SetStorage.Bucket bucket = this._buckets[index];
      if (bucket.Item != null)
      {
        if (bucket.Item == SetStorage.Removed || bucket.HashCode != hashCode || !this._eqFunc(item, bucket.Item))
          SetStorage.ProbeNext(this._buckets, ref index);
        else
          goto label_5;
      }
      else
        break;
    }
    ++this._version;
    this._buckets[index].HashCode = hashCode;
    this._buckets[index].Item = item;
    ++this._count;
    if (this._count <= this._maxCount)
      return;
    this.Grow();
    return;
label_5:
    ++this._version;
    this._buckets[index].Item = SetStorage.Removed;
    --this._count;
  }

  public void Clear()
  {
    lock (this)
      this.ClearNoLock();
  }

  public void ClearNoLock()
  {
    if (this._buckets != null)
    {
      ++this._version;
      this.Initialize();
      this._count = 0;
    }
    this._hasNull = false;
  }

  public SetStorage Clone()
  {
    SetStorage setStorage = new SetStorage();
    setStorage._hasNull = this._hasNull;
    if (this._count == 0)
      return setStorage;
    SetStorage.Bucket[] buckets = this._buckets;
    setStorage._hashFunc = this._hashFunc;
    setStorage._eqFunc = this._eqFunc;
    setStorage._itemType = this._itemType;
    if ((double) this._count < (double) this._buckets.Length * 0.5)
    {
      setStorage.Initialize(this._count);
      for (int index = 0; index < buckets.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed)
          setStorage.AddWorker(bucket.Item, bucket.HashCode);
      }
    }
    else
    {
      setStorage._maxCount = (int) ((double) buckets.Length * 0.7);
      setStorage._buckets = new SetStorage.Bucket[buckets.Length];
      for (int index = 0; index < buckets.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets[index];
        if (bucket.Item != null)
        {
          setStorage._buckets[index].Item = bucket.Item;
          setStorage._buckets[index].HashCode = bucket.HashCode;
          if (bucket.Item != SetStorage.Removed)
            ++setStorage._count;
        }
      }
    }
    return setStorage;
  }

  public bool Contains(object item)
  {
    if (item == null)
      return this._hasNull;
    if (this._count == 0)
      return false;
    int hashCode;
    Func<object, object, bool> eqFunc;
    if (item.GetType() == this._itemType || this._itemType == SetStorage.HeterogeneousType)
    {
      hashCode = this._hashFunc(item);
      eqFunc = this._eqFunc;
    }
    else
    {
      hashCode = SetStorage._genericHash(item);
      eqFunc = SetStorage._genericEquals;
    }
    return SetStorage.ContainsWorker(this._buckets, item, hashCode, eqFunc);
  }

  public bool ContainsAlwaysHash(object item)
  {
    if (item == null)
      return this._hasNull;
    int hashCode;
    Func<object, object, bool> eqFunc;
    if (item.GetType() == this._itemType || this._itemType == SetStorage.HeterogeneousType)
    {
      hashCode = this._hashFunc(item);
      eqFunc = this._eqFunc;
    }
    else
    {
      hashCode = SetStorage._genericHash(item);
      eqFunc = SetStorage._genericEquals;
    }
    return this._count > 0 && SetStorage.ContainsWorker(this._buckets, item, hashCode, eqFunc);
  }

  private static bool ContainsWorker(
    SetStorage.Bucket[] buckets,
    object item,
    int hashCode,
    Func<object, object, bool> eqFunc)
  {
    int index = hashCode & buckets.Length - 1;
    int num = index;
    do
    {
      SetStorage.Bucket bucket = buckets[index];
      if (bucket.Item != null)
      {
        if (bucket.Item != SetStorage.Removed && bucket.HashCode == hashCode && eqFunc(item, bucket.Item))
          return true;
        SetStorage.ProbeNext(buckets, ref index);
      }
      else
        break;
    }
    while (num != index);
    return false;
  }

  public void CopyTo(SetStorage into)
  {
    lock (into)
      into.UnionUpdate(this);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public IEnumerator<object> GetEnumerator()
  {
    if (this._hasNull)
      yield return (object) null;
    if (this._count != 0)
    {
      foreach (SetStorage.Bucket bucket in this._buckets)
      {
        object obj = bucket.Item;
        if (obj != null && obj != SetStorage.Removed)
          yield return obj;
      }
    }
  }

  public List GetItems()
  {
    List items = new List(this.Count);
    if (this._hasNull)
      items.AddNoLock((object) null);
    if (this._count > 0)
    {
      foreach (SetStorage.Bucket bucket in this._buckets)
      {
        object obj = bucket.Item;
        if (obj != null && obj != SetStorage.Removed)
          items.AddNoLock(obj);
      }
    }
    return items;
  }

  public bool Pop(out object item)
  {
    item = (object) null;
    if (this._hasNull)
    {
      this._hasNull = false;
      return true;
    }
    if (this._count == 0)
      return false;
    lock (this)
    {
      for (int index = 0; index < this._buckets.Length; ++index)
      {
        if (this._buckets[index].Item != null && this._buckets[index].Item != SetStorage.Removed)
        {
          item = this._buckets[index].Item;
          ++this._version;
          this._buckets[index].Item = SetStorage.Removed;
          --this._count;
          return true;
        }
      }
      item = (object) null;
      return false;
    }
  }

  public bool Remove(object item)
  {
    lock (this)
      return this.RemoveNoLock(item);
  }

  public bool RemoveNoLock(object item)
  {
    if (item == null)
      return this.RemoveNull();
    return this._count != 0 && this.RemoveItem(item);
  }

  internal bool RemoveAlwaysHash(object item)
  {
    lock (this)
      return item == null ? this.RemoveNull() : this.RemoveItem(item);
  }

  private bool RemoveNull()
  {
    if (!this._hasNull)
      return false;
    this._hasNull = false;
    return true;
  }

  private bool RemoveItem(object item)
  {
    int hashCode;
    Func<object, object, bool> eqFunc;
    if (item.GetType() == this._itemType || this._itemType == SetStorage.HeterogeneousType)
    {
      hashCode = this._hashFunc(item);
      eqFunc = this._eqFunc;
    }
    else
    {
      hashCode = SetStorage._genericHash(item);
      eqFunc = SetStorage._genericEquals;
    }
    return this.RemoveWorker(item, hashCode, eqFunc);
  }

  private bool RemoveWorker(object item, int hashCode, Func<object, object, bool> eqFunc)
  {
    if (this._count == 0)
      return false;
    int index = hashCode & this._buckets.Length - 1;
    int num = index;
    do
    {
      SetStorage.Bucket bucket = this._buckets[index];
      if (bucket.Item != null)
      {
        if (bucket.Item != SetStorage.Removed && bucket.HashCode == hashCode && eqFunc(item, bucket.Item))
        {
          ++this._version;
          this._buckets[index].Item = SetStorage.Removed;
          --this._count;
          return true;
        }
        SetStorage.ProbeNext(this._buckets, ref index);
      }
      else
        break;
    }
    while (index != num);
    return false;
  }

  public bool IsDisjoint(SetStorage other) => SetStorage.IsDisjoint(this, other);

  public static bool IsDisjoint(SetStorage self, SetStorage other)
  {
    SetStorage.SortBySize(ref self, ref other);
    if (self._hasNull && other._hasNull)
      return false;
    if (self._count == 0 || other._count == 0)
      return true;
    SetStorage.Bucket[] buckets1 = self._buckets;
    SetStorage.Bucket[] buckets2 = other._buckets;
    Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(self, other);
    for (int index = 0; index < buckets1.Length; ++index)
    {
      SetStorage.Bucket bucket = buckets1[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed && SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
        return false;
    }
    return true;
  }

  public bool IsSubset(SetStorage other)
  {
    return this._count <= other._count && (!this._hasNull || other._hasNull) && this.IsSubsetWorker(other);
  }

  public bool IsStrictSubset(SetStorage other)
  {
    return this._count <= other._count && (!this._hasNull || other._hasNull) && this.Count != other.Count && this.IsSubsetWorker(other);
  }

  private bool IsSubsetWorker(SetStorage other)
  {
    if (this._count == 0)
      return true;
    if (other._count == 0)
      return false;
    SetStorage.Bucket[] buckets1 = this._buckets;
    SetStorage.Bucket[] buckets2 = other._buckets;
    Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(this, other);
    for (int index = 0; index < buckets1.Length; ++index)
    {
      SetStorage.Bucket bucket = buckets1[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed && !SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
        return false;
    }
    return true;
  }

  public void UnionUpdate(SetStorage other)
  {
    this._hasNull |= other._hasNull;
    if (other._count == 0)
      return;
    if (this._buckets == null)
      this.Initialize(other._count);
    SetStorage.Bucket[] buckets = other._buckets;
    this.UpdateHelperFunctions(other);
    for (int index = 0; index < buckets.Length; ++index)
    {
      SetStorage.Bucket bucket = buckets[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed)
        this.AddWorker(bucket.Item, bucket.HashCode);
    }
  }

  public void IntersectionUpdate(SetStorage other)
  {
    if (other._count == 0)
    {
      this.ClearNoLock();
      this._hasNull &= other._hasNull;
    }
    else
    {
      this._hasNull &= other._hasNull;
      if (this._count == 0)
        return;
      SetStorage.Bucket[] buckets1 = this._buckets;
      SetStorage.Bucket[] buckets2 = other._buckets;
      Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(this, other);
      for (int index = 0; index < buckets1.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets1[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed && !SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
        {
          ++this._version;
          buckets1[index].Item = SetStorage.Removed;
          --this._count;
        }
      }
    }
  }

  public void SymmetricDifferenceUpdate(SetStorage other)
  {
    this._hasNull ^= other._hasNull;
    if (other._count == 0)
      return;
    if (this._buckets == null)
      this.Initialize();
    SetStorage.Bucket[] buckets = other._buckets;
    this.UpdateHelperFunctions(other);
    for (int index = 0; index < buckets.Length; ++index)
    {
      SetStorage.Bucket bucket = buckets[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed)
        this.AddOrRemoveWorker(bucket.Item, bucket.HashCode);
    }
  }

  public void DifferenceUpdate(SetStorage other)
  {
    this._hasNull &= !other._hasNull;
    if (this._count == 0 || other._count == 0)
      return;
    SetStorage.Bucket[] buckets1 = this._buckets;
    SetStorage.Bucket[] buckets2 = other._buckets;
    Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(this, other);
    if (buckets1.Length < buckets2.Length)
    {
      for (int index = 0; index < buckets1.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets1[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed && SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
          this.RemoveWorker(bucket.Item, bucket.HashCode, eqFunc);
      }
    }
    else
    {
      for (int index = 0; index < buckets2.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets2[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed)
          this.RemoveWorker(bucket.Item, bucket.HashCode, eqFunc);
      }
    }
  }

  public static SetStorage Union(SetStorage self, SetStorage other)
  {
    SetStorage setStorage;
    if (self._count < other._count)
    {
      setStorage = other.Clone();
      setStorage.UnionUpdate(self);
    }
    else
    {
      setStorage = self.Clone();
      setStorage.UnionUpdate(other);
    }
    return setStorage;
  }

  public static SetStorage Intersection(SetStorage self, SetStorage other)
  {
    SetStorage setStorage = new SetStorage(Math.Min(self._count, other._count));
    setStorage._hasNull = self._hasNull && other._hasNull;
    if (self._count == 0 || other._count == 0)
      return setStorage;
    SetStorage.SortBySize(ref self, ref other);
    SetStorage.Bucket[] buckets1 = self._buckets;
    SetStorage.Bucket[] buckets2 = other._buckets;
    Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(self, other);
    if (other._itemType != SetStorage.HeterogeneousType)
      setStorage.UpdateHelperFunctions(other);
    else
      setStorage.UpdateHelperFunctions(self);
    for (int index = 0; index < buckets1.Length; ++index)
    {
      SetStorage.Bucket bucket = buckets1[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed && SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
        setStorage.AddWorker(bucket.Item, bucket.HashCode);
    }
    return setStorage;
  }

  public static SetStorage SymmetricDifference(SetStorage self, SetStorage other)
  {
    SetStorage.SortBySize(ref self, ref other);
    SetStorage setStorage = other.Clone();
    setStorage.SymmetricDifferenceUpdate(self);
    return setStorage;
  }

  public static SetStorage Difference(SetStorage self, SetStorage other)
  {
    if (self._count == 0 || other._count == 0)
    {
      SetStorage setStorage = self.Clone();
      setStorage._hasNull &= !other._hasNull;
      return setStorage;
    }
    SetStorage setStorage1;
    if (self._buckets.Length <= other._buckets.Length)
    {
      setStorage1 = new SetStorage(self._count);
      setStorage1._hasNull &= !other._hasNull;
      SetStorage.Bucket[] buckets1 = self._buckets;
      SetStorage.Bucket[] buckets2 = other._buckets;
      Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(self, other);
      setStorage1.UpdateHelperFunctions(self);
      for (int index = 0; index < buckets1.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets1[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed && !SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
          setStorage1.AddWorker(bucket.Item, bucket.HashCode);
      }
    }
    else
    {
      setStorage1 = self.Clone();
      setStorage1.DifferenceUpdate(other);
    }
    return setStorage1;
  }

  public static bool Equals(SetStorage x, SetStorage y, IEqualityComparer comparer)
  {
    if (x == y)
      return true;
    if (x._count != y._count || x._hasNull ^ y._hasNull)
      return false;
    if (x._count == 0)
      return true;
    SetStorage.SortBySize(ref x, ref y);
    if (comparer is PythonContext.PythonEqualityComparer)
    {
      SetStorage.Bucket[] buckets1 = x._buckets;
      SetStorage.Bucket[] buckets2 = y._buckets;
      Func<object, object, bool> eqFunc = SetStorage.GetEqFunc(x, y);
      for (int index = 0; index < buckets1.Length; ++index)
      {
        SetStorage.Bucket bucket = buckets1[index];
        if (bucket.Item != null && bucket.Item != SetStorage.Removed && !SetStorage.ContainsWorker(buckets2, bucket.Item, bucket.HashCode, eqFunc))
          return false;
      }
      return true;
    }
    SetStorage setStorage = new SetStorage();
    setStorage._itemType = SetStorage.HeterogeneousType;
    setStorage._eqFunc = new Func<object, object, bool>(comparer.Equals);
    setStorage._hashFunc = new Func<object, int>(comparer.GetHashCode);
    foreach (object obj in y)
      setStorage.AddNoLock(obj);
    foreach (object obj in x)
    {
      if (!setStorage.RemoveNoLock(obj))
        return false;
    }
    return setStorage._count == 0;
  }

  public static int GetHashCode(SetStorage set, IEqualityComparer comparer)
  {
    int num1 = 1420601183;
    int num2 = 674132117;
    int num3 = 393601577;
    if (set._count > 0)
    {
      int num4 = num1 ^ set._count * 8803;
      num1 = num4 << 10 ^ num4 >> 22;
      int num5 = num2 + set._count * 5179;
      num2 = num5 << 10 ^ num5 >> 22;
      int num6 = num3 * set._count + 784251623;
      num3 = num6 << 10 ^ num6 >> 22;
    }
    if (comparer is PythonContext.PythonEqualityComparer)
    {
      if (set._hasNull)
      {
        num1 = num1 << 7 ^ num1 >> 25 ^ 505032256;
        num2 = (num2 << 7 ^ num2 >> 25) + 505032256;
        num3 = (num3 << 7 ^ num3 >> 25) * 505032256;
      }
      if (set._count > 0)
      {
        SetStorage.Bucket[] buckets = set._buckets;
        for (int index = 0; index < buckets.Length; ++index)
        {
          object obj = buckets[index].Item;
          if (obj != null && obj != SetStorage.Removed)
          {
            int hashCode = buckets[index].HashCode;
            num1 ^= hashCode;
            num2 += hashCode;
            num3 *= hashCode;
          }
        }
      }
    }
    else
    {
      if (set._hasNull)
      {
        int hashCode = comparer.GetHashCode((object) null);
        num1 = num1 + (num1 << 7 ^ num1 >> 25) ^ hashCode;
        num2 = (num2 << 7 ^ num2 >> 25) + hashCode;
        num3 = (num3 << 7 ^ num3 >> 25) * hashCode;
      }
      if (set._count > 0)
      {
        foreach (SetStorage.Bucket bucket in set._buckets)
        {
          object obj = bucket.Item;
          if (obj != null && obj != SetStorage.Removed)
          {
            int hashCode = comparer.GetHashCode(obj);
            num1 ^= hashCode;
            num2 += hashCode;
            num3 *= hashCode;
          }
        }
      }
    }
    int num7 = num1 << 11 ^ num1 >> 21 ^ num2;
    int num8 = num7 << 27 ^ num7 >> 5 ^ num3;
    return num8 << 9 ^ num8 >> 23 ^ 2001081521;
  }

  private static int PrimitiveHash(object o) => o.GetHashCode();

  private static int IntHash(object o) => (int) o;

  private static int DoubleHash(object o) => DoubleOps.__hash__((double) o);

  private static int TupleHash(object o)
  {
    return ((IStructuralEquatable) o).GetHashCode(DefaultContext.DefaultPythonContext.EqualityComparerNonGeneric);
  }

  private static int GenericHash(object o) => PythonOps.Hash(DefaultContext.Default, o);

  private static bool StringEquals(object o1, object o2) => (string) o1 == (string) o2;

  private static bool IntEquals(object o1, object o2) => (int) o1 == (int) o2;

  private static bool DoubleEquals(object o1, object o2) => (double) o1 == (double) o2;

  private static bool TupleEquals(object o1, object o2)
  {
    return ((IStructuralEquatable) o1).Equals(o2, DefaultContext.DefaultPythonContext.EqualityComparerNonGeneric);
  }

  private static bool GenericEquals(object o1, object o2)
  {
    return o1 == o2 || PythonOps.EqualRetBool(o1, o2);
  }

  private void UpdateHelperFunctions(SetStorage other)
  {
    if (this._itemType == SetStorage.HeterogeneousType || this._itemType == other._itemType)
      return;
    if (other._itemType == SetStorage.HeterogeneousType)
      this.SetHeterogeneousSites();
    else if (this._itemType == (Type) null)
    {
      this._hashFunc = other._hashFunc;
      this._eqFunc = other._eqFunc;
      this._itemType = other._itemType;
    }
    else
      this.SetHeterogeneousSites();
  }

  private void UpdateHelperFunctions(Type t, object item)
  {
    if (this._itemType == (Type) null)
    {
      if (t == typeof (int))
      {
        this._hashFunc = SetStorage._intHash;
        this._eqFunc = SetStorage._intEquals;
      }
      else if (t == typeof (string))
      {
        this._hashFunc = SetStorage._primitiveHash;
        this._eqFunc = SetStorage._stringEquals;
      }
      else if (t == typeof (double))
      {
        this._hashFunc = SetStorage._doubleHash;
        this._eqFunc = SetStorage._doubleEquals;
      }
      else if (t == typeof (PythonTuple))
      {
        this._hashFunc = SetStorage._tupleHash;
        this._eqFunc = SetStorage._tupleEquals;
      }
      else if (t == typeof (Type).GetType())
      {
        this._hashFunc = SetStorage._primitiveHash;
        this._eqFunc = SetStorage._objectEquals;
      }
      else
      {
        PythonType pythonType = DynamicHelpers.GetPythonType(item);
        this.AssignSiteDelegates(PythonContext.GetHashSite(pythonType), DefaultContext.DefaultPythonContext.GetEqualSite((Type) pythonType));
      }
      this._itemType = t;
    }
    else
    {
      if (!(this._itemType != SetStorage.HeterogeneousType))
        return;
      this.SetHeterogeneousSites();
    }
  }

  private void SetHeterogeneousSites()
  {
    this._buckets = (SetStorage.Bucket[]) this._buckets.Clone();
    this.AssignSiteDelegates(DefaultContext.DefaultPythonContext.MakeHashSite(), DefaultContext.DefaultPythonContext.MakeEqualSite());
    this._itemType = SetStorage.HeterogeneousType;
  }

  private void AssignSiteDelegates(
    CallSite<Func<CallSite, object, int>> hashSite,
    CallSite<Func<CallSite, object, object, bool>> equalSite)
  {
    this._hashFunc = (Func<object, int>) (o => hashSite.Target((CallSite) hashSite, o));
    this._eqFunc = (Func<object, object, bool>) ((o0, o1) => equalSite.Target((CallSite) equalSite, o0, o1));
  }

  private int Hash(object item) => item is string ? item.GetHashCode() : this._hashFunc(item);

  private static Func<object, object, bool> GetEqFunc(SetStorage self, SetStorage other)
  {
    if (self._itemType == other._itemType || self._itemType == SetStorage.HeterogeneousType)
      return self._eqFunc;
    return other._itemType == SetStorage.HeterogeneousType ? other._eqFunc : SetStorage._genericEquals;
  }

  internal static void SortBySize(ref SetStorage x, ref SetStorage y)
  {
    if (x._count <= 0 || (y._count <= 0 || x._buckets.Length <= y._buckets.Length) && y._count != 0)
      return;
    SetStorage setStorage = x;
    x = y;
    y = setStorage;
  }

  internal static SetStorage GetItems(object set)
  {
    SetStorage items;
    return SetStorage.GetItemsIfSet(set, out items) ? items : SetStorage.GetItemsWorker(set);
  }

  internal static bool GetItems(object set, out SetStorage items)
  {
    if (SetStorage.GetItemsIfSet(set, out items))
      return true;
    items = SetStorage.GetItemsWorker(set);
    return false;
  }

  internal static SetStorage GetFrozenItems(object o)
  {
    switch (o)
    {
      case FrozenSetCollection frozenSetCollection:
        return frozenSetCollection._items;
      case SetCollection setCollection:
        return setCollection._items.Clone();
      default:
        return SetStorage.GetItemsWorker(o);
    }
  }

  internal static SetStorage GetItemsWorker(object set)
  {
    return SetStorage.GetItemsWorker(PythonOps.GetEnumerator(set));
  }

  internal static SetStorage GetItemsWorker(IEnumerator en)
  {
    SetStorage itemsWorker = new SetStorage();
    while (en.MoveNext())
      itemsWorker.AddNoLock(en.Current);
    return itemsWorker;
  }

  public static bool GetItemsIfSet(object o, out SetStorage items)
  {
    switch (o)
    {
      case FrozenSetCollection frozenSetCollection:
        items = frozenSetCollection._items;
        return true;
      case SetCollection setCollection:
        items = setCollection._items;
        return true;
      default:
        items = (SetStorage) null;
        return false;
    }
  }

  internal static bool GetHashableSetIfSet(ref object o)
  {
    if (!(o is SetCollection set))
      return o is FrozenSetCollection;
    if (SetStorage.IsHashable(set))
      return true;
    o = (object) new FrozenSetCollection(set._items.Clone());
    return true;
  }

  private static bool IsHashable(SetCollection set)
  {
    if (set.GetType() == typeof (SetCollection))
      return false;
    PythonType pythonType = DynamicHelpers.GetPythonType((object) set);
    PythonTypeSlot slot;
    object obj;
    return pythonType.TryResolveSlot(DefaultContext.Default, "__hash__", out slot) && slot.TryGetValue(DefaultContext.Default, (object) set, pythonType, out obj) && obj != null;
  }

  internal static PythonTuple Reduce(SetStorage items, PythonType type)
  {
    PythonTuple pythonTuple = PythonTuple.MakeTuple((object) items.GetItems());
    return PythonTuple.MakeTuple((object) type, (object) pythonTuple, null);
  }

  internal static string SetToString(CodeContext context, object set, SetStorage items)
  {
    Type type = set.GetType();
    string str1 = !(type == typeof (SetCollection)) ? (!(type == typeof (FrozenSetCollection)) ? PythonTypeOps.GetName(set) : "frozenset") : nameof (set);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(str1);
    stringBuilder.Append("([");
    string str2 = "";
    if (items._hasNull)
    {
      stringBuilder.Append(str2);
      stringBuilder.Append(PythonOps.Repr(context, (object) null));
      str2 = ", ";
    }
    if (items._count > 0)
    {
      foreach (SetStorage.Bucket bucket in items._buckets)
      {
        if (bucket.Item != null && bucket.Item != SetStorage.Removed)
        {
          stringBuilder.Append(str2);
          stringBuilder.Append(PythonOps.Repr(context, bucket.Item));
          str2 = ", ";
        }
      }
    }
    stringBuilder.Append("])");
    return stringBuilder.ToString();
  }

  private void Grow()
  {
    SetStorage.Bucket[] buckets = this._buckets.Length < 1073741824 /*0x40000000*/ ? new SetStorage.Bucket[this._buckets.Length << 1] : throw PythonOps.MemoryError("set has reached its maximum size");
    for (int index = 0; index < this._buckets.Length; ++index)
    {
      SetStorage.Bucket bucket = this._buckets[index];
      if (bucket.Item != null && bucket.Item != SetStorage.Removed)
        SetStorage.AddWorker(buckets, bucket.Item, bucket.HashCode, this._eqFunc, ref this._version);
    }
    this._buckets = buckets;
    this._maxCount = (int) ((double) this._buckets.Length * 0.7);
  }

  private static void ProbeNext(SetStorage.Bucket[] buckets, ref int index)
  {
    ++index;
    if (index != buckets.Length)
      return;
    index = 0;
  }

  private static int CeilLog2(int x)
  {
    int num1 = x;
    int num2 = 1;
    if (x >= 65536 /*0x010000*/)
    {
      x >>= 16 /*0x10*/;
      num2 += 16 /*0x10*/;
    }
    if (x >= 256 /*0x0100*/)
    {
      x >>= 8;
      num2 += 8;
    }
    if (x >= 16 /*0x10*/)
    {
      x >>= 4;
      num2 += 4;
    }
    if (x >= 4)
    {
      x >>= 2;
      num2 += 2;
    }
    if (x >= 2)
      ++num2;
    return 1 << num2 != num1 ? num2 : num2 + 1;
  }

  public void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("buckets", (object) this.GetItems());
    info.AddValue("hasnull", this._hasNull);
  }

  void IDeserializationCallback.OnDeserialization(object sender)
  {
    if (this._buckets == null || !(this._buckets[0].Item is SerializationInfo serializationInfo))
      return;
    this._buckets = (SetStorage.Bucket[]) null;
    foreach (object obj in (List) serializationInfo.GetValue("buckets", typeof (List)))
      this.AddNoLock(obj);
    this._hasNull = (bool) serializationInfo.GetValue("hasnull", typeof (bool));
  }

  internal struct Bucket(int hashCode, object item)
  {
    public object Item = item;
    public int HashCode = hashCode;
  }
}
