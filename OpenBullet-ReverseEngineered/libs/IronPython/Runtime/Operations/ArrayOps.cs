// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ArrayOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ArrayOps
{
  [SpecialName]
  public static Array Add(Array data1, Array data2)
  {
    if (data1 == null)
      throw PythonOps.TypeError("expected array for 1st argument, got None");
    if (data2 == null)
      throw PythonOps.TypeError("expected array for 2nd argument, got None");
    if (data1.Rank > 1 || data2.Rank > 1)
      throw new NotImplementedException("can't add multidimensional arrays");
    Type type1 = data1.GetType();
    Type type2 = data2.GetType();
    Array instance = Array.CreateInstance(type1 == type2 ? type1.GetElementType() : typeof (object), data1.Length + data2.Length);
    Array.Copy(data1, 0, instance, 0, data1.Length);
    Array.Copy(data2, 0, instance, data1.Length, data2.Length);
    return instance;
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType pythonType, ICollection items)
  {
    Type elementType = pythonType.UnderlyingSystemType.GetElementType();
    Array instance = Array.CreateInstance(elementType, items.Count);
    int num = 0;
    foreach (object obj in (IEnumerable) items)
      instance.SetValue(Converter.Convert(obj, elementType), num++);
    return (object) instance;
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType pythonType, object items)
  {
    Type elementType = pythonType.UnderlyingSystemType.GetElementType();
    object ret;
    if (!PythonOps.TryGetBoundAttr(items, "__len__", out ret))
      throw PythonOps.TypeErrorForBadInstance("expected object with __len__ function, got {0}", items);
    int int32 = context.LanguageContext.ConvertToInt32(PythonOps.CallWithContext(context, ret));
    Array instance = Array.CreateInstance(elementType, int32);
    IEnumerator enumerator = PythonOps.GetEnumerator(items);
    int num = 0;
    while (enumerator.MoveNext())
      instance.SetValue(Converter.Convert(enumerator.Current, elementType), num++);
    return (object) instance;
  }

  [SpecialName]
  public static Array Multiply(Array data, int count)
  {
    Type elementType = data.Rank <= 1 ? data.GetType().GetElementType() : throw new NotImplementedException("can't multiply multidimensional arrays");
    if (count <= 0)
      return Array.CreateInstance(elementType, 0);
    int length1 = data.Length * count;
    Array instance = Array.CreateInstance(elementType, length1);
    Array.Copy(data, 0, instance, 0, data.Length);
    int length2 = data.Length;
    int length3 = data.Length;
    while (length3 < length1)
    {
      Array.Copy(instance, 0, instance, length3, Math.Min(length2, length1 - length3));
      length3 += length2;
      length2 *= 2;
    }
    return instance;
  }

  [SpecialName]
  public static object GetItem(Array data, int index)
  {
    if (data == null)
      throw PythonOps.TypeError("expected Array, got None");
    return data.GetValue(PythonOps.FixIndex(index, data.Length) + data.GetLowerBound(0));
  }

  [SpecialName]
  public static object GetItem(Array data, Slice slice)
  {
    if (data == null)
      throw PythonOps.TypeError("expected Array, got None");
    return (object) ArrayOps.GetSlice(data, data.Length, slice);
  }

  [SpecialName]
  public static object GetItem(Array data, PythonTuple tuple)
  {
    return data != null ? ArrayOps.GetItem(data, tuple.ToArray()) : throw PythonOps.TypeError("expected Array, got None");
  }

  [SpecialName]
  public static object GetItem(Array data, params object[] indices)
  {
    if (indices == null || indices.Length < 1)
      throw PythonOps.TypeError("__getitem__ requires at least 1 parameter");
    int result;
    if (indices.Length == 1 && Converter.TryConvertToInt32(indices[0], out result))
      return ArrayOps.GetItem(data, result);
    data.GetType();
    int[] indices1 = ArrayOps.TupleToIndices(data, (IList<object>) indices);
    if (data.Rank != indices.Length)
      throw PythonOps.ValueError("bad dimensions for array, got {0} expected {1}", (object) indices.Length, (object) data.Rank);
    for (int dimension = 0; dimension < indices1.Length; ++dimension)
      indices1[dimension] += data.GetLowerBound(dimension);
    return data.GetValue(indices1);
  }

  [SpecialName]
  public static void SetItem(Array data, int index, object value)
  {
    if (data == null)
      throw PythonOps.TypeError("expected Array, got None");
    data.SetValue(Converter.Convert(value, data.GetType().GetElementType()), PythonOps.FixIndex(index, data.Length) + data.GetLowerBound(0));
  }

  [SpecialName]
  public static void SetItem(Array a, params object[] indexAndValue)
  {
    if (indexAndValue == null || indexAndValue.Length < 2)
      throw PythonOps.TypeError("__setitem__ requires at least 2 parameters");
    int result;
    if (indexAndValue.Length == 2 && Converter.TryConvertToInt32(indexAndValue[0], out result))
    {
      ArrayOps.SetItem(a, result, indexAndValue[1]);
    }
    else
    {
      a.GetType();
      object[] tuple = ArrayUtils.RemoveLast<object>(indexAndValue);
      int[] indices = ArrayOps.TupleToIndices(a, (IList<object>) tuple);
      if (a.Rank != tuple.Length)
        throw PythonOps.ValueError("bad dimensions for array, got {0} expected {1}", (object) tuple.Length, (object) a.Rank);
      for (int dimension = 0; dimension < indices.Length; ++dimension)
        indices[dimension] += a.GetLowerBound(dimension);
      a.SetValue(indexAndValue[indexAndValue.Length - 1], indices);
    }
  }

  [SpecialName]
  public static void SetItem(Array a, Slice index, object value)
  {
    Type elm = a.Rank == 1 ? a.GetType().GetElementType() : throw PythonOps.NotImplementedError("slice on multi-dimensional array");
    index.DoSliceAssign((Slice.SliceAssign) ((idx, val) => a.SetValue(Converter.Convert(val, elm), idx + a.GetLowerBound(0))), a.Length, value);
  }

  public static string __repr__(CodeContext context, [NotNull] Array self)
  {
    List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) self);
    if (andCheckInfinite == null)
      return "...";
    int count = andCheckInfinite.Count;
    andCheckInfinite.Add((object) self);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (self.Rank == 1)
      {
        stringBuilder.Append("Array[");
        Type elementType = self.GetType().GetElementType();
        stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(elementType).Name);
        stringBuilder.Append("]");
        stringBuilder.Append("((");
        for (int index = 0; index < self.Length; ++index)
        {
          if (index > 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(PythonOps.Repr(context, self.GetValue(index + self.GetLowerBound(0))));
        }
        stringBuilder.Append("))");
      }
      else
      {
        stringBuilder.Append("<");
        stringBuilder.Append(self.Rank);
        stringBuilder.Append(" dimensional Array[");
        Type elementType = self.GetType().GetElementType();
        stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(elementType).Name);
        stringBuilder.Append("] at ");
        stringBuilder.Append(PythonOps.HexId((object) self));
        stringBuilder.Append(">");
      }
      return stringBuilder.ToString();
    }
    finally
    {
      andCheckInfinite.RemoveAt(count);
    }
  }

  internal static object[] Multiply(object[] data, int size, int count)
  {
    int newSize = checked (size * count);
    object[] objArray = ArrayOps.CopyArray(data, newSize);
    if (count > 0)
    {
      int val1 = size;
      int destinationIndex = size;
      while (destinationIndex < newSize)
      {
        Array.Copy((Array) objArray, 0, (Array) objArray, destinationIndex, Math.Min(val1, newSize - destinationIndex));
        destinationIndex += val1;
        val1 *= 2;
      }
    }
    return objArray;
  }

  internal static object[] Add(object[] data1, int size1, object[] data2, int size2)
  {
    object[] destinationArray = ArrayOps.CopyArray(data1, size1 + size2);
    Array.Copy((Array) data2, 0, (Array) destinationArray, size1, size2);
    return destinationArray;
  }

  internal static object[] GetSlice(object[] data, int start, int stop)
  {
    if (stop <= start)
      return ArrayUtils.EmptyObjects;
    object[] slice = new object[stop - start];
    int num = 0;
    for (int index = start; index < stop; ++index)
      slice[num++] = data[index];
    return slice;
  }

  internal static object[] GetSlice(object[] data, int start, int stop, int step)
  {
    if (step == 1)
      return ArrayOps.GetSlice(data, start, stop);
    int sliceSize = ArrayOps.GetSliceSize(start, stop, step);
    if (sliceSize <= 0)
      return ArrayUtils.EmptyObjects;
    object[] slice = new object[sliceSize];
    int index1 = 0;
    int index2 = start;
    while (index1 < slice.Length)
    {
      slice[index1] = data[index2];
      ++index1;
      index2 += step;
    }
    return slice;
  }

  internal static object[] GetSlice(object[] data, Slice slice)
  {
    int ostart;
    int ostop;
    int ostep;
    slice.indices(data.Length, out ostart, out ostop, out ostep);
    return ArrayOps.GetSlice(data, ostart, ostop, ostep);
  }

  internal static Array GetSlice(Array data, int size, Slice slice)
  {
    if (data.Rank != 1)
      throw PythonOps.NotImplementedError("slice on multi-dimensional array");
    int ostart;
    int ostop;
    int ostep;
    slice.indices(size, out ostart, out ostop, out ostep);
    if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
      return data.GetType().GetElementType() == typeof (object) ? (Array) ArrayUtils.EmptyObjects : Array.CreateInstance(data.GetType().GetElementType(), 0);
    if (ostep == 1)
    {
      int length = ostop - ostart;
      Array instance = Array.CreateInstance(data.GetType().GetElementType(), length);
      Array.Copy(data, ostart + data.GetLowerBound(0), instance, 0, length);
      return instance;
    }
    int sliceSize = ArrayOps.GetSliceSize(ostart, ostop, ostep);
    Array instance1 = Array.CreateInstance(data.GetType().GetElementType(), sliceSize);
    int num1 = 0;
    int num2 = 0;
    int num3 = ostart;
    while (num2 < sliceSize)
    {
      instance1.SetValue(data.GetValue(num3 + data.GetLowerBound(0)), num1++);
      ++num2;
      num3 += ostep;
    }
    return instance1;
  }

  private static int GetSliceSize(int start, int stop, int step)
  {
    return step <= 0 ? (stop - start + step + 1) / step : (stop - start + step - 1) / step;
  }

  internal static object[] CopyArray(object[] data, int newSize)
  {
    if (newSize == 0)
      return ArrayUtils.EmptyObjects;
    object[] destinationArray = new object[newSize];
    if (data.Length < 20)
    {
      for (int index = 0; index < data.Length && index < newSize; ++index)
        destinationArray[index] = data[index];
    }
    else
      Array.Copy((Array) data, (Array) destinationArray, Math.Min(newSize, data.Length));
    return destinationArray;
  }

  private static int[] TupleToIndices(Array a, IList<object> tuple)
  {
    int[] indices = new int[tuple.Count];
    for (int index = 0; index < indices.Length; ++index)
      indices[index] = PythonOps.FixIndex(Converter.ConvertToInt32(tuple[index]), a.GetUpperBound(index) + 1);
    return indices;
  }
}
