// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CollectionExtensions
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class CollectionExtensions
{
  internal static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> enumerable)
  {
    switch (enumerable)
    {
      case null:
        return EmptyReadOnlyCollection<T>.Instance;
      case ReadOnlyCollection<T> readOnlyCollection:
        return readOnlyCollection;
      case ICollection<T> objs:
        int count = objs.Count;
        if (count == 0)
          return EmptyReadOnlyCollection<T>.Instance;
        T[] objArray = new T[count];
        objs.CopyTo(objArray, 0);
        return new ReadOnlyCollection<T>((IList<T>) objArray);
      default:
        return new ReadOnlyCollection<T>((IList<T>) new List<T>(enumerable).ToArray());
    }
  }

  internal static T[] ToArray<T>(this IEnumerable<T> enumerable)
  {
    if (!(enumerable is ICollection<T> objs))
      return new List<T>(enumerable).ToArray();
    T[] array = new T[objs.Count];
    objs.CopyTo(array, 0);
    return array;
  }

  internal static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
  {
    foreach (T obj in source)
    {
      if (predicate(obj))
        return true;
    }
    return false;
  }

  internal static TSource Aggregate<TSource>(
    this IEnumerable<TSource> source,
    Func<TSource, TSource, TSource> func)
  {
    using (IEnumerator<TSource> enumerator = source.GetEnumerator())
    {
      TSource source1 = enumerator.MoveNext() ? enumerator.Current : throw new ArgumentException("Collection is empty", nameof (source));
      while (enumerator.MoveNext())
        source1 = func(source1, enumerator.Current);
      return source1;
    }
  }

  internal static T[] AddFirst<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    array[0] = item;
    list.CopyTo(array, 1);
    return array;
  }

  internal static bool TrueForAll<T>(this IEnumerable<T> collection, Predicate<T> predicate)
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
}
