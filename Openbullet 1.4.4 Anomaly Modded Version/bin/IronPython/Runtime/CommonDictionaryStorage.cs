// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CommonDictionaryStorage
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
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class CommonDictionaryStorage : DictionaryStorage, ISerializable, IDeserializationCallback
{
  protected CommonDictionaryStorage.Bucket[] _buckets;
  private int _count;
  private int _version;
  private CommonDictionaryStorage.NullValue _nullValue;
  private Func<object, int> _hashFunc;
  private Func<object, object, bool> _eqFunc;
  private Type _keyType;
  private const int InitialBucketSize = 7;
  private const int ResizeMultiplier = 3;
  private const double Load = 0.7;
  private static readonly Func<object, int> _primitiveHash = new Func<object, int>(CommonDictionaryStorage.PrimitiveHash);
  private static readonly Func<object, int> _doubleHash = new Func<object, int>(CommonDictionaryStorage.DoubleHash);
  private static readonly Func<object, int> _intHash = new Func<object, int>(CommonDictionaryStorage.IntHash);
  private static readonly Func<object, int> _tupleHash = new Func<object, int>(CommonDictionaryStorage.TupleHash);
  private static readonly Func<object, int> _genericHash = new Func<object, int>(CommonDictionaryStorage.GenericHash);
  private static readonly Func<object, object, bool> _intEquals = new Func<object, object, bool>(CommonDictionaryStorage.IntEquals);
  private static readonly Func<object, object, bool> _doubleEquals = new Func<object, object, bool>(CommonDictionaryStorage.DoubleEquals);
  private static readonly Func<object, object, bool> _stringEquals = new Func<object, object, bool>(CommonDictionaryStorage.StringEquals);
  private static readonly Func<object, object, bool> _tupleEquals = new Func<object, object, bool>(CommonDictionaryStorage.TupleEquals);
  private static readonly Func<object, object, bool> _genericEquals = new Func<object, object, bool>(CommonDictionaryStorage.GenericEquals);
  private static readonly Func<object, object, bool> _objectEq = new Func<object, object, bool>(object.ReferenceEquals);
  private static readonly Type HeterogeneousType = typeof (CommonDictionaryStorage);
  private static readonly object _removed = new object();

  public CommonDictionaryStorage()
  {
  }

  public CommonDictionaryStorage(int count)
  {
    this._buckets = new CommonDictionaryStorage.Bucket[(int) ((double) count / 0.7 + 2.0)];
  }

  public CommonDictionaryStorage(object[] items, bool isHomogeneous)
    : this(Math.Max(items.Length / 2, 7))
  {
    PythonType t = DynamicHelpers.GetPythonType(items[1]);
    if (!isHomogeneous)
    {
      for (int index = 1; index < items.Length / 2; ++index)
      {
        if (DynamicHelpers.GetPythonType(items[index * 2 + 1]) != t)
        {
          this.SetHeterogeneousSites();
          t = (PythonType) null;
          break;
        }
      }
    }
    if (t != null)
      this.UpdateHelperFunctions((Type) t, items[1]);
    for (int index = 0; index < items.Length / 2; ++index)
    {
      object key = items[index * 2 + 1];
      if (key != null)
        this.AddOne(key, items[index * 2]);
      else
        this.AddNull(items[index * 2]);
    }
  }

  public int Version => this._version;

  private void AddItems(object[] items)
  {
    for (int index = 0; index < items.Length / 2; ++index)
      this.AddNoLock(items[index * 2 + 1], items[index * 2]);
  }

  private CommonDictionaryStorage(
    CommonDictionaryStorage.Bucket[] buckets,
    int count,
    Type keyType,
    Func<object, int> hashFunc,
    Func<object, object, bool> eqFunc,
    CommonDictionaryStorage.NullValue nullValue)
  {
    this._buckets = buckets;
    this._count = count;
    this._keyType = keyType;
    this._hashFunc = hashFunc;
    this._eqFunc = eqFunc;
    this._nullValue = nullValue;
  }

  private CommonDictionaryStorage(SerializationInfo info, StreamingContext context)
  {
    this._nullValue = (CommonDictionaryStorage.NullValue) new CommonDictionaryStorage.DeserializationNullValue(info);
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this.Add(key, value);
  }

  public void Add(object key, object value)
  {
    lock (this)
      this.AddNoLock(key, value);
  }

  private void AddNull(object value)
  {
    if (this._nullValue != null)
      this._nullValue.Value = value;
    else
      this._nullValue = new CommonDictionaryStorage.NullValue(value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    this.AddNoLock(key, value);
  }

  public void AddNoLock(object key, object value)
  {
    if (key != null)
    {
      if (this._buckets == null)
        this.Initialize();
      if (key.GetType() != this._keyType && this._keyType != CommonDictionaryStorage.HeterogeneousType)
        this.UpdateHelperFunctions(key.GetType(), key);
      this.AddOne(key, value);
    }
    else
      this.AddNull(value);
  }

  private void AddOne(object key, object value)
  {
    if (!this.Add(this._buckets, key, value))
      return;
    ++this._count;
    if ((double) this._count < (double) this._buckets.Length * 0.7)
      return;
    this.EnsureSize((int) ((double) this._buckets.Length / 0.7) * 3);
  }

  private void UpdateHelperFunctions(Type t, object key)
  {
    if (this._keyType == (Type) null)
    {
      if (t == typeof (int))
      {
        this._hashFunc = CommonDictionaryStorage._intHash;
        this._eqFunc = CommonDictionaryStorage._intEquals;
      }
      else if (t == typeof (string))
      {
        this._hashFunc = CommonDictionaryStorage._primitiveHash;
        this._eqFunc = CommonDictionaryStorage._stringEquals;
      }
      else if (t == typeof (double))
      {
        this._hashFunc = CommonDictionaryStorage._doubleHash;
        this._eqFunc = CommonDictionaryStorage._doubleEquals;
      }
      else if (t == typeof (PythonTuple))
      {
        this._hashFunc = CommonDictionaryStorage._tupleHash;
        this._eqFunc = CommonDictionaryStorage._tupleEquals;
      }
      else if (t == typeof (Type).GetType())
      {
        this._hashFunc = CommonDictionaryStorage._primitiveHash;
        this._eqFunc = CommonDictionaryStorage._objectEq;
      }
      else
      {
        PythonType pythonType = DynamicHelpers.GetPythonType(key);
        this.AssignSiteDelegates(PythonContext.GetHashSite(pythonType), DefaultContext.DefaultPythonContext.GetEqualSite((Type) pythonType));
      }
      this._keyType = t;
    }
    else
    {
      if (!(this._keyType != CommonDictionaryStorage.HeterogeneousType))
        return;
      this.SetHeterogeneousSites();
      this._buckets = (CommonDictionaryStorage.Bucket[]) this._buckets.Clone();
    }
  }

  private void SetHeterogeneousSites()
  {
    this.AssignSiteDelegates(DefaultContext.DefaultPythonContext.MakeHashSite(), DefaultContext.DefaultPythonContext.MakeEqualSite());
    this._keyType = CommonDictionaryStorage.HeterogeneousType;
  }

  private void AssignSiteDelegates(
    CallSite<Func<CallSite, object, int>> hashSite,
    CallSite<Func<CallSite, object, object, bool>> equalSite)
  {
    this._hashFunc = (Func<object, int>) (o => hashSite.Target((CallSite) hashSite, o));
    this._eqFunc = (Func<object, object, bool>) ((o1, o2) => equalSite.Target((CallSite) equalSite, o1, o2));
  }

  private void EnsureSize(int newSize)
  {
    if (this._buckets.Length >= newSize)
      return;
    CommonDictionaryStorage.Bucket[] buckets1 = this._buckets;
    CommonDictionaryStorage.Bucket[] buckets2 = new CommonDictionaryStorage.Bucket[newSize];
    for (int index = 0; index < buckets1.Length; ++index)
    {
      CommonDictionaryStorage.Bucket bucket = buckets1[index];
      if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed)
        this.AddWorker(buckets2, bucket.Key, bucket.Value, bucket.HashCode);
    }
    this._buckets = buckets2;
  }

  public override void EnsureCapacityNoLock(int size)
  {
    if (this._buckets == null)
      this._buckets = new CommonDictionaryStorage.Bucket[(int) ((double) size / 0.7) + 1];
    else
      this.EnsureSize((int) ((double) size / 0.7));
  }

  private void Initialize() => this._buckets = new CommonDictionaryStorage.Bucket[7];

  private bool Add(CommonDictionaryStorage.Bucket[] buckets, object key, object value)
  {
    int hc = this.Hash(key);
    return this.AddWorker(buckets, key, value, hc);
  }

  protected bool AddWorker(
    CommonDictionaryStorage.Bucket[] buckets,
    object key,
    object value,
    int hc)
  {
    int num = hc % buckets.Length;
    int index1 = num;
    int index2 = -1;
    do
    {
      CommonDictionaryStorage.Bucket bucket = buckets[index1];
      if (bucket.Key == null)
      {
        if (index2 == -1)
        {
          index2 = index1;
          break;
        }
        break;
      }
      if (bucket.Key == CommonDictionaryStorage._removed)
      {
        if (index2 == -1)
          index2 = index1;
      }
      else if (key == bucket.Key || bucket.HashCode == hc && this._eqFunc(key, bucket.Key))
      {
        ++this._version;
        buckets[index1].Value = value;
        return false;
      }
      index1 = CommonDictionaryStorage.ProbeNext(buckets, index1);
    }
    while (index1 != num);
    ++this._version;
    buckets[index2].HashCode = hc;
    buckets[index2].Value = value;
    buckets[index2].Key = key;
    return true;
  }

  private static int ProbeNext(CommonDictionaryStorage.Bucket[] buckets, int index)
  {
    ++index;
    if (index == buckets.Length)
      index = 0;
    return index;
  }

  public override bool Remove(ref DictionaryStorage storage, object key) => this.Remove(key);

  public bool Remove(object key) => this.TryRemoveValue(key, out object _);

  internal bool RemoveAlwaysHash(object key)
  {
    lock (this)
    {
      object obj;
      return key == null ? this.TryRemoveNull(out obj) : this.TryRemoveNoLock(key, out obj);
    }
  }

  public override bool TryRemoveValue(ref DictionaryStorage storage, object key, out object value)
  {
    return this.TryRemoveValue(key, out value);
  }

  public bool TryRemoveValue(object key, out object value)
  {
    lock (this)
    {
      if (key == null)
        return this.TryRemoveNull(out value);
      if (this._count != 0)
        return this.TryRemoveNoLock(key, out value);
      value = (object) null;
      return false;
    }
  }

  private bool TryRemoveNull(out object value)
  {
    if (this._nullValue != null)
    {
      value = this._nullValue.Value;
      this._nullValue = (CommonDictionaryStorage.NullValue) null;
      return true;
    }
    value = (object) null;
    return false;
  }

  private bool TryRemoveNoLock(object key, out object value)
  {
    Func<object, int> func;
    Func<object, object, bool> eqFunc;
    if (key.GetType() == this._keyType || this._keyType == CommonDictionaryStorage.HeterogeneousType)
    {
      func = this._hashFunc;
      eqFunc = this._eqFunc;
    }
    else
    {
      func = CommonDictionaryStorage._genericHash;
      eqFunc = CommonDictionaryStorage._genericEquals;
    }
    int hc = func(key) & int.MaxValue;
    return this.TryRemoveNoLock(key, eqFunc, hc, out value);
  }

  protected bool TryRemoveNoLock(
    object key,
    Func<object, object, bool> eqFunc,
    int hc,
    out object value)
  {
    if (this._buckets == null)
    {
      value = (object) null;
      return false;
    }
    int index = hc % this._buckets.Length;
    int num = index;
    do
    {
      CommonDictionaryStorage.Bucket bucket = this._buckets[index];
      if (bucket.Key != null)
      {
        if (key == bucket.Key || bucket.Key != CommonDictionaryStorage._removed && bucket.HashCode == hc && eqFunc(key, bucket.Key))
        {
          value = bucket.Value;
          ++this._version;
          this._buckets[index].Key = CommonDictionaryStorage._removed;
          Thread.MemoryBarrier();
          this._buckets[index].Value = (object) null;
          --this._count;
          return true;
        }
        index = CommonDictionaryStorage.ProbeNext(this._buckets, index);
      }
      else
        break;
    }
    while (index != num);
    value = (object) null;
    return false;
  }

  public override bool Contains(object key)
  {
    return PythonContext.IsHashable(key) ? this.TryGetValue(key, out object _) : throw PythonOps.TypeErrorForUnhashableObject(key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (key != null)
      return this.TryGetValue(this._buckets, key, out value);
    CommonDictionaryStorage.NullValue nullValue = this._nullValue;
    if (nullValue != null)
    {
      value = nullValue.Value;
      return true;
    }
    value = (object) null;
    return false;
  }

  private bool TryGetValue(CommonDictionaryStorage.Bucket[] buckets, object key, out object value)
  {
    if (this._count > 0 && buckets != null)
    {
      int hc;
      Func<object, object, bool> eqFunc;
      if (key.GetType() == this._keyType || this._keyType == CommonDictionaryStorage.HeterogeneousType)
      {
        hc = this._hashFunc(key) & int.MaxValue;
        eqFunc = this._eqFunc;
      }
      else
      {
        hc = CommonDictionaryStorage._genericHash(key) & int.MaxValue;
        eqFunc = CommonDictionaryStorage._genericEquals;
      }
      return CommonDictionaryStorage.TryGetValue(buckets, key, hc, eqFunc, out value);
    }
    value = (object) null;
    return false;
  }

  protected static bool TryGetValue(
    CommonDictionaryStorage.Bucket[] buckets,
    object key,
    int hc,
    Func<object, object, bool> eqFunc,
    out object value)
  {
    int index = hc % buckets.Length;
    int num = index;
    do
    {
      CommonDictionaryStorage.Bucket bucket = buckets[index];
      if (bucket.Key != null)
      {
        if (key == bucket.Key || bucket.Key != CommonDictionaryStorage._removed && bucket.HashCode == hc && eqFunc(key, bucket.Key))
        {
          value = bucket.Value;
          return true;
        }
        index = CommonDictionaryStorage.ProbeNext(buckets, index);
      }
      else
        break;
    }
    while (num != index);
    value = (object) null;
    return false;
  }

  public override int Count
  {
    get
    {
      int count = this._count;
      if (this._nullValue != null)
        ++count;
      return count;
    }
  }

  public override void Clear(ref DictionaryStorage storage) => this.Clear();

  public void Clear()
  {
    lock (this)
    {
      if (this._buckets != null)
      {
        ++this._version;
        this._buckets = new CommonDictionaryStorage.Bucket[8];
        this._count = 0;
      }
      this._nullValue = (CommonDictionaryStorage.NullValue) null;
    }
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    lock (this)
    {
      List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>(this._count + (this._nullValue != null ? 1 : 0));
      if (this._count > 0)
      {
        for (int index = 0; index < this._buckets.Length; ++index)
        {
          CommonDictionaryStorage.Bucket bucket = this._buckets[index];
          if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed)
            items.Add(new KeyValuePair<object, object>(bucket.Key, bucket.Value));
        }
      }
      if (this._nullValue != null)
        items.Add(new KeyValuePair<object, object>((object) null, this._nullValue.Value));
      return items;
    }
  }

  public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
  {
    CommonDictionaryStorage dictionaryStorage = this;
    lock (dictionaryStorage)
    {
      if (dictionaryStorage._count > 0)
      {
        for (int i = 0; i < dictionaryStorage._buckets.Length; ++i)
        {
          CommonDictionaryStorage.Bucket bucket = dictionaryStorage._buckets[i];
          if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed)
            yield return new KeyValuePair<object, object>(bucket.Key, bucket.Value);
        }
      }
      if (dictionaryStorage._nullValue != null)
        yield return new KeyValuePair<object, object>((object) null, dictionaryStorage._nullValue.Value);
    }
  }

  public override IEnumerable<object> GetKeys()
  {
    CommonDictionaryStorage.Bucket[] buckets = this._buckets;
    lock (this)
    {
      object[] keys = new object[this.Count];
      int num1 = 0;
      if (buckets != null)
      {
        for (int index = 0; index < buckets.Length; ++index)
        {
          CommonDictionaryStorage.Bucket bucket = buckets[index];
          if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed)
            keys[num1++] = bucket.Key;
        }
      }
      if (this._nullValue != null)
      {
        object[] objArray = keys;
        int index = num1;
        int num2 = index + 1;
        objArray[index] = (object) null;
      }
      return (IEnumerable<object>) keys;
    }
  }

  public override bool HasNonStringAttributes()
  {
    lock (this)
    {
      CommonDictionaryStorage.NullValue nullValue = this._nullValue;
      if (nullValue != null && !(nullValue.Value is string))
        return true;
      if (this._keyType != typeof (string))
      {
        if (this._keyType != (Type) null)
        {
          if (this._count > 0)
          {
            for (int index = 0; index < this._buckets.Length; ++index)
            {
              CommonDictionaryStorage.Bucket bucket = this._buckets[index];
              if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed && !(bucket.Key is string))
                return true;
            }
          }
        }
      }
    }
    return false;
  }

  public override DictionaryStorage Clone()
  {
    lock (this)
    {
      if (this._buckets == null)
        return this._nullValue != null ? (DictionaryStorage) new CommonDictionaryStorage((CommonDictionaryStorage.Bucket[]) null, 1, this._keyType, this._hashFunc, this._eqFunc, new CommonDictionaryStorage.NullValue(this._nullValue.Value)) : (DictionaryStorage) new CommonDictionaryStorage();
      CommonDictionaryStorage.Bucket[] buckets = new CommonDictionaryStorage.Bucket[this._buckets.Length];
      for (int index = 0; index < this._buckets.Length; ++index)
      {
        if (this._buckets[index].Key != null)
          buckets[index] = this._buckets[index];
      }
      CommonDictionaryStorage.NullValue nullValue = (CommonDictionaryStorage.NullValue) null;
      if (this._nullValue != null)
        nullValue = new CommonDictionaryStorage.NullValue(this._nullValue.Value);
      return (DictionaryStorage) new CommonDictionaryStorage(buckets, this._count, this._keyType, this._hashFunc, this._eqFunc, nullValue);
    }
  }

  public override void CopyTo(ref DictionaryStorage into) => into = this.CopyTo(into);

  public DictionaryStorage CopyTo(DictionaryStorage into)
  {
    if (this._buckets != null)
    {
      using (new OrderedLocker((object) this, (object) into))
      {
        if (into is CommonDictionaryStorage into1)
          this.CommonCopyTo(into1);
        else
          this.UncommonCopyTo(ref into);
      }
    }
    CommonDictionaryStorage.NullValue nullValue = this._nullValue;
    if (nullValue != null)
      into.Add(ref into, (object) null, nullValue.Value);
    return into;
  }

  private void CommonCopyTo(CommonDictionaryStorage into)
  {
    if (into._buckets == null)
    {
      into._buckets = new CommonDictionaryStorage.Bucket[this._buckets.Length];
    }
    else
    {
      int length = into._buckets.Length;
      int num = (int) ((double) (this._count + into._count) / 0.7) + 2;
      while (length < num)
        length *= 3;
      into.EnsureSize(length);
    }
    if (into._keyType == (Type) null)
    {
      into._keyType = this._keyType;
      into._hashFunc = this._hashFunc;
      into._eqFunc = this._eqFunc;
    }
    else if (into._keyType != this._keyType)
      into.SetHeterogeneousSites();
    for (int index = 0; index < this._buckets.Length; ++index)
    {
      CommonDictionaryStorage.Bucket bucket = this._buckets[index];
      if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed && into.AddWorker(into._buckets, bucket.Key, bucket.Value, bucket.HashCode))
        ++into._count;
    }
  }

  private void UncommonCopyTo(ref DictionaryStorage into)
  {
    for (int index = 0; index < this._buckets.Length; ++index)
    {
      CommonDictionaryStorage.Bucket bucket = this._buckets[index];
      if (bucket.Key != null && bucket.Key != CommonDictionaryStorage._removed)
        into.AddNoLock(ref into, bucket.Key, bucket.Value);
    }
  }

  private int Hash(object key)
  {
    return key is string ? key.GetHashCode() & int.MaxValue : this._hashFunc(key) & int.MaxValue;
  }

  private static int PrimitiveHash(object o) => o.GetHashCode();

  private static int IntHash(object o) => (int) o;

  private static int DoubleHash(object o) => DoubleOps.__hash__((double) o);

  private static int GenericHash(object o) => PythonOps.Hash(DefaultContext.Default, o);

  private static int TupleHash(object o)
  {
    return ((IStructuralEquatable) o).GetHashCode(DefaultContext.DefaultPythonContext.EqualityComparerNonGeneric);
  }

  private static bool StringEquals(object o1, object o2) => (string) o1 == (string) o2;

  private static bool IntEquals(object o1, object o2) => (int) o1 == (int) o2;

  private static bool DoubleEquals(object o1, object o2) => (double) o1 == (double) o2;

  private static bool TupleEquals(object o1, object o2)
  {
    return ((IStructuralEquatable) o1).Equals(o2, DefaultContext.DefaultPythonContext.EqualityComparerNonGeneric);
  }

  private static bool GenericEquals(object o1, object o2) => PythonOps.EqualRetBool(o1, o2);

  private CommonDictionaryStorage.DeserializationNullValue GetDeserializationBucket()
  {
    return this._nullValue as CommonDictionaryStorage.DeserializationNullValue;
  }

  public void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("buckets", (object) this.GetItems());
    info.AddValue("nullvalue", (object) this._nullValue);
  }

  void IDeserializationCallback.OnDeserialization(object sender)
  {
    CommonDictionaryStorage.DeserializationNullValue deserializationBucket = this.GetDeserializationBucket();
    if (deserializationBucket == null)
      return;
    SerializationInfo serializationInfo = deserializationBucket.SerializationInfo;
    this._buckets = (CommonDictionaryStorage.Bucket[]) null;
    this._nullValue = (CommonDictionaryStorage.NullValue) null;
    foreach (KeyValuePair<object, object> keyValuePair in (List<KeyValuePair<object, object>>) serializationInfo.GetValue("buckets", typeof (List<KeyValuePair<object, object>>)))
      this.Add(keyValuePair.Key, keyValuePair.Value);
    CommonDictionaryStorage.NullValue nullValue = (CommonDictionaryStorage.NullValue) null;
    try
    {
      nullValue = (CommonDictionaryStorage.NullValue) serializationInfo.GetValue("nullvalue", typeof (CommonDictionaryStorage.NullValue));
    }
    catch (SerializationException ex)
    {
    }
    if (nullValue == null)
      return;
    this._nullValue = new CommonDictionaryStorage.NullValue((object) nullValue);
  }

  protected struct Bucket(int hashCode, object key, object value)
  {
    public object Key = key;
    public object Value = value;
    public int HashCode = hashCode;
  }

  [Serializable]
  private class NullValue
  {
    public object Value;

    public NullValue(object value) => this.Value = value;
  }

  private class DeserializationNullValue(SerializationInfo info) : CommonDictionaryStorage.NullValue((object) info)
  {
    public SerializationInfo SerializationInfo => (SerializationInfo) this.Value;
  }
}
