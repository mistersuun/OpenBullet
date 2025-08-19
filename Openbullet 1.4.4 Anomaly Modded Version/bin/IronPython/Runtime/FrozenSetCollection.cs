// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FrozenSetCollection
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("frozenset")]
[DebuggerDisplay("frozenset, {Count} items", TargetTypeName = "frozenset")]
[DebuggerTypeProxy(typeof (CollectionDebugProxy))]
public class FrozenSetCollection : 
  IEnumerable,
  IEnumerable<object>,
  ICollection,
  IStructuralEquatable,
  ICodeFormattable
{
  internal SetStorage _items;
  private FrozenSetCollection.HashCache _hashCache;
  private static readonly FrozenSetCollection _empty = new FrozenSetCollection();

  public void __init__(params object[] o)
  {
  }

  public static FrozenSetCollection __new__(CodeContext context, object cls)
  {
    if (cls == TypeCache.FrozenSet)
      return FrozenSetCollection._empty;
    object instance = ((PythonType) cls).CreateInstance(context);
    return instance is FrozenSetCollection frozenSetCollection ? frozenSetCollection : throw PythonOps.TypeError("{0} is not a subclass of frozenset", instance);
  }

  public static FrozenSetCollection __new__(CodeContext context, object cls, object set)
  {
    if (cls == TypeCache.FrozenSet)
      return FrozenSetCollection.Make(TypeCache.FrozenSet, set);
    object instance = ((PythonType) cls).CreateInstance(context, set);
    return instance is FrozenSetCollection frozenSetCollection ? frozenSetCollection : throw PythonOps.TypeError("{0} is not a subclass of frozenset", instance);
  }

  public FrozenSetCollection()
    : this(new SetStorage())
  {
  }

  internal FrozenSetCollection(SetStorage set) => this._items = set;

  protected internal FrozenSetCollection(object set)
    : this(SetStorage.GetFrozenItems(set))
  {
  }

  private FrozenSetCollection Empty
  {
    get
    {
      return this.GetType() == typeof (FrozenSetCollection) ? FrozenSetCollection._empty : FrozenSetCollection.Make(DynamicHelpers.GetPythonType((object) this), new SetStorage());
    }
  }

  private FrozenSetCollection Make(SetStorage items)
  {
    if (items.Count == 0)
      return this.Empty;
    return this.GetType() == typeof (FrozenSetCollection) ? new FrozenSetCollection(items) : FrozenSetCollection.Make(DynamicHelpers.GetPythonType((object) this), items);
  }

  private static FrozenSetCollection Make(PythonType cls, SetStorage items)
  {
    if (cls == TypeCache.FrozenSet)
      return items.Count == 0 ? FrozenSetCollection._empty : new FrozenSetCollection(items);
    FrozenSetCollection frozenSetCollection = PythonCalls.Call((object) cls) as FrozenSetCollection;
    if (items.Count > 0)
      frozenSetCollection._items = items;
    return frozenSetCollection;
  }

  internal static FrozenSetCollection Make(PythonType cls, object set)
  {
    return set is FrozenSetCollection frozenSetCollection && cls == TypeCache.FrozenSet ? frozenSetCollection : FrozenSetCollection.Make(cls, SetStorage.GetFrozenItems(set));
  }

  public FrozenSetCollection copy()
  {
    return this.GetType() == typeof (FrozenSetCollection) ? this : FrozenSetCollection.Make(DynamicHelpers.GetPythonType((object) this), this._items);
  }

  public int __len__() => this.Count;

  public bool __contains__(object item)
  {
    return !SetStorage.GetHashableSetIfSet(ref item) ? this._items.ContainsAlwaysHash(item) : this._items.Contains(item);
  }

  public PythonTuple __reduce__()
  {
    return SetStorage.Reduce(this._items, this.GetType() != typeof (FrozenSetCollection) ? DynamicHelpers.GetPythonType((object) this) : TypeCache.FrozenSet);
  }

  private int CalculateHashCode(IEqualityComparer comparer)
  {
    FrozenSetCollection.HashCache hashCache = this._hashCache;
    if (hashCache != null && comparer == hashCache.Comparer)
      return hashCache.HashCode;
    int hashCode = SetStorage.GetHashCode(this._items, comparer);
    this._hashCache = new FrozenSetCollection.HashCache(hashCode, comparer);
    return hashCode;
  }

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    return this.CalculateHashCode(comparer);
  }

  bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
  {
    SetStorage items;
    return SetStorage.GetItemsIfSet(other, out items) && SetStorage.Equals(this._items, items, comparer);
  }

  public bool __eq__(object other)
  {
    SetStorage items;
    return SetStorage.GetItemsIfSet(other, out items) && this._items.Count == items.Count && this._items.IsSubset(items);
  }

  public bool __ne__(object other)
  {
    SetStorage items;
    return !SetStorage.GetItemsIfSet(other, out items) || this._items.Count != items.Count || !this._items.IsSubset(items);
  }

  public bool isdisjoint(FrozenSetCollection set) => this._items.IsDisjoint(set._items);

  public bool isdisjoint(SetCollection set) => this._items.IsDisjoint(set._items);

  public bool isdisjoint(object set) => this._items.IsDisjoint(SetStorage.GetItems(set));

  public bool issubset(FrozenSetCollection set) => this._items.IsSubset(set._items);

  public bool issubset(SetCollection set) => this._items.IsSubset(set._items);

  public bool issubset(object set) => this._items.IsSubset(SetStorage.GetItems(set));

  public bool issuperset(FrozenSetCollection set) => set._items.IsSubset(this._items);

  public bool issuperset(SetCollection set) => set._items.IsSubset(this._items);

  public bool issuperset(object set) => SetStorage.GetItems(set).IsSubset(this._items);

  public FrozenSetCollection union() => this.Make(this._items);

  public FrozenSetCollection union(FrozenSetCollection set)
  {
    return this.Make(SetStorage.Union(this._items, set._items));
  }

  public FrozenSetCollection union(SetCollection set)
  {
    return this.Make(SetStorage.Union(this._items, set._items));
  }

  public FrozenSetCollection union(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.Union(this._items, items);
    else
      items.UnionUpdate(this._items);
    return this.Make(items);
  }

  public FrozenSetCollection union([NotNull] params object[] sets)
  {
    SetStorage items = this._items.Clone();
    foreach (object set in sets)
      items.UnionUpdate(SetStorage.GetItems(set));
    return this.Make(items);
  }

  public FrozenSetCollection intersection() => this.Make(this._items);

  public FrozenSetCollection intersection(FrozenSetCollection set)
  {
    return this.Make(SetStorage.Intersection(this._items, set._items));
  }

  public FrozenSetCollection intersection(SetCollection set)
  {
    return this.Make(SetStorage.Intersection(this._items, set._items));
  }

  public FrozenSetCollection intersection(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.Intersection(this._items, items);
    else
      items.IntersectionUpdate(this._items);
    return this.Make(items);
  }

  public FrozenSetCollection intersection([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return this.Make(this._items);
    SetStorage items = this._items;
    foreach (object set in sets)
    {
      SetStorage x = items;
      SetStorage setStorage;
      ref SetStorage local = ref setStorage;
      SetStorage y;
      if (SetStorage.GetItems(set, out local))
      {
        y = setStorage;
        SetStorage.SortBySize(ref x, ref y);
        if (x == setStorage || x == this._items)
          x = x.Clone();
      }
      else
      {
        y = setStorage;
        SetStorage.SortBySize(ref x, ref y);
        if (x == this._items)
          x = x.Clone();
      }
      x.IntersectionUpdate(y);
      items = x;
    }
    return this.Make(items);
  }

  public FrozenSetCollection difference() => this.Make(this._items);

  public FrozenSetCollection difference(FrozenSetCollection set)
  {
    return set == this ? this.Empty : this.Make(SetStorage.Difference(this._items, set._items));
  }

  public FrozenSetCollection difference(SetCollection set)
  {
    return this.Make(SetStorage.Difference(this._items, set._items));
  }

  public FrozenSetCollection difference(object set)
  {
    return this.Make(SetStorage.Difference(this._items, SetStorage.GetItems(set)));
  }

  public FrozenSetCollection difference([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return this.Make(this._items);
    SetStorage items1 = this._items;
    foreach (object set in sets)
    {
      if (set == this)
        return this.Empty;
      SetStorage items2 = SetStorage.GetItems(set);
      if (items1 == this._items)
        items1 = SetStorage.Difference(this._items, items2);
      else
        items1.DifferenceUpdate(items2);
    }
    return this.Make(items1);
  }

  public FrozenSetCollection symmetric_difference(FrozenSetCollection set)
  {
    return set == this ? this.Empty : this.Make(SetStorage.SymmetricDifference(this._items, set._items));
  }

  public FrozenSetCollection symmetric_difference(SetCollection set)
  {
    return this.Make(SetStorage.SymmetricDifference(this._items, set._items));
  }

  public FrozenSetCollection symmetric_difference(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.SymmetricDifference(this._items, items);
    else
      items.SymmetricDifferenceUpdate(this._items);
    return this.Make(items);
  }

  public static FrozenSetCollection operator |(FrozenSetCollection x, FrozenSetCollection y)
  {
    return x.union(y);
  }

  public static FrozenSetCollection operator &(FrozenSetCollection x, FrozenSetCollection y)
  {
    return x.intersection(y);
  }

  public static FrozenSetCollection operator ^(FrozenSetCollection x, FrozenSetCollection y)
  {
    return x.symmetric_difference(y);
  }

  public static FrozenSetCollection operator -(FrozenSetCollection x, FrozenSetCollection y)
  {
    return x.difference(y);
  }

  public static FrozenSetCollection operator |(FrozenSetCollection x, SetCollection y)
  {
    return x.union(y);
  }

  public static FrozenSetCollection operator &(FrozenSetCollection x, SetCollection y)
  {
    return x.intersection(y);
  }

  public static FrozenSetCollection operator ^(FrozenSetCollection x, SetCollection y)
  {
    return x.symmetric_difference(y);
  }

  public static FrozenSetCollection operator -(FrozenSetCollection x, SetCollection y)
  {
    return x.difference(y);
  }

  public static bool operator >(FrozenSetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return items.IsStrictSubset(self._items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator <(FrozenSetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return self._items.IsStrictSubset(items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator >=(FrozenSetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return items.IsSubset(self._items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator <=(FrozenSetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return self._items.IsSubset(items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  [SpecialName]
  public int Compare(object o) => throw PythonOps.TypeError("cannot compare sets using cmp()");

  public int __cmp__(object o) => throw PythonOps.TypeError("cannot compare sets using cmp()");

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new SetIterator(this._items, false);

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new SetIterator(this._items, false);
  }

  public virtual string __repr__(CodeContext context)
  {
    return SetStorage.SetToString(context, (object) this, this._items);
  }

  void ICollection.CopyTo(Array array, int index)
  {
    int num = 0;
    foreach (object obj in (IEnumerable<object>) this)
      array.SetValue(obj, index + num++);
  }

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._items.Count;
  }

  bool ICollection.IsSynchronized => false;

  object ICollection.SyncRoot => (object) this;

  private sealed class HashCache
  {
    internal readonly int HashCode;
    internal readonly IEqualityComparer Comparer;

    internal HashCache(int hashCode, IEqualityComparer comparer)
    {
      this.HashCode = hashCode;
      this.Comparer = comparer;
    }
  }
}
