// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonDictionary
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime;

[PythonType("dict")]
[DebuggerTypeProxy(typeof (PythonDictionary.DebugProxy))]
[DebuggerDisplay("Count = {Count}")]
[Serializable]
public class PythonDictionary : 
  IDictionary<object, object>,
  ICollection<KeyValuePair<object, object>>,
  IEnumerable<KeyValuePair<object, object>>,
  IEnumerable,
  IDictionary,
  ICollection,
  ICodeFormattable,
  IStructuralEquatable
{
  internal DictionaryStorage _storage;
  public const object __hash__ = null;

  internal static object MakeDict(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Dict ? (object) new PythonDictionary() : PythonCalls.Call(context, (object) cls);
  }

  public PythonDictionary() => this._storage = (DictionaryStorage) EmptyDictionaryStorage.Instance;

  internal PythonDictionary(DictionaryStorage storage) => this._storage = storage;

  internal PythonDictionary(IDictionary dict)
  {
    CommonDictionaryStorage dictionaryStorage = new CommonDictionaryStorage();
    foreach (DictionaryEntry dictionaryEntry in dict)
      dictionaryStorage.AddNoLock(dictionaryEntry.Key, dictionaryEntry.Value);
    this._storage = (DictionaryStorage) dictionaryStorage;
  }

  internal PythonDictionary(PythonDictionary dict) => this._storage = dict._storage.Clone();

  internal PythonDictionary(CodeContext context, object o)
    : this()
  {
    this.update(context, o);
  }

  internal PythonDictionary(int size)
  {
    this._storage = (DictionaryStorage) new CommonDictionaryStorage(size);
  }

  internal static PythonDictionary FromIAC(CodeContext context, PythonDictionary iac)
  {
    return !(iac.GetType() == typeof (PythonDictionary)) ? PythonDictionary.MakeDictFromIAC(context, iac) : iac;
  }

  private static PythonDictionary MakeDictFromIAC(CodeContext context, PythonDictionary iac)
  {
    return new PythonDictionary((DictionaryStorage) new ObjectAttributesAdapter(context, (object) iac));
  }

  internal static PythonDictionary MakeSymbolDictionary()
  {
    return new PythonDictionary((DictionaryStorage) new StringDictionaryStorage());
  }

  internal static PythonDictionary MakeSymbolDictionary(int count)
  {
    return new PythonDictionary((DictionaryStorage) new StringDictionaryStorage(count));
  }

  public void __init__(CodeContext context, object oø, [ParamDictionary] IDictionary<object, object> kwArgs)
  {
    this.update(context, oø);
    this.update(context, kwArgs);
  }

  public void __init__(CodeContext context, [ParamDictionary] IDictionary<object, object> kwArgs)
  {
    this.update(context, kwArgs);
  }

  public void __init__(CodeContext context, object oø) => this.update(context, oø);

  public void __init__()
  {
  }

  [PythonHidden(new PlatformID[] {})]
  public void Add(object key, object value) => this._storage.Add(ref this._storage, key, value);

  [PythonHidden(new PlatformID[] {})]
  public bool ContainsKey(object key) => this._storage.Contains(key);

  public ICollection<object> Keys
  {
    [PythonHidden(new PlatformID[] {})] get => (ICollection<object>) this.keys();
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Remove(object key)
  {
    try
    {
      this.__delitem__(key);
      return true;
    }
    catch (KeyNotFoundException ex)
    {
      return false;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public bool RemoveDirect(object key) => this._storage.Remove(ref this._storage, key);

  [PythonHidden(new PlatformID[] {})]
  public bool TryGetValue(object key, out object value)
  {
    return this._storage.TryGetValue(key, out value) || this.GetType() != typeof (PythonDictionary) && PythonTypeOps.TryInvokeBinaryOperator(DefaultContext.Default, (object) this, key, "__missing__", out value);
  }

  internal bool TryGetValueNoMissing(object key, out object value)
  {
    return this._storage.TryGetValue(key, out value);
  }

  public ICollection<object> Values
  {
    [PythonHidden(new PlatformID[] {})] get => (ICollection<object>) this.values();
  }

  [PythonHidden(new PlatformID[] {})]
  public void Add(KeyValuePair<object, object> item)
  {
    this._storage.Add(ref this._storage, item.Key, item.Value);
  }

  [PythonHidden(new PlatformID[] {})]
  public void Clear() => this._storage.Clear(ref this._storage);

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(KeyValuePair<object, object> item)
  {
    object x;
    return this._storage.TryGetValue(item.Key, out x) && PythonOps.EqualRetBool(x, item.Value);
  }

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
  {
    this._storage.GetItems().CopyTo(array, arrayIndex);
  }

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._storage.Count;
  }

  bool ICollection<KeyValuePair<object, object>>.IsReadOnly => false;

  [PythonHidden(new PlatformID[] {})]
  public bool Remove(KeyValuePair<object, object> item)
  {
    return this._storage.Remove(ref this._storage, item.Key);
  }

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
  {
    foreach (KeyValuePair<object, object> keyValuePair in this._storage.GetItems())
      yield return keyValuePair;
  }

  IEnumerator IEnumerable.GetEnumerator() => Converter.ConvertToIEnumerator(this.__iter__());

  public virtual object __iter__() => (object) new DictionaryKeyEnumerator(this._storage);

  public object get(object key) => DictionaryOps.get(this, key);

  public object get(object key, object defaultValue) => DictionaryOps.get(this, key, defaultValue);

  public virtual object this[params object[] key]
  {
    get
    {
      if (key == null)
        return this.GetItem((object) null);
      return key.Length != 0 ? this[(object) PythonTuple.MakeTuple(key)] : throw PythonOps.TypeError("__getitem__() takes exactly one argument (0 given)");
    }
    set
    {
      if (key == null)
      {
        this.SetItem((object) null, value);
      }
      else
      {
        if (key.Length == 0)
          throw PythonOps.TypeError("__setitem__() takes exactly two argument (1 given)");
        this[(object) PythonTuple.MakeTuple(key)] = value;
      }
    }
  }

  public virtual object this[object key]
  {
    get => this.GetItem(key);
    set => this.SetItem(key, value);
  }

  internal void SetItem(object key, object value)
  {
    this._storage.Add(ref this._storage, key, value);
  }

  private object GetItem(object key)
  {
    object obj;
    if (this.TryGetValue(key, out obj))
      return obj;
    throw PythonOps.KeyError(key);
  }

  public virtual void __delitem__(object key)
  {
    if (!this.RemoveDirect(key))
      throw PythonOps.KeyError(key);
  }

  public virtual void __delitem__(params object[] key)
  {
    if (key == null)
    {
      this.__delitem__((object) null);
    }
    else
    {
      if (key.Length == 0)
        throw PythonOps.TypeError("__delitem__() takes exactly one argument (0 given)");
      this.__delitem__((object) PythonTuple.MakeTuple(key));
    }
  }

  public virtual int __len__() => this.Count;

  public void clear() => this._storage.Clear(ref this._storage);

  [Python3Warning("dict.has_key() not supported in 3.x; use the in operator")]
  public bool has_key(object key) => DictionaryOps.has_key((IDictionary<object, object>) this, key);

  public object pop(object key) => DictionaryOps.pop(this, key);

  public object pop(object key, object defaultValue) => DictionaryOps.pop(this, key, defaultValue);

  public PythonTuple popitem() => DictionaryOps.popitem((IDictionary<object, object>) this);

  public object setdefault(object key) => DictionaryOps.setdefault(this, key);

  public object setdefault(object key, object defaultValue)
  {
    return DictionaryOps.setdefault(this, key, defaultValue);
  }

  public virtual List keys()
  {
    List list = new List();
    foreach (KeyValuePair<object, object> keyValuePair in this._storage.GetItems())
      list.append(keyValuePair.Key);
    return list;
  }

  public virtual List values()
  {
    List list = new List();
    foreach (KeyValuePair<object, object> keyValuePair in this._storage.GetItems())
      list.append(keyValuePair.Value);
    return list;
  }

  public virtual List items()
  {
    List list = new List();
    foreach (KeyValuePair<object, object> keyValuePair in this._storage.GetItems())
      list.append((object) PythonTuple.MakeTuple(keyValuePair.Key, keyValuePair.Value));
    return list;
  }

  public IEnumerator iteritems() => (IEnumerator) new DictionaryItemEnumerator(this._storage);

  public IEnumerator iterkeys() => (IEnumerator) new DictionaryKeyEnumerator(this._storage);

  public IEnumerator itervalues() => (IEnumerator) new DictionaryValueEnumerator(this._storage);

  public IEnumerable viewitems() => (IEnumerable) new DictionaryItemView(this);

  public IEnumerable viewkeys() => (IEnumerable) new DictionaryKeyView(this);

  public IEnumerable viewvalues() => (IEnumerable) new DictionaryValueView(this);

  public void update()
  {
  }

  public void update(CodeContext context, [ParamDictionary] IDictionary<object, object> otherø)
  {
    DictionaryOps.update(context, this, (object) otherø);
  }

  public void update(CodeContext context, object otherø)
  {
    DictionaryOps.update(context, this, otherø);
  }

  public void update(CodeContext context, object otherø, [ParamDictionary] IDictionary<object, object> otherArgsø)
  {
    DictionaryOps.update(context, this, otherø);
    DictionaryOps.update(context, this, (object) otherArgsø);
  }

  private static object fromkeysAny(CodeContext context, PythonType cls, object o, object value)
  {
    if (cls == TypeCache.Dict)
    {
      PythonDictionary pythonDictionary;
      switch (o)
      {
        case ICollection collection:
          pythonDictionary = new PythonDictionary((DictionaryStorage) new CommonDictionaryStorage(collection.Count));
          break;
        case string str:
          pythonDictionary = new PythonDictionary(str.Length);
          break;
        default:
          pythonDictionary = new PythonDictionary();
          break;
      }
      IEnumerator enumerator = PythonOps.GetEnumerator(o);
      while (enumerator.MoveNext())
        pythonDictionary._storage.AddNoLock(ref pythonDictionary._storage, enumerator.Current, value);
      return (object) pythonDictionary;
    }
    object a = PythonDictionary.MakeDict(context, cls);
    if (a is PythonDictionary pythonDictionary1)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(o);
      while (enumerator.MoveNext())
        pythonDictionary1[enumerator.Current] = value;
    }
    else
    {
      PythonContext languageContext = context.LanguageContext;
      IEnumerator enumerator = PythonOps.GetEnumerator(o);
      while (enumerator.MoveNext())
        languageContext.SetIndex(a, enumerator.Current, value);
    }
    return a;
  }

  [ClassMethod]
  public static object fromkeys(CodeContext context, PythonType cls, object seq)
  {
    return PythonDictionary.fromkeys(context, cls, seq, (object) null);
  }

  [ClassMethod]
  public static object fromkeys(CodeContext context, PythonType cls, object seq, object value)
  {
    if (!(seq is XRange xrange))
      return PythonDictionary.fromkeysAny(context, cls, seq, value);
    int num = xrange.__len__();
    object a = context.LanguageContext.CallSplat((object) cls);
    if (a.GetType() == typeof (PythonDictionary))
    {
      PythonDictionary pythonDictionary = a as PythonDictionary;
      for (int index = 0; index < num; ++index)
        pythonDictionary[xrange[index]] = value;
    }
    else
    {
      PythonContext languageContext = context.LanguageContext;
      for (int index = 0; index < num; ++index)
        languageContext.SetIndex(a, xrange[index], value);
    }
    return a;
  }

  public virtual PythonDictionary copy(CodeContext context)
  {
    return new PythonDictionary(this._storage.Clone());
  }

  public virtual bool __contains__(object key) => this._storage.Contains(key);

  [return: MaybeNotImplemented]
  public object __eq__(CodeContext context, object other)
  {
    switch (other)
    {
      case PythonDictionary _:
      case IDictionary<object, object> _:
        return ScriptingRuntimeHelpers.BooleanToObject(((IStructuralEquatable) this).Equals(other, context.LanguageContext.EqualityComparerNonGeneric));
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [return: MaybeNotImplemented]
  public object __ne__(CodeContext context, object other)
  {
    switch (other)
    {
      case PythonDictionary _:
      case IDictionary<object, object> _:
        return ScriptingRuntimeHelpers.BooleanToObject(!((IStructuralEquatable) this).Equals(other, context.LanguageContext.EqualityComparerNonGeneric));
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [return: MaybeNotImplemented]
  public object __cmp__(CodeContext context, object other)
  {
    if (!(other is IDictionary<object, object> dictionary))
    {
      object ret1;
      object ret2;
      if (!PythonOps.TryGetBoundAttr(context, other, "__len__", out ret1) || !PythonOps.TryGetBoundAttr(context, other, "iteritems", out ret2))
        return (object) NotImplementedType.Value;
      int count = this.Count;
      int int32 = context.LanguageContext.ConvertToInt32(PythonOps.CallWithContext(context, ret1));
      return count != int32 ? (object) (count > int32 ? 1 : -1) : (object) DictionaryOps.CompareToWorker(context, (IDictionary<object, object>) this, new List(PythonOps.CallWithContext(context, ret2)));
    }
    CompareUtil.Push((object) this, (object) dictionary);
    try
    {
      return (object) DictionaryOps.CompareTo(context, (IDictionary<object, object>) this, dictionary);
    }
    finally
    {
      CompareUtil.Pop((object) this, (object) dictionary);
    }
  }

  public int __cmp__(CodeContext context, [NotNull] PythonDictionary other)
  {
    CompareUtil.Push((object) this, (object) other);
    try
    {
      return DictionaryOps.CompareTo(context, (IDictionary<object, object>) this, (IDictionary<object, object>) other);
    }
    finally
    {
      CompareUtil.Pop((object) this, (object) other);
    }
  }

  [Python3Warning("dict inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator >(PythonDictionary self, PythonDictionary other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("dict inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator <(PythonDictionary self, PythonDictionary other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("dict inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator >=(PythonDictionary self, PythonDictionary other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("dict inequality comparisons not supported in 3.x")]
  public static NotImplementedType operator <=(PythonDictionary self, PythonDictionary other)
  {
    return PythonOps.NotImplemented;
  }

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    if (CompareUtil.Check((object) this))
      return 0;
    SetStorage set = new SetStorage();
    foreach (KeyValuePair<object, object> keyValuePair in this._storage.GetItems())
      set.AddNoLock((object) PythonTuple.MakeTuple(keyValuePair.Key, keyValuePair.Value));
    CompareUtil.Push((object) this);
    try
    {
      return ((IStructuralEquatable) FrozenSetCollection.Make(TypeCache.FrozenSet, (object) set)).GetHashCode(comparer);
    }
    finally
    {
      CompareUtil.Pop((object) this);
    }
  }

  bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
  {
    return this.EqualsWorker(other, comparer);
  }

  private bool EqualsWorker(object other, IEqualityComparer comparer)
  {
    if (this == other)
      return true;
    if (!(other is IDictionary<object, object> dictionary) || dictionary.Count != this.Count)
      return false;
    if (other is PythonDictionary pd)
      return this.ValueEqualsPythonDict(pd, comparer);
    foreach (object key in this.keys())
    {
      object obj;
      if (!dictionary.TryGetValue(key, out obj))
        return false;
      CompareUtil.Push(obj);
      try
      {
        if (comparer == null)
        {
          if (!PythonOps.EqualRetBool(obj, this[key]))
            return false;
        }
        else if (!comparer.Equals(obj, this[key]))
          return false;
      }
      finally
      {
        CompareUtil.Pop(obj);
      }
    }
    return true;
  }

  private bool ValueEqualsPythonDict(PythonDictionary pd, IEqualityComparer comparer)
  {
    foreach (object key in this.keys())
    {
      object obj;
      if (!pd.TryGetValueNoMissing(key, out obj))
        return false;
      CompareUtil.Push(obj);
      try
      {
        if (comparer == null)
        {
          if (!PythonOps.EqualRetBool(obj, this[key]))
            return false;
        }
        else if (!comparer.Equals(obj, this[key]))
          return false;
      }
      finally
      {
        CompareUtil.Pop(obj);
      }
    }
    return true;
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(object key) => this.__contains__(key);

  IDictionaryEnumerator IDictionary.GetEnumerator()
  {
    return (IDictionaryEnumerator) new PythonDictionary.DictEnumerator((IEnumerator<KeyValuePair<object, object>>) this._storage.GetItems().GetEnumerator());
  }

  bool IDictionary.IsFixedSize => false;

  bool IDictionary.IsReadOnly => false;

  ICollection IDictionary.Keys => (ICollection) this.keys();

  ICollection IDictionary.Values => (ICollection) this.values();

  void IDictionary.Remove(object key) => this.Remove(key);

  void ICollection.CopyTo(Array array, int index)
  {
    throw new NotImplementedException("The method or operation is not implemented.");
  }

  bool ICollection.IsSynchronized => false;

  object ICollection.SyncRoot => (object) null;

  public virtual string __repr__(CodeContext context)
  {
    return DictionaryOps.__repr__(context, (IDictionary<object, object>) this);
  }

  internal bool TryRemoveValue(object key, out object value)
  {
    return this._storage.TryRemoveValue(ref this._storage, key, out value);
  }

  internal class DictEnumerator : IDictionaryEnumerator, IEnumerator
  {
    private IEnumerator<KeyValuePair<object, object>> _enumerator;
    private bool _moved;

    public DictEnumerator(
      IEnumerator<KeyValuePair<object, object>> enumerator)
    {
      this._enumerator = enumerator;
    }

    public DictionaryEntry Entry
    {
      get
      {
        if (!this._moved)
          throw new InvalidOperationException();
        KeyValuePair<object, object> current = this._enumerator.Current;
        object key = current.Key;
        current = this._enumerator.Current;
        object obj = current.Value;
        return new DictionaryEntry(key, obj);
      }
    }

    public object Key => this.Entry.Key;

    public object Value => this.Entry.Value;

    public object Current => (object) this.Entry;

    public bool MoveNext()
    {
      if (this._enumerator.MoveNext())
      {
        this._moved = true;
        return true;
      }
      this._moved = false;
      return false;
    }

    public void Reset()
    {
      this._enumerator.Reset();
      this._moved = false;
    }
  }

  internal class DebugProxy
  {
    private readonly PythonDictionary _dict;

    public DebugProxy(PythonDictionary dict) => this._dict = dict;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<PythonDictionary.KeyValueDebugView> Members
    {
      get
      {
        List<PythonDictionary.KeyValueDebugView> members = new List<PythonDictionary.KeyValueDebugView>();
        foreach (KeyValuePair<object, object> keyValuePair in this._dict)
          members.Add(new PythonDictionary.KeyValueDebugView(keyValuePair.Key, keyValuePair.Value));
        return members;
      }
    }
  }

  [DebuggerDisplay("{Value}", Name = "{Key,nq}", Type = "{TypeInfo,nq}")]
  internal class KeyValueDebugView
  {
    public readonly object Key;
    public readonly object Value;

    public KeyValueDebugView(object key, object value)
    {
      this.Key = key;
      this.Value = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string TypeInfo
    {
      get => $"Key: {PythonTypeOps.GetName(this.Key)}, Value: {PythonTypeOps.GetName(this.Value)}";
    }
  }
}
