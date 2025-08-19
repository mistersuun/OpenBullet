// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.CollectionExtensions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal static class CollectionExtensions
{
  internal static T[] RemoveFirst<T>(this T[] array)
  {
    T[] destinationArray = new T[array.Length - 1];
    Array.Copy((Array) array, 1, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
  }

  internal static T[] AddFirst<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    array[0] = item;
    list.CopyTo(array, 1);
    return array;
  }

  internal static T[] ToArray<T>(this IList<T> list)
  {
    T[] array = new T[list.Count];
    list.CopyTo(array, 0);
    return array;
  }

  internal static T[] AddLast<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    list.CopyTo(array, 0);
    array[list.Count] = item;
    return array;
  }
}
