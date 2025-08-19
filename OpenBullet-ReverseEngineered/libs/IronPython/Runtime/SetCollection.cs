// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SetCollection
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
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("set")]
[DebuggerDisplay("set, {Count} items", TargetTypeName = "set")]
[DebuggerTypeProxy(typeof (CollectionDebugProxy))]
public class SetCollection : 
  IEnumerable,
  IEnumerable<object>,
  ICollection,
  IStructuralEquatable,
  ICodeFormattable
{
  internal SetStorage _items;
  public const object __hash__ = null;

  public void __init__() => this.clear();

  public void __init__([NotNull] SetCollection set) => this._items = set._items.Clone();

  public void __init__([NotNull] FrozenSetCollection set) => this._items = set._items.Clone();

  public void __init__(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      this._items = items.Clone();
    else
      this._items = items;
  }

  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Set ? (object) new SetCollection() : cls.CreateInstance(context);
  }

  public static object __new__(CodeContext context, PythonType cls, object arg)
  {
    return SetCollection.__new__(context, cls);
  }

  public static object __new__(CodeContext context, PythonType cls, params object[] argsø)
  {
    return SetCollection.__new__(context, cls);
  }

  public static object __new__(
    CodeContext context,
    PythonType cls,
    [ParamDictionary] IDictionary<object, object> kwArgs,
    params object[] argsø)
  {
    return SetCollection.__new__(context, cls);
  }

  public SetCollection() => this._items = new SetStorage();

  internal SetCollection(SetStorage items) => this._items = items;

  private SetCollection Empty
  {
    get
    {
      return this.GetType() == typeof (SetCollection) ? new SetCollection() : SetCollection.Make(DynamicHelpers.GetPythonType((object) this), new SetStorage());
    }
  }

  internal SetCollection(object[] items)
  {
    this._items = new SetStorage(items.Length);
    foreach (object obj in items)
      this._items.AddNoLock(obj);
  }

  private SetCollection Make(SetStorage items)
  {
    return this.GetType() == typeof (SetCollection) ? new SetCollection(items) : SetCollection.Make(DynamicHelpers.GetPythonType((object) this), items);
  }

  private static SetCollection Make(PythonType cls, SetStorage items)
  {
    if (cls == TypeCache.Set)
      return new SetCollection(items);
    SetCollection setCollection = PythonCalls.Call((object) cls) as SetCollection;
    if (items.Count > 0)
      setCollection._items = items;
    return setCollection;
  }

  internal static SetCollection Make(PythonType cls, object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = items.Clone();
    return SetCollection.Make(cls, items);
  }

  public SetCollection copy() => this.Make(this._items.Clone());

  public int __len__() => this.Count;

  public bool __contains__(object item)
  {
    return !SetStorage.GetHashableSetIfSet(ref item) ? this._items.ContainsAlwaysHash(item) : this._items.Contains(item);
  }

  public PythonTuple __reduce__()
  {
    return SetStorage.Reduce(this._items, this.GetType() != typeof (SetCollection) ? DynamicHelpers.GetPythonType((object) this) : TypeCache.Set);
  }

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    if (CompareUtil.Check((object) this))
      return 0;
    CompareUtil.Push((object) this);
    try
    {
      return ((IStructuralEquatable) new FrozenSetCollection(this._items)).GetHashCode(comparer);
    }
    finally
    {
      CompareUtil.Pop((object) this);
    }
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

  public void add(object item) => this._items.Add(item);

  public void clear() => this._items.Clear();

  public void discard(object item)
  {
    SetStorage.GetHashableSetIfSet(ref item);
    this._items.Remove(item);
  }

  public object pop()
  {
    object obj;
    if (this._items.Pop(out obj))
      return obj;
    throw PythonOps.KeyError("pop from an empty set");
  }

  public void remove(object item)
  {
    object o = item;
    if (!(!SetStorage.GetHashableSetIfSet(ref o) ? this._items.RemoveAlwaysHash(o) : this._items.Remove(o)))
      throw PythonOps.KeyError(item);
  }

  public void update(SetCollection set)
  {
    if (set == this)
      return;
    lock (this._items)
      this._items.UnionUpdate(set._items);
  }

  public void update(FrozenSetCollection set)
  {
    lock (this._items)
      this._items.UnionUpdate(set._items);
  }

  public void update(object set)
  {
    if (set == this)
      return;
    SetStorage items = SetStorage.GetItems(set);
    lock (this._items)
      this._items.UnionUpdate(items);
  }

  public void update([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return;
    lock (this._items)
    {
      foreach (object set in sets)
      {
        if (set != this)
          this._items.UnionUpdate(SetStorage.GetItems(set));
      }
    }
  }

  public void intersection_update(SetCollection set)
  {
    if (set == this)
      return;
    lock (this._items)
      this._items.IntersectionUpdate(set._items);
  }

  public void intersection_update(FrozenSetCollection set)
  {
    lock (this._items)
      this._items.IntersectionUpdate(set._items);
  }

  public void intersection_update(object set)
  {
    if (set == this)
      return;
    SetStorage items = SetStorage.GetItems(set);
    lock (this._items)
      this._items.IntersectionUpdate(items);
  }

  public void intersection_update([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return;
    lock (this._items)
    {
      foreach (object set in sets)
      {
        if (set != this)
          this._items.IntersectionUpdate(SetStorage.GetItems(set));
      }
    }
  }

  public void difference_update(SetCollection set)
  {
    if (set == this)
    {
      this._items.Clear();
    }
    else
    {
      lock (this._items)
        this._items.DifferenceUpdate(set._items);
    }
  }

  public void difference_update(FrozenSetCollection set)
  {
    lock (this._items)
      this._items.DifferenceUpdate(set._items);
  }

  public void difference_update(object set)
  {
    if (set == this)
    {
      this._items.Clear();
    }
    else
    {
      SetStorage items = SetStorage.GetItems(set);
      lock (this._items)
        this._items.DifferenceUpdate(items);
    }
  }

  public void difference_update([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return;
    lock (this._items)
    {
      foreach (object set in sets)
      {
        if (set == this)
        {
          this._items.ClearNoLock();
          break;
        }
        this._items.DifferenceUpdate(SetStorage.GetItems(set));
      }
    }
  }

  public void symmetric_difference_update(SetCollection set)
  {
    if (set == this)
    {
      this._items.Clear();
    }
    else
    {
      lock (this._items)
        this._items.SymmetricDifferenceUpdate(set._items);
    }
  }

  public void symmetric_difference_update(FrozenSetCollection set)
  {
    lock (this._items)
      this._items.SymmetricDifferenceUpdate(set._items);
  }

  public void symmetric_difference_update(object set)
  {
    if (set == this)
    {
      this._items.Clear();
    }
    else
    {
      SetStorage items = SetStorage.GetItems(set);
      lock (this._items)
        this._items.SymmetricDifferenceUpdate(items);
    }
  }

  public bool isdisjoint(SetCollection set) => this._items.IsDisjoint(set._items);

  public bool isdisjoint(FrozenSetCollection set) => this._items.IsDisjoint(set._items);

  public bool isdisjoint(object set) => this._items.IsDisjoint(SetStorage.GetItems(set));

  public bool issubset(SetCollection set) => this._items.IsSubset(set._items);

  public bool issubset(FrozenSetCollection set) => this._items.IsSubset(set._items);

  public bool issubset(object set) => this._items.IsSubset(SetStorage.GetItems(set));

  public bool issuperset(SetCollection set) => set._items.IsSubset(this._items);

  public bool issuperset(FrozenSetCollection set) => set._items.IsSubset(this._items);

  public bool issuperset(object set) => SetStorage.GetItems(set).IsSubset(this._items);

  public SetCollection union() => this.copy();

  public SetCollection union(SetCollection set)
  {
    return this.Make(SetStorage.Union(this._items, set._items));
  }

  public SetCollection union(FrozenSetCollection set)
  {
    return this.Make(SetStorage.Union(this._items, set._items));
  }

  public SetCollection union(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.Union(this._items, items);
    else
      items.UnionUpdate(this._items);
    return this.Make(items);
  }

  public SetCollection union([NotNull] params object[] sets)
  {
    SetStorage items = this._items.Clone();
    foreach (object set in sets)
      items.UnionUpdate(SetStorage.GetItems(set));
    return this.Make(items);
  }

  public SetCollection intersection() => this.copy();

  public SetCollection intersection(SetCollection set)
  {
    return this.Make(SetStorage.Intersection(this._items, set._items));
  }

  public SetCollection intersection(FrozenSetCollection set)
  {
    return this.Make(SetStorage.Intersection(this._items, set._items));
  }

  public SetCollection intersection(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.Intersection(this._items, items);
    else
      items.IntersectionUpdate(this._items);
    return this.Make(items);
  }

  public SetCollection intersection([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return this.copy();
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

  public SetCollection difference() => this.copy();

  public SetCollection difference(SetCollection set)
  {
    return set == this ? this.Empty : this.Make(SetStorage.Difference(this._items, set._items));
  }

  public SetCollection difference(FrozenSetCollection set)
  {
    return this.Make(SetStorage.Difference(this._items, set._items));
  }

  public SetCollection difference(object set)
  {
    return this.Make(SetStorage.Difference(this._items, SetStorage.GetItems(set)));
  }

  public SetCollection difference([NotNull] params object[] sets)
  {
    if (sets.Length == 0)
      return this.copy();
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

  public SetCollection symmetric_difference(SetCollection set)
  {
    return set == this ? this.Empty : this.Make(SetStorage.SymmetricDifference(this._items, set._items));
  }

  public SetCollection symmetric_difference(FrozenSetCollection set)
  {
    return this.Make(SetStorage.SymmetricDifference(this._items, set._items));
  }

  public SetCollection symmetric_difference(object set)
  {
    SetStorage items;
    if (SetStorage.GetItems(set, out items))
      items = SetStorage.SymmetricDifference(this._items, items);
    else
      items.SymmetricDifferenceUpdate(this._items);
    return this.Make(items);
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseOr(SetCollection set)
  {
    this.update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseOr(FrozenSetCollection set)
  {
    this.update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseOr(object set)
  {
    switch (set)
    {
      case FrozenSetCollection _:
      case SetCollection _:
        this.update(set);
        return this;
      default:
        throw PythonOps.TypeError("unsupported operand type(s) for |=: '{0}' and '{1}'", (object) PythonTypeOps.GetName((object) this), (object) PythonTypeOps.GetName(set));
    }
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseAnd(SetCollection set)
  {
    this.intersection_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseAnd(FrozenSetCollection set)
  {
    this.intersection_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceBitwiseAnd(object set)
  {
    switch (set)
    {
      case FrozenSetCollection _:
      case SetCollection _:
        this.intersection_update(set);
        return this;
      default:
        throw PythonOps.TypeError("unsupported operand type(s) for &=: '{0}' and '{1}'", (object) PythonTypeOps.GetName((object) this), (object) PythonTypeOps.GetName(set));
    }
  }

  [SpecialName]
  public SetCollection InPlaceExclusiveOr(SetCollection set)
  {
    this.symmetric_difference_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceExclusiveOr(FrozenSetCollection set)
  {
    this.symmetric_difference_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceExclusiveOr(object set)
  {
    switch (set)
    {
      case FrozenSetCollection _:
      case SetCollection _:
        this.symmetric_difference_update(set);
        return this;
      default:
        throw PythonOps.TypeError("unsupported operand type(s) for ^=: '{0}' and '{1}'", (object) PythonTypeOps.GetName((object) this), (object) PythonTypeOps.GetName(set));
    }
  }

  [SpecialName]
  public SetCollection InPlaceSubtract(SetCollection set)
  {
    this.difference_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceSubtract(FrozenSetCollection set)
  {
    this.difference_update(set);
    return this;
  }

  [SpecialName]
  public SetCollection InPlaceSubtract(object set)
  {
    switch (set)
    {
      case FrozenSetCollection _:
      case SetCollection _:
        this.difference_update(set);
        return this;
      default:
        throw PythonOps.TypeError("unsupported operand type(s) for -=: '{0}' and '{1}'", (object) PythonTypeOps.GetName((object) this), (object) PythonTypeOps.GetName(set));
    }
  }

  public static SetCollection operator |(SetCollection x, SetCollection y) => x.union(y);

  public static SetCollection operator &(SetCollection x, SetCollection y) => x.intersection(y);

  public static SetCollection operator ^(SetCollection x, SetCollection y)
  {
    return x.symmetric_difference(y);
  }

  public static SetCollection operator -(SetCollection x, SetCollection y) => x.difference(y);

  public static SetCollection operator |(SetCollection x, FrozenSetCollection y) => x.union(y);

  public static SetCollection operator &(SetCollection x, FrozenSetCollection y)
  {
    return x.intersection(y);
  }

  public static SetCollection operator ^(SetCollection x, FrozenSetCollection y)
  {
    return x.symmetric_difference(y);
  }

  public static SetCollection operator -(SetCollection x, FrozenSetCollection y) => x.difference(y);

  public static bool operator >(SetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return items.IsStrictSubset(self._items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator <(SetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return self._items.IsStrictSubset(items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator >=(SetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return items.IsSubset(self._items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  public static bool operator <=(SetCollection self, object other)
  {
    SetStorage items;
    if (SetStorage.GetItemsIfSet(other, out items))
      return self._items.IsSubset(items);
    throw PythonOps.TypeError("can only compare to a set");
  }

  [SpecialName]
  public int Compare(object o) => throw PythonOps.TypeError("cannot compare sets using cmp()");

  public int __cmp__(object o) => throw PythonOps.TypeError("cannot compare sets using cmp()");

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new SetIterator(this._items, true);

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new SetIterator(this._items, true);
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
}
