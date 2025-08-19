// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonBisectModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;

#nullable disable
namespace IronPython.Modules;

public class PythonBisectModule
{
  public const string __doc__ = "Bisection algorithms.\r\n\r\nThis module provides support for maintaining a list in sorted order without\r\nhaving to sort the list after each insertion. For long lists of items with\r\nexpensive comparison operations, this can be an improvement over the more\r\ncommon approach.\r\n";

  private static int InternalBisectLeft(
    CodeContext context,
    IronPython.Runtime.List list,
    object item,
    int lo,
    int hi)
  {
    if (lo < 0)
      throw PythonOps.ValueError("lo must be non-negative");
    if (hi == -1)
      hi = list.Count;
    IComparer comparer = context.LanguageContext.GetComparer((object) null, PythonBisectModule.GetComparisonType(list));
    while (lo < hi)
    {
      int index = (int) (((long) lo + (long) hi) / 2L);
      object x = list[index];
      if (comparer.Compare(x, item) < 0)
        lo = index + 1;
      else
        hi = index;
    }
    return lo;
  }

  private static int InternalBisectLeft(
    CodeContext context,
    object list,
    object item,
    int lo,
    int hi)
  {
    if (lo < 0)
      throw PythonOps.ValueError("lo must be non-negative");
    if (hi == -1)
      hi = PythonOps.Length(list);
    IComparer comparer = context.LanguageContext.GetComparer((object) null, PythonBisectModule.GetComparisonType(context, list));
    while (lo < hi)
    {
      int index1 = (int) (((long) lo + (long) hi) / 2L);
      object index2 = PythonOps.GetIndex(context, list, (object) index1);
      if (comparer.Compare(index2, item) < 0)
        lo = index1 + 1;
      else
        hi = index1;
    }
    return lo;
  }

  private static int InternalBisectRight(
    CodeContext context,
    IronPython.Runtime.List list,
    object item,
    int lo,
    int hi)
  {
    if (lo < 0)
      throw PythonOps.ValueError("lo must be non-negative");
    if (hi == -1)
      hi = list.Count;
    IComparer comparer = context.LanguageContext.GetComparer((object) null, PythonBisectModule.GetComparisonType(list));
    while (lo < hi)
    {
      int index = (int) (((long) lo + (long) hi) / 2L);
      object y = list[index];
      if (comparer.Compare(item, y) < 0)
        hi = index;
      else
        lo = index + 1;
    }
    return lo;
  }

  private static int InternalBisectRight(
    CodeContext context,
    object list,
    object item,
    int lo,
    int hi)
  {
    if (lo < 0)
      throw PythonOps.ValueError("lo must be non-negative");
    if (hi == -1)
      hi = PythonOps.Length(list);
    IComparer comparer = context.LanguageContext.GetComparer((object) null, PythonBisectModule.GetComparisonType(context, list));
    while (lo < hi)
    {
      int index1 = (int) (((long) lo + (long) hi) / 2L);
      object index2 = PythonOps.GetIndex(context, list, (object) index1);
      if (comparer.Compare(item, index2) < 0)
        hi = index1;
      else
        lo = index1 + 1;
    }
    return lo;
  }

  private static Type GetComparisonType(CodeContext context, object a)
  {
    return PythonOps.Length(a) > 0 ? CompilerHelpers.GetType(PythonOps.GetIndex(context, a, (object) 0)) : typeof (object);
  }

  private static Type GetComparisonType(IronPython.Runtime.List a)
  {
    return a.Count > 0 ? CompilerHelpers.GetType(a[0]) : typeof (object);
  }

  [Documentation("bisect_right(a, x[, lo[, hi]]) -> index\r\n\r\nReturn the index where to insert item x in list a, assuming a is sorted.\r\n\r\nThe return value i is such that all e in a[:i] have e <= x, and all e in\r\na[i:] have e > x.  So if x already appears in the list, i points just\r\nbeyond the rightmost x already there\r\n\r\nOptional args lo (default 0) and hi (default len(a)) bound the\r\nslice of a to be searched.\r\n")]
  public static object bisect_right(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    return a is IronPython.Runtime.List list && list.GetType() == typeof (IronPython.Runtime.List) ? (object) PythonBisectModule.InternalBisectRight(context, list, x, lo, hi) : (object) PythonBisectModule.InternalBisectRight(context, a, x, lo, hi);
  }

  [Documentation("insort_right(a, x[, lo[, hi]])\r\n\r\nInsert item x in list a, and keep it sorted assuming a is sorted.\r\n\r\nIf x is already in a, insert it to the right of the rightmost x.\r\n\r\nOptional args lo (default 0) and hi (default len(a)) bound the\r\nslice of a to be searched.\r\n")]
  public static void insort_right(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    if (a is IronPython.Runtime.List list && list.GetType() == typeof (IronPython.Runtime.List))
      list.Insert(PythonBisectModule.InternalBisectRight(context, list, x, lo, hi), x);
    else
      PythonOps.Invoke(context, a, "insert", (object) PythonBisectModule.InternalBisectRight(context, a, x, lo, hi), x);
  }

  [Documentation("bisect_left(a, x[, lo[, hi]]) -> index\r\n\r\nReturn the index where to insert item x in list a, assuming a is sorted.\r\n\r\nThe return value i is such that all e in a[:i] have e < x, and all e in\r\na[i:] have e >= x.  So if x already appears in the list, i points just\r\nbefore the leftmost x already there.\r\n\r\nOptional args lo (default 0) and hi (default len(a)) bound the\r\nslice of a to be searched.\r\n")]
  public static object bisect_left(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    return a is IronPython.Runtime.List list && list.GetType() == typeof (IronPython.Runtime.List) ? (object) PythonBisectModule.InternalBisectLeft(context, list, x, lo, hi) : (object) PythonBisectModule.InternalBisectLeft(context, a, x, lo, hi);
  }

  [Documentation("insort_left(a, x[, lo[, hi]])\r\n\r\nInsert item x in list a, and keep it sorted assuming a is sorted.\r\n\r\nIf x is already in a, insert it to the left of the leftmost x.\r\n\r\nOptional args lo (default 0) and hi (default len(a)) bound the\r\nslice of a to be searched.\r\n")]
  public static void insort_left(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    if (a is IronPython.Runtime.List list && list.GetType() == typeof (IronPython.Runtime.List))
      list.Insert(PythonBisectModule.InternalBisectLeft(context, list, x, lo, hi), x);
    else
      PythonOps.Invoke(context, a, "insert", (object) PythonBisectModule.InternalBisectLeft(context, a, x, lo, hi), x);
  }

  [Documentation("Alias for bisect_right().")]
  public static object bisect(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    return PythonBisectModule.bisect_right(context, a, x, lo, hi);
  }

  [Documentation("Alias for insort_right().")]
  public static void insort(CodeContext context, object a, object x, int lo = 0, int hi = -1)
  {
    PythonBisectModule.insort_right(context, a, x, lo, hi);
  }
}
