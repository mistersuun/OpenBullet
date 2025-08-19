// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonHeapq
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;

#nullable disable
namespace IronPython.Modules;

public static class PythonHeapq
{
  public const string __doc__ = "implements a heapq or priority queue.";
  public const string __about__ = "Heaps are arrays for which a[k] <= a[2*k+1] and a[k] <= a[2*k+2] for all k.";

  [Documentation("Transform list into a heap, in-place, in O(len(heap)) time.")]
  public static void heapify(CodeContext context, IronPython.Runtime.List list)
  {
    lock (list)
      PythonHeapq.DoHeapify(context, list);
  }

  [Documentation("Pop the smallest item off the heap, maintaining the heap invariant.")]
  public static object heappop(CodeContext context, IronPython.Runtime.List list)
  {
    lock (list)
    {
      int j = list._size - 1;
      if (j < 0)
        throw PythonOps.IndexError("index out of range");
      list.FastSwap(0, j);
      --list._size;
      PythonHeapq.SiftDown(context, list, 0, j - 1);
      return list._data[list._size];
    }
  }

  [Documentation("Push item onto heap, maintaining the heap invariant.")]
  public static void heappush(CodeContext context, IronPython.Runtime.List list, object item)
  {
    lock (list)
    {
      list.AddNoLock(item);
      PythonHeapq.SiftUp(context, list, list._size - 1);
    }
  }

  [Documentation("Push item on the heap, then pop and return the smallest item\nfrom the heap. The combined action runs more efficiently than\nheappush() followed by a separate call to heappop().")]
  public static object heappushpop(CodeContext context, IronPython.Runtime.List list, object item)
  {
    lock (list)
      return PythonHeapq.DoPushPop(context, list, item);
  }

  [Documentation("Pop and return the current smallest value, and add the new item.\n\nThis is more efficient than heappop() followed by heappush(), and can be\nmore appropriate when using a fixed-size heap. Note that the value\nreturned may be larger than item!  That constrains reasonable uses of\nthis routine unless written as part of a conditional replacement:\n\n        if item > heap[0]:\n            item = heapreplace(heap, item)\n")]
  public static object heapreplace(CodeContext context, IronPython.Runtime.List list, object item)
  {
    lock (list)
    {
      object obj = list._data[0];
      list._data[0] = item;
      PythonHeapq.SiftDown(context, list, 0, list._size - 1);
      return obj;
    }
  }

  [Documentation("Find the n largest elements in a dataset.\n\nEquivalent to:  sorted(iterable, reverse=True)[:n]\n")]
  public static IronPython.Runtime.List nlargest(CodeContext context, int n, object iterable)
  {
    if (n <= 0)
      return new IronPython.Runtime.List();
    IronPython.Runtime.List list = new IronPython.Runtime.List(Math.Min(n, 4000));
    IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
    for (int index = 0; index < n; ++index)
    {
      if (!enumerator.MoveNext())
      {
        PythonHeapq.HeapSort(context, list, true);
        return list;
      }
      list.append(enumerator.Current);
    }
    PythonHeapq.DoHeapify(context, list);
    while (enumerator.MoveNext())
      PythonHeapq.DoPushPop(context, list, enumerator.Current);
    PythonHeapq.HeapSort(context, list, true);
    return list;
  }

  [Documentation("Find the n smallest elements in a dataset.\n\nEquivalent to:  sorted(iterable)[:n]\n")]
  public static IronPython.Runtime.List nsmallest(CodeContext context, int n, object iterable)
  {
    if (n <= 0)
      return new IronPython.Runtime.List();
    IronPython.Runtime.List list = new IronPython.Runtime.List(Math.Min(n, 4000));
    IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
    for (int index = 0; index < n; ++index)
    {
      if (!enumerator.MoveNext())
      {
        PythonHeapq.HeapSort(context, list);
        return list;
      }
      list.append(enumerator.Current);
    }
    PythonHeapq.DoHeapifyMax(context, list);
    while (enumerator.MoveNext())
      PythonHeapq.DoPushPopMax(context, list, enumerator.Current);
    PythonHeapq.HeapSort(context, list);
    return list;
  }

  private static bool IsLessThan(CodeContext context, object x, object y)
  {
    object obj;
    if (PythonTypeOps.TryInvokeBinaryOperator(context, x, y, "__lt__", out obj) && obj != NotImplementedType.Value)
      return Converter.ConvertToBoolean(obj);
    return PythonTypeOps.TryInvokeBinaryOperator(context, y, x, "__le__", out obj) && obj != NotImplementedType.Value ? !Converter.ConvertToBoolean(obj) : context.LanguageContext.LessThan(x, y);
  }

  private static void HeapSort(CodeContext context, IronPython.Runtime.List list)
  {
    PythonHeapq.HeapSort(context, list, false);
  }

  private static void HeapSort(CodeContext context, IronPython.Runtime.List list, bool reverse)
  {
    if (reverse)
      PythonHeapq.DoHeapify(context, list);
    else
      PythonHeapq.DoHeapifyMax(context, list);
    int num = list._size - 1;
    while (num > 0)
    {
      list.FastSwap(0, num);
      --num;
      if (reverse)
        PythonHeapq.SiftDown(context, list, 0, num);
      else
        PythonHeapq.SiftDownMax(context, list, 0, num);
    }
  }

  private static void DoHeapify(CodeContext context, IronPython.Runtime.List list)
  {
    int stop = list._size - 1;
    for (int start = (stop - 1) / 2; start >= 0; --start)
      PythonHeapq.SiftDown(context, list, start, stop);
  }

  private static void DoHeapifyMax(CodeContext context, IronPython.Runtime.List list)
  {
    int stop = list._size - 1;
    for (int start = (stop - 1) / 2; start >= 0; --start)
      PythonHeapq.SiftDownMax(context, list, start, stop);
  }

  private static object DoPushPop(CodeContext context, IronPython.Runtime.List heap, object item)
  {
    object obj;
    if (heap._size == 0 || !PythonHeapq.IsLessThan(context, obj = heap._data[0], item))
      return item;
    heap._data[0] = item;
    PythonHeapq.SiftDown(context, heap, 0, heap._size - 1);
    return obj;
  }

  private static object DoPushPopMax(CodeContext context, IronPython.Runtime.List heap, object item)
  {
    object obj;
    if (heap._size == 0 || !PythonHeapq.IsLessThan(context, item, obj = heap._data[0]))
      return item;
    heap._data[0] = item;
    PythonHeapq.SiftDownMax(context, heap, 0, heap._size - 1);
    return obj;
  }

  private static void SiftDown(CodeContext context, IronPython.Runtime.List heap, int start, int stop)
  {
    int j;
    for (int i = start; (j = i * 2 + 1) <= stop; i = j)
    {
      if (j + 1 <= stop && PythonHeapq.IsLessThan(context, heap._data[j + 1], heap._data[j]))
        ++j;
      if (!PythonHeapq.IsLessThan(context, heap._data[j], heap._data[i]))
        break;
      heap.FastSwap(i, j);
    }
  }

  private static void SiftDownMax(CodeContext context, IronPython.Runtime.List heap, int start, int stop)
  {
    int j;
    for (int i = start; (j = i * 2 + 1) <= stop; i = j)
    {
      if (j + 1 <= stop && PythonHeapq.IsLessThan(context, heap._data[j], heap._data[j + 1]))
        ++j;
      if (!PythonHeapq.IsLessThan(context, heap._data[i], heap._data[j]))
        break;
      heap.FastSwap(i, j);
    }
  }

  private static void SiftUp(CodeContext context, IronPython.Runtime.List heap, int index)
  {
    int i;
    for (; index > 0; index = i)
    {
      i = (index - 1) / 2;
      if (!PythonHeapq.IsLessThan(context, heap._data[index], heap._data[i]))
        break;
      heap.FastSwap(i, index);
    }
  }

  private static void SiftUpMax(CodeContext context, IronPython.Runtime.List heap, int index)
  {
    int i;
    for (; index > 0; index = i)
    {
      i = (index - 1) / 2;
      if (!PythonHeapq.IsLessThan(context, heap._data[i], heap._data[index]))
        break;
      heap.FastSwap(i, index);
    }
  }
}
