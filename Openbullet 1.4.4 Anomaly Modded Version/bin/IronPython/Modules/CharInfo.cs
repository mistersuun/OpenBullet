// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.CharInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Modules;

internal class CharInfo
{
  internal readonly string Name;
  internal readonly string General_Category;
  internal readonly int Canonical_Combining_Class;
  internal readonly string Bidi_Class;
  internal readonly string Decomposition_Type;
  internal readonly int? Numeric_Value_Decimal;
  internal readonly int? Numeric_Value_Digit;
  internal readonly double? Numeric_Value_Numeric;
  internal readonly int Bidi_Mirrored;
  internal readonly string East_Asian_Width;

  internal CharInfo(string[] info)
  {
    this.Name = info[0].ToUpperInvariant();
    this.General_Category = info[1];
    this.Canonical_Combining_Class = int.Parse(info[2]);
    this.Bidi_Class = info[3];
    this.Decomposition_Type = info[4];
    string s1 = info[5];
    this.Numeric_Value_Decimal = s1 != "" ? new int?(int.Parse(s1)) : new int?();
    string s2 = info[6];
    this.Numeric_Value_Digit = s2 != "" ? new int?(int.Parse(s2)) : new int?();
    string str = info[7];
    if (str != "")
    {
      string[] strArray = str.Split('/');
      double num1 = double.Parse(strArray[0]);
      if (strArray.Length > 1)
      {
        double num2 = double.Parse(strArray[1]);
        num1 /= num2;
      }
      this.Numeric_Value_Numeric = new double?(num1);
    }
    else
      this.Numeric_Value_Numeric = new double?();
    this.Bidi_Mirrored = info[8] == "Y" ? 1 : 0;
    this.East_Asian_Width = info[9];
  }

  private static class PropertyIndex
  {
    internal const int Name = 0;
    internal const int General_Category = 1;
    internal const int Canonical_Combining_Class = 2;
    internal const int Bidi_Class = 3;
    internal const int Decomposition_Type = 4;
    internal const int Numeric_Value_Decimal = 5;
    internal const int Numeric_Value_Digit = 6;
    internal const int Numeric_Value_Numeric = 7;
    internal const int Bidi_Mirrored = 8;
    internal const int East_Asian_Width = 9;
  }
}
