// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ListOfTOps`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ListOfTOps<T>
{
  public static string __repr__(CodeContext context, List<T> self)
  {
    List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) self);
    if (andCheckInfinite == null)
      return "[...]";
    int count = andCheckInfinite.Count;
    andCheckInfinite.Add((object) self);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("List[");
      stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(typeof (T)).Name);
      stringBuilder.Append("](");
      if (self.Count > 0)
      {
        stringBuilder.Append("[");
        string str = "";
        foreach (T o in self)
        {
          stringBuilder.Append(str);
          stringBuilder.Append(PythonOps.Repr(context, (object) o));
          str = ", ";
        }
        stringBuilder.Append("]");
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }
    finally
    {
      andCheckInfinite.RemoveAt(count);
    }
  }

  [SpecialName]
  public static void DeleteItem(List<T> l, int index)
  {
    l.RemoveAt(PythonOps.FixIndex(index, l.Count));
  }

  [SpecialName]
  public static void DeleteItem(List<T> l, object index)
  {
    ListOfTOps<T>.DeleteItem(l, Converter.ConvertToIndex(index));
  }

  [SpecialName]
  public static void DeleteItem(List<T> l, Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("List<T> indices must be slices or integers");
    int ostart;
    int ostop;
    int ostep;
    slice.indices(l.Count, out ostart, out ostop, out ostep);
    if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
      return;
    switch (ostep)
    {
      case -1:
        int index1 = ostop + 1;
        int index2 = ostart + 1;
        while (index2 < l.Count)
        {
          l[index1] = l[index2];
          ++index2;
          ++index1;
        }
        l.RemoveRange(index1, ostart - ostop);
        break;
      case 1:
        int index3 = ostart;
        int index4 = ostop;
        while (index4 < l.Count)
        {
          l[index3] = l[index4];
          ++index4;
          ++index3;
        }
        l.RemoveRange(index3, ostop - ostart);
        break;
      default:
        if (ostep < 0)
        {
          int num1 = ostart;
          while (num1 > ostop)
            num1 += ostep;
          int num2 = num1 - ostep;
          ostop = ostart + 1;
          ostart = num2;
          ostep = -ostep;
        }
        int num3;
        int index5 = num3 = ostart;
        int num4 = num3;
        int index6;
        for (index6 = num3; index6 < ostop && index5 < ostop; ++index5)
        {
          if (index5 != num4)
            l[index6++] = l[index5];
          else
            num4 += ostep;
        }
        while (ostop < l.Count)
          l[index6++] = l[ostop++];
        l.RemoveRange(index6, l.Count - index6);
        break;
    }
  }

  [SpecialName]
  public static List<T> GetItem(List<T> l, Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("List<T> indices must be slices or integers");
    int ostart;
    int ostop;
    int ostep;
    slice.indices(l.Count, out ostart, out ostop, out ostep);
    if (ostep == 1)
      return ostop <= ostart ? new List<T>() : l.Skip<T>(ostart).Take<T>(ostop - ostart).ToList<T>();
    int num = 0;
    List<T> objList;
    if (ostep > 0)
    {
      if (ostart > ostop)
        return new List<T>();
      objList = new List<T>((ostop - ostart + ostep - 1) / ostep);
      for (int index = ostart; index < ostop; index += ostep)
        objList[num++] = l[index];
    }
    else
    {
      if (ostart < ostop)
        return new List<T>();
      objList = new List<T>((ostop - ostart + ostep + 1) / ostep);
      for (int index = ostart; index > ostop; index += ostep)
        objList[num++] = l[index];
    }
    return objList;
  }

  public static List<T> __getslice__(List<T> self, int x, int y)
  {
    Slice.FixSliceArguments(self.Count, ref x, ref y);
    return x >= y ? new List<T>() : self.Skip<T>(x).Take<T>(y - x).ToList<T>();
  }
}
