// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.CollectionUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal static class CollectionUtils
{
  internal static T[] RemoveLast<T>(this T[] array)
  {
    T[] destinationArray = new T[array.Length - 1];
    Array.Copy((Array) array, 0, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
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

  internal static int ListHashCode<T>(this IEnumerable<T> list)
  {
    EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
    int num = 6551;
    foreach (T obj in list)
      num ^= num << 5 ^ equalityComparer.GetHashCode(obj);
    return num;
  }
}
