// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ArrayUtils
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class ArrayUtils
{
  public static readonly string[] EmptyStrings = new string[0];
  public static readonly object[] EmptyObjects = new object[0];

  public static IComparer<T> ToComparer<T>(Comparison<T> comparison)
  {
    return (IComparer<T>) new ArrayUtils.FunctorComparer<T>(comparison);
  }

  public static void PrintTable(StringBuilder output, string[,] table)
  {
    ContractUtils.RequiresNotNull((object) output, nameof (output));
    ContractUtils.RequiresNotNull((object) table, nameof (table));
    int num = 0;
    for (int index = 0; index < table.GetLength(0); ++index)
    {
      if (table[index, 0].Length > num)
        num = table[index, 0].Length;
    }
    for (int index = 0; index < table.GetLength(0); ++index)
    {
      output.Append(" ");
      output.Append(table[index, 0]);
      for (int length = table[index, 0].Length; length < num + 1; ++length)
        output.Append(' ');
      output.AppendLine(table[index, 1]);
    }
  }

  public static T[] Copy<T>(T[] array) => array.Length == 0 ? array : (T[]) array.Clone();

  public static T[] MakeArray<T>(ICollection<T> list)
  {
    if (list.Count == 0)
      return new T[0];
    T[] array = new T[list.Count];
    list.CopyTo(array, 0);
    return array;
  }

  public static T[] MakeArray<T>(
    ICollection<T> elements,
    int reservedSlotsBefore,
    int reservedSlotsAfter)
  {
    if (reservedSlotsAfter < 0)
      throw new ArgumentOutOfRangeException(nameof (reservedSlotsAfter));
    if (reservedSlotsBefore < 0)
      throw new ArgumentOutOfRangeException(nameof (reservedSlotsBefore));
    if (elements == null)
      return new T[reservedSlotsBefore + reservedSlotsAfter];
    T[] array = new T[reservedSlotsBefore + elements.Count + reservedSlotsAfter];
    elements.CopyTo(array, reservedSlotsBefore);
    return array;
  }

  public static T[] RotateRight<T>(T[] array, int count)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    if (count < 0 || count > array.Length)
      throw new ArgumentOutOfRangeException(nameof (count));
    T[] destinationArray = new T[array.Length];
    int num = array.Length - count;
    Array.Copy((Array) array, 0, (Array) destinationArray, count, num);
    Array.Copy((Array) array, num, (Array) destinationArray, 0, count);
    return destinationArray;
  }

  public static T[] ShiftRight<T>(T[] array, int count)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    if (count < 0)
      throw new ArgumentOutOfRangeException(nameof (count));
    T[] destinationArray = new T[array.Length + count];
    Array.Copy((Array) array, 0, (Array) destinationArray, count, array.Length);
    return destinationArray;
  }

  public static T[] ShiftLeft<T>(T[] array, int count)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    if (count < 0)
      throw new ArgumentOutOfRangeException(nameof (count));
    T[] destinationArray = new T[array.Length - count];
    Array.Copy((Array) array, count, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
  }

  public static T[] Insert<T>(T item, IList<T> list)
  {
    T[] array = new T[list.Count + 1];
    array[0] = item;
    list.CopyTo(array, 1);
    return array;
  }

  public static T[] Insert<T>(T item1, T item2, IList<T> list)
  {
    T[] array = new T[list.Count + 2];
    array[0] = item1;
    array[1] = item2;
    list.CopyTo(array, 2);
    return array;
  }

  public static T[] Insert<T>(T item, T[] array)
  {
    T[] objArray = ArrayUtils.ShiftRight<T>(array, 1);
    objArray[0] = item;
    return objArray;
  }

  public static T[] Insert<T>(T item1, T item2, T[] array)
  {
    T[] objArray = ArrayUtils.ShiftRight<T>(array, 2);
    objArray[0] = item1;
    objArray[1] = item2;
    return objArray;
  }

  public static T[] Append<T>(T[] array, T item)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    Array.Resize<T>(ref array, array.Length + 1);
    array[array.Length - 1] = item;
    return array;
  }

  public static T[] AppendRange<T>(T[] array, IList<T> items)
  {
    return ArrayUtils.AppendRange<T>(array, items, 0);
  }

  public static T[] AppendRange<T>(T[] array, IList<T> items, int additionalItemCount)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    if (additionalItemCount < 0)
      throw new ArgumentOutOfRangeException(nameof (additionalItemCount));
    int length = array.Length;
    Array.Resize<T>(ref array, array.Length + items.Count + additionalItemCount);
    int index = 0;
    while (index < items.Count)
    {
      array[length] = items[index];
      ++index;
      ++length;
    }
    return array;
  }

  public static T[,] Concatenate<T>(T[,] array1, T[,] array2)
  {
    int length1 = array1.GetLength(1);
    int length2 = array1.GetLength(0);
    int length3 = array2.GetLength(0);
    T[,] objArray = new T[length2 + length3, length1];
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        objArray[index1, index2] = array1[index1, index2];
    }
    for (int index3 = 0; index3 < length3; ++index3)
    {
      for (int index4 = 0; index4 < length1; ++index4)
        objArray[index3 + length2, index4] = array2[index3, index4];
    }
    return objArray;
  }

  public static void SwapLastTwo<T>(T[] array)
  {
    T obj = array[array.Length - 1];
    array[array.Length - 1] = array[array.Length - 2];
    array[array.Length - 2] = obj;
  }

  public static T[] RemoveFirst<T>(IList<T> list)
  {
    return ArrayUtils.ShiftLeft<T>(ArrayUtils.MakeArray<T>((ICollection<T>) list), 1);
  }

  public static T[] RemoveFirst<T>(T[] array) => ArrayUtils.ShiftLeft<T>(array, 1);

  public static T[] RemoveLast<T>(T[] array)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    Array.Resize<T>(ref array, array.Length - 1);
    return array;
  }

  public static T[] RemoveAt<T>(IList<T> list, int indexToRemove)
  {
    return ArrayUtils.RemoveAt<T>(ArrayUtils.MakeArray<T>((ICollection<T>) list), indexToRemove);
  }

  public static T[] RemoveAt<T>(T[] array, int indexToRemove)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    ContractUtils.Requires(indexToRemove >= 0 && indexToRemove < array.Length, nameof (indexToRemove));
    T[] destinationArray = new T[array.Length - 1];
    if (indexToRemove > 0)
      Array.Copy((Array) array, 0, (Array) destinationArray, 0, indexToRemove);
    int length = array.Length - indexToRemove - 1;
    if (length > 0)
      Array.Copy((Array) array, array.Length - length, (Array) destinationArray, destinationArray.Length - length, length);
    return destinationArray;
  }

  public static T[] InsertAt<T>(IList<T> list, int index, params T[] items)
  {
    return ArrayUtils.InsertAt<T>(ArrayUtils.MakeArray<T>((ICollection<T>) list), index, items);
  }

  public static T[] InsertAt<T>(T[] array, int index, params T[] items)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    ContractUtils.RequiresNotNull((object) items, nameof (items));
    ContractUtils.Requires(index >= 0 && index <= array.Length, nameof (index));
    if (items.Length == 0)
      return ArrayUtils.Copy<T>(array);
    T[] destinationArray = new T[array.Length + items.Length];
    if (index > 0)
      Array.Copy((Array) array, 0, (Array) destinationArray, 0, index);
    Array.Copy((Array) items, 0, (Array) destinationArray, index, items.Length);
    int length = array.Length - index;
    if (length > 0)
      Array.Copy((Array) array, array.Length - length, (Array) destinationArray, destinationArray.Length - length, length);
    return destinationArray;
  }

  public static T[] ToArray<T>(ICollection<T> list)
  {
    if (!(list is T[] array))
    {
      array = new T[list.Count];
      int num = 0;
      foreach (T obj in (IEnumerable<T>) list)
        array[num++] = obj;
    }
    return array;
  }

  public static bool ValueEquals<T>(this T[] array, T[] other)
  {
    if (other.Length != array.Length)
      return false;
    for (int index = 0; index < array.Length; ++index)
    {
      if (!object.Equals((object) array[index], (object) other[index]))
        return false;
    }
    return true;
  }

  public static T[] Reverse<T>(this T[] array)
  {
    T[] objArray = new T[array.Length];
    for (int index = 0; index < array.Length; ++index)
      objArray[array.Length - index - 1] = array[index];
    return objArray;
  }

  internal sealed class FunctorComparer<T> : IComparer<T>
  {
    private readonly Comparison<T> _comparison;

    public FunctorComparer(Comparison<T> comparison) => this._comparison = comparison;

    public int Compare(T x, T y) => this._comparison(x, y);
  }
}
