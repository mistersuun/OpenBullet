// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CollectionUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class CollectionUtils
{
  public static IEnumerable<T> Cast<S, T>(this IEnumerable<S> sequence) where S : T
  {
    return (IEnumerable<T>) sequence;
  }

  public static IEnumerable<TSuper> ToCovariant<T, TSuper>(IEnumerable<T> enumerable) where T : TSuper
  {
    return (IEnumerable<TSuper>) enumerable;
  }

  public static void AddRange<T>(ICollection<T> collection, IEnumerable<T> items)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) items, nameof (items));
    if (collection is List<T> objList)
    {
      objList.AddRange(items);
    }
    else
    {
      foreach (T obj in items)
        collection.Add(obj);
    }
  }

  public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
  {
    foreach (T obj in items)
      list.Add(obj);
  }

  public static IEnumerable<T> ToEnumerable<T>(IEnumerable enumerable)
  {
    foreach (T obj in enumerable)
      yield return obj;
  }

  public static IEnumerator<TSuper> ToCovariant<T, TSuper>(IEnumerator<T> enumerator) where T : TSuper
  {
    ContractUtils.RequiresNotNull((object) enumerator, nameof (enumerator));
    while (enumerator.MoveNext())
      yield return (TSuper) enumerator.Current;
  }

  public static IDictionaryEnumerator ToDictionaryEnumerator(
    IEnumerator<KeyValuePair<object, object>> enumerator)
  {
    return (IDictionaryEnumerator) new CollectionUtils.DictionaryEnumerator(enumerator);
  }

  public static List<T> MakeList<T>(T item)
  {
    return new List<T>() { item };
  }

  public static int CountOf<T>(IList<T> list, T item) where T : IEquatable<T>
  {
    if (list == null)
      return 0;
    int num = 0;
    for (int index = 0; index < list.Count; ++index)
    {
      if (list[index].Equals(item))
        ++num;
    }
    return num;
  }

  public static int Max(this IEnumerable<int> values)
  {
    ContractUtils.RequiresNotNull((object) values, nameof (values));
    int num1 = int.MinValue;
    foreach (int num2 in values)
    {
      if (num2 > num1)
        num1 = num2;
    }
    return num1;
  }

  public static bool TrueForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) predicate, nameof (predicate));
    foreach (T obj in collection)
    {
      if (!predicate(obj))
        return false;
    }
    return true;
  }

  public static IList<TRet> ConvertAll<T, TRet>(IList<T> collection, Func<T, TRet> predicate)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) predicate, nameof (predicate));
    List<TRet> retList = new List<TRet>(collection.Count);
    foreach (T obj in (IEnumerable<T>) collection)
      retList.Add(predicate(obj));
    return (IList<TRet>) retList;
  }

  public static IEnumerable<TRet> Select<TRet>(
    this IEnumerable enumerable,
    Func<object, TRet> selector)
  {
    ContractUtils.RequiresNotNull((object) enumerable, nameof (enumerable));
    ContractUtils.RequiresNotNull((object) selector, nameof (selector));
    foreach (object obj in enumerable)
      yield return selector(obj);
  }

  public static List<T> GetRange<T>(IList<T> list, int index, int count)
  {
    ContractUtils.RequiresNotNull((object) list, nameof (list));
    ContractUtils.RequiresArrayRange<T>(list, index, count, nameof (index), nameof (count));
    List<T> range = new List<T>(count);
    int num = index + count;
    for (int index1 = index; index1 < num; ++index1)
      range.Add(list[index1]);
    return range;
  }

  public static void InsertRange<T>(IList<T> collection, int index, IEnumerable<T> items)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) items, nameof (items));
    ContractUtils.RequiresArrayInsertIndex<T>(collection, index, nameof (index));
    if (collection is List<T> objList)
    {
      objList.InsertRange(index, items);
    }
    else
    {
      int num = index;
      foreach (T obj in items)
        collection.Insert(num++, obj);
    }
  }

  public static void RemoveRange<T>(IList<T> collection, int index, int count)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresArrayRange<T>(collection, index, count, nameof (index), nameof (count));
    if (collection is List<T> objList)
    {
      objList.RemoveRange(index, count);
    }
    else
    {
      for (int index1 = index + count - 1; index1 >= index; --index1)
        collection.RemoveAt(index1);
    }
  }

  public static int FindIndex<T>(this IList<T> collection, Predicate<T> predicate)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) predicate, nameof (predicate));
    for (int index = 0; index < collection.Count; ++index)
    {
      if (predicate(collection[index]))
        return index;
    }
    return -1;
  }

  public static IList<T> ToSortedList<T>(this ICollection<T> collection, Comparison<T> comparison)
  {
    ContractUtils.RequiresNotNull((object) collection, nameof (collection));
    ContractUtils.RequiresNotNull((object) comparison, nameof (comparison));
    T[] array = new T[collection.Count];
    collection.CopyTo(array, 0);
    Array.Sort<T>(array, comparison);
    return (IList<T>) array;
  }

  public static T[] ToReverseArray<T>(this IList<T> list)
  {
    ContractUtils.RequiresNotNull((object) list, nameof (list));
    T[] reverseArray = new T[list.Count];
    for (int index = 0; index < reverseArray.Length; ++index)
      reverseArray[index] = list[reverseArray.Length - 1 - index];
    return reverseArray;
  }

  public static IEqualityComparer<HashSet<T>> CreateSetComparer<T>()
  {
    return HashSet<T>.CreateSetComparer();
  }

  private class CovariantConvertor<T, TSuper> : IEnumerable<TSuper>, IEnumerable where T : TSuper
  {
    private IEnumerable<T> _enumerable;

    public CovariantConvertor(IEnumerable<T> enumerable)
    {
      ContractUtils.RequiresNotNull((object) enumerable, nameof (enumerable));
      this._enumerable = enumerable;
    }

    public IEnumerator<TSuper> GetEnumerator()
    {
      return CollectionUtils.ToCovariant<T, TSuper>(this._enumerable.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }

  private sealed class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
  {
    private readonly IEnumerator<KeyValuePair<object, object>> _enumerator;

    public DictionaryEnumerator(
      IEnumerator<KeyValuePair<object, object>> enumerator)
    {
      this._enumerator = enumerator;
    }

    public DictionaryEntry Entry
    {
      get
      {
        KeyValuePair<object, object> current = this._enumerator.Current;
        object key = current.Key;
        current = this._enumerator.Current;
        object obj = current.Value;
        return new DictionaryEntry(key, obj);
      }
    }

    public object Key => this._enumerator.Current.Key;

    public object Value => this._enumerator.Current.Value;

    public object Current => (object) this.Entry;

    public bool MoveNext() => this._enumerator.MoveNext();

    public void Reset() => this._enumerator.Reset();
  }
}
