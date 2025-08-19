// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CollectionExtensions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

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

  internal static int ListHashCode<T>(this IEnumerable<T> list)
  {
    EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
    int num = 6551;
    foreach (T obj in list)
      num ^= num << 5 ^ equalityComparer.GetHashCode(obj);
    return num;
  }

  internal static bool ListEquals<T>(this ICollection<T> first, ICollection<T> second)
  {
    if (first.Count != second.Count)
      return false;
    EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
    using (IEnumerator<T> enumerator1 = first.GetEnumerator())
    {
      using (IEnumerator<T> enumerator2 = second.GetEnumerator())
      {
        while (enumerator1.MoveNext())
        {
          enumerator2.MoveNext();
          if (!equalityComparer.Equals(enumerator1.Current, enumerator2.Current))
            return false;
        }
        return true;
      }
    }
  }

  internal static U[] Map<T, U>(this ICollection<T> collection, Func<T, U> select)
  {
    U[] uArray = new U[collection.Count];
    int num = 0;
    foreach (T obj in (IEnumerable<T>) collection)
      uArray[num++] = select(obj);
    return uArray;
  }

  internal static T[] RemoveFirst<T>(this T[] array)
  {
    T[] destinationArray = new T[array.Length - 1];
    Array.Copy((Array) array, 1, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
  }

  internal static T[] RemoveLast<T>(this T[] array)
  {
    T[] destinationArray = new T[array.Length - 1];
    Array.Copy((Array) array, 0, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
  }

  internal static T[] AddFirst<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    array[0] = item;
    list.CopyTo(array, 1);
    return array;
  }

  internal static T[] AddLast<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    list.CopyTo(array, 0);
    array[list.Count] = item;
    return array;
  }

  internal static T[] RemoveAt<T>(this T[] array, int indexToRemove)
  {
    T[] destinationArray = new T[array.Length - 1];
    if (indexToRemove > 0)
      Array.Copy((Array) array, 0, (Array) destinationArray, 0, indexToRemove);
    int length = array.Length - indexToRemove - 1;
    if (length > 0)
      Array.Copy((Array) array, array.Length - length, (Array) destinationArray, destinationArray.Length - length, length);
    return destinationArray;
  }

  internal static T[] RotateRight<T>(this T[] array, int count)
  {
    T[] destinationArray = new T[array.Length];
    int num = array.Length - count;
    Array.Copy((Array) array, 0, (Array) destinationArray, count, num);
    Array.Copy((Array) array, num, (Array) destinationArray, 0, count);
    return destinationArray;
  }
}
