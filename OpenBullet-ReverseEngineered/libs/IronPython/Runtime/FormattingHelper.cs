// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FormattingHelper
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Globalization;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

internal static class FormattingHelper
{
  private static NumberFormatInfo _invariantCommaSeperatorInfo;

  public static NumberFormatInfo InvariantCommaNumberInfo
  {
    get
    {
      if (FormattingHelper._invariantCommaSeperatorInfo == null)
        Interlocked.CompareExchange<NumberFormatInfo>(ref FormattingHelper._invariantCommaSeperatorInfo, new NumberFormatInfo()
        {
          NumberGroupSeparator = ",",
          NumberDecimalSeparator = ".",
          NumberGroupSizes = new int[1]{ 3 }
        }, (NumberFormatInfo) null);
      return FormattingHelper._invariantCommaSeperatorInfo;
    }
  }

  public static string ToCultureString<T>(T val, NumberFormatInfo nfi, StringFormatSpec spec)
  {
    string numberGroupSeparator = nfi.NumberGroupSeparator;
    int[] numberGroupSizes = nfi.NumberGroupSizes;
    string cultureString = val.ToString();
    int num1 = spec.Width ?? 0;
    int count = Math.Max(num1 - cultureString.Length, 0);
    bool flag = ((int) spec.Fill ?? 0) == 48 /*0x30*/ && num1 > cultureString.Length;
    int length = count;
    if (flag)
      cultureString = cultureString.Insert(0, new string('0', count));
    if (numberGroupSizes.Length != 0)
    {
      StringBuilder stringBuilder = new StringBuilder(cultureString);
      int index1 = 0;
      int num2 = cultureString.Length - 1;
      while (num2 > 0)
      {
        int num3 = numberGroupSizes[index1];
        if (num3 != 0)
        {
          num2 -= num3;
          if (num2 >= 0)
          {
            stringBuilder.Insert(num2 + 1, numberGroupSeparator);
            if (flag && num2 < count)
            {
              ++length;
              if (Math.Max(num1 - (stringBuilder.Length - length), 0) == 0)
                break;
            }
          }
          if (index1 + 1 < numberGroupSizes.Length)
          {
            if (numberGroupSizes[index1 + 1] != 0)
              ++index1;
            else
              break;
          }
        }
        else
          break;
      }
      if (flag && stringBuilder.Length > num1)
      {
        int num4 = Math.Max(num1 - (stringBuilder.Length - length), 0);
        if (num4 > 0)
        {
          int num5 = length - num4;
          if (numberGroupSeparator.IndexOf(stringBuilder[num5]) != -1)
          {
            for (int index2 = num5 - 1; index2 >= 0; --index2)
            {
              if (numberGroupSeparator.IndexOf(stringBuilder[index2]) == -1)
              {
                stringBuilder.Remove(0, index2);
                break;
              }
            }
          }
          else
            stringBuilder.Remove(0, num5);
        }
        else
          stringBuilder.Remove(0, length);
      }
      cultureString = stringBuilder.ToString();
    }
    return cultureString;
  }
}
