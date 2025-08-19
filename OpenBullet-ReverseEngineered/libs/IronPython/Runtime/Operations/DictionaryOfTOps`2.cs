// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.DictionaryOfTOps`2
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class DictionaryOfTOps<K, V>
{
  public static string __repr__(CodeContext context, Dictionary<K, V> self)
  {
    List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) self);
    if (andCheckInfinite == null)
      return "{...}";
    int count = andCheckInfinite.Count;
    andCheckInfinite.Add((object) self);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Dictionary[");
      stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(typeof (K)).Name);
      stringBuilder.Append(", ");
      stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(typeof (V)).Name);
      stringBuilder.Append("](");
      if (self.Count > 0)
      {
        stringBuilder.Append("{");
        string str = "";
        foreach (KeyValuePair<K, V> keyValuePair in self)
        {
          stringBuilder.Append(str);
          stringBuilder.Append(PythonOps.Repr(context, (object) keyValuePair.Key));
          stringBuilder.Append(" : ");
          stringBuilder.Append(PythonOps.Repr(context, (object) keyValuePair.Value));
          str = ", ";
        }
        stringBuilder.Append("}");
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }
    finally
    {
      andCheckInfinite.RemoveAt(count);
    }
  }
}
