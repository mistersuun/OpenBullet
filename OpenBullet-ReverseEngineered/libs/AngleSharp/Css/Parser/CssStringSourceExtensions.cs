// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssStringSourceExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Css.Parser;

public static class CssStringSourceExtensions
{
  public static char SkipCssComment(this StringSource source)
  {
    char ch = source.Next();
    while (ch != char.MaxValue)
    {
      if (ch == '*')
      {
        ch = source.Next();
        if (ch == '/')
          return source.Next();
      }
      else
        ch = source.Next();
    }
    return ch;
  }

  public static string ConsumeEscape(this StringSource source)
  {
    char c = source.Next();
    if (c.IsHex())
    {
      bool flag = true;
      char[] chArray = new char[6];
      int num1;
      for (num1 = 0; flag && num1 < chArray.Length; flag = c.IsHex())
      {
        chArray[num1++] = c;
        c = source.Next();
      }
      if (!c.IsSpaceCharacter())
      {
        int num2 = (int) source.Back();
      }
      int num3 = 0;
      int num4 = 1;
      for (int index = num1 - 1; index >= 0; --index)
      {
        num3 += chArray[index].FromHex() * num4;
        num4 *= 16 /*0x10*/;
      }
      if (!num3.IsInvalid())
        return char.ConvertFromUtf32(num3);
      c = '�';
    }
    return c.ToString();
  }

  public static bool IsValidEscape(this StringSource source)
  {
    if (source.Current != '\\')
      return false;
    char c = source.Peek();
    return c != char.MaxValue && !c.IsLineBreak();
  }
}
