// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.Mro
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

internal static class Mro
{
  public static List<PythonType> Calculate(PythonType startingType, IList<PythonType> bases)
  {
    return Mro.Calculate(startingType, (IList<PythonType>) new List<PythonType>((IEnumerable<PythonType>) bases), false);
  }

  public static List<PythonType> Calculate(
    PythonType startingType,
    IList<PythonType> baseTypes,
    bool forceNewStyle)
  {
    List<PythonType> t = new List<PythonType>();
    foreach (PythonType baseType in (IEnumerable<PythonType>) baseTypes)
      t.Add(baseType);
    if (t.Contains(startingType))
      throw PythonOps.TypeError("a __bases__ item causes an inheritance cycle ({0})", (object) startingType.Name);
    List<PythonType> pythonTypeList = new List<PythonType>();
    pythonTypeList.Add(startingType);
    if (t.Count != 0)
    {
      List<IList<PythonType>> pythonTypeListList = new List<IList<PythonType>>();
      int num = 0;
      foreach (PythonType pythonType in t)
      {
        if (pythonType.IsOldClass)
          ++num;
      }
      foreach (PythonType oldStyleType in t)
      {
        if (!oldStyleType.IsOldClass)
          pythonTypeListList.Add(Mro.TupleToList(oldStyleType.ResolutionOrder));
        else if (num == 1 && !forceNewStyle)
          pythonTypeListList.Add(Mro.GetOldStyleMro(oldStyleType));
        else
          pythonTypeListList.Add(Mro.GetNewStyleMro(oldStyleType));
      }
      pythonTypeListList.Add(Mro.TupleToList((IList<PythonType>) t));
      bool flag1;
      PythonType pythonType1;
      do
      {
        flag1 = false;
        bool flag2 = false;
        pythonType1 = (PythonType) null;
        for (int index1 = 0; index1 < pythonTypeListList.Count; ++index1)
        {
          if (pythonTypeListList[index1].Count != 0)
          {
            flag2 = true;
            PythonType pythonType2 = pythonType1 = pythonTypeListList[index1][0];
            bool flag3 = false;
            for (int index2 = 0; index2 < pythonTypeListList.Count; ++index2)
            {
              if (pythonTypeListList[index2].Count != 0 && !pythonTypeListList[index2][0].Equals((object) pythonType2) && pythonTypeListList[index2].Contains(pythonType2))
              {
                flag3 = true;
                break;
              }
            }
            if (!flag3)
            {
              if (pythonTypeList.Contains(pythonType2))
                throw PythonOps.TypeError("a __bases__ item causes an inheritance cycle");
              pythonTypeList.Add(pythonType2);
              for (int index3 = 0; index3 < pythonTypeListList.Count; ++index3)
                pythonTypeListList[index3].Remove(pythonType2);
              flag1 = true;
              break;
            }
          }
        }
        if (!flag2)
          goto label_50;
      }
      while (flag1);
      string format = $"Cannot create a consistent method resolution\norder (MRO) for bases {pythonType1.Name}";
      for (int index = 0; index < pythonTypeListList.Count; ++index)
      {
        if (pythonTypeListList[index].Count != 0 && !pythonTypeListList[index][0].Equals((object) pythonType1))
        {
          PythonType pythonType3 = pythonTypeListList[index][0];
          format = format + ", " + pythonType3.Name;
        }
      }
      throw PythonOps.TypeError(format);
    }
label_50:
    return pythonTypeList;
  }

  private static IList<PythonType> TupleToList(IList<PythonType> t)
  {
    return (IList<PythonType>) new List<PythonType>((IEnumerable<PythonType>) t);
  }

  private static IList<PythonType> GetOldStyleMro(PythonType oldStyleType)
  {
    List<PythonType> res = new List<PythonType>();
    Mro.GetOldStyleMroWorker(oldStyleType, res);
    return (IList<PythonType>) res;
  }

  private static void GetOldStyleMroWorker(PythonType curType, List<PythonType> res)
  {
    PythonType pythonType = curType;
    if (res.Contains(curType))
      return;
    res.Add(curType);
    foreach (PythonType baseType in (IEnumerable<PythonType>) pythonType.BaseTypes)
      Mro.GetOldStyleMroWorker(baseType, res);
  }

  private static IList<PythonType> GetNewStyleMro(PythonType oldStyleType)
  {
    PythonType pythonType = oldStyleType;
    List<PythonType> newStyleMro = new List<PythonType>();
    newStyleMro.Add(oldStyleType);
    foreach (PythonType baseType in (IEnumerable<PythonType>) pythonType.BaseTypes)
      newStyleMro.AddRange((IEnumerable<PythonType>) Mro.TupleToList((IList<PythonType>) Mro.Calculate(baseType, baseType.BaseTypes, true)));
    return (IList<PythonType>) newStyleMro;
  }
}
