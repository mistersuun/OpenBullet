// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.CharExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace AngleSharp.Text;

public static class CharExtensions
{
  public static int FromHex(this char c)
  {
    return !c.IsDigit() ? (int) c - (c.IsLowercaseAscii() ? 87 : 55) : (int) c - 48 /*0x30*/;
  }

  public static string ToHex(this byte num)
  {
    char[] chArray = new char[2];
    int num1 = (int) num >> 4;
    chArray[0] = (char) (num1 + (num1 < 10 ? 48 /*0x30*/ : 55));
    int num2 = (int) num - 16 /*0x10*/ * num1;
    chArray[1] = (char) (num2 + (num2 < 10 ? 48 /*0x30*/ : 55));
    return new string(chArray);
  }

  public static string ToHex(this char character) => ((int) character).ToString("x");

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsInRange(this char c, int lower, int upper)
  {
    return (int) c >= lower && (int) c <= upper;
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsNormalQueryCharacter(this char c)
  {
    return c.IsInRange(33, 126) && c != '"' && c != '`' && c != '#' && c != '<' && c != '>';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsNormalPathCharacter(this char c)
  {
    return c.IsInRange(32 /*0x20*/, 126) && c != '"' && c != '`' && c != '#' && c != '<' && c != '>' && c != ' ' && c != '?';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsUppercaseAscii(this char c) => c >= 'A' && c <= 'Z';

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsLowercaseAscii(this char c) => c >= 'a' && c <= 'z';

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsAlphanumericAscii(this char c)
  {
    return c.IsDigit() || c.IsUppercaseAscii() || c.IsLowercaseAscii();
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsHex(this char c)
  {
    if (c.IsDigit() || c >= 'A' && c <= 'F')
      return true;
    return c >= 'a' && c <= 'f';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsNonAscii(this char c) => c != char.MaxValue && c >= '\u0080';

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsNonPrintable(this char c)
  {
    if (c >= char.MinValue && c <= '\b' || c >= '\u000E' && c <= '\u001F')
      return true;
    return c >= '\u007F' && c <= '\u009F';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsLetter(this char c) => c.IsUppercaseAscii() || c.IsLowercaseAscii();

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsName(this char c)
  {
    return c.IsNonAscii() || c.IsLetter() || c == '_' || c == '-' || c.IsDigit();
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsNameStart(this char c)
  {
    return c.IsNonAscii() || c.IsUppercaseAscii() || c.IsLowercaseAscii() || c == '_';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsLineBreak(this char c) => c == '\n' || c == '\r';

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsSpaceCharacter(this char c)
  {
    return c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsWhiteSpaceCharacter(this char c)
  {
    return c.IsInRange(9, 13) || c == ' ' || c == '\u0085' || c == ' ' || c == ' ' || c == '\u180E' || c.IsInRange(8192 /*0x2000*/, 8202) || c == '\u2028' || c == '\u2029' || c == ' ' || c == ' ' || c == '　';
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsDigit(this char c) => c >= '0' && c <= '9';

  public static bool IsUrlCodePoint(this char c)
  {
    return c.IsAlphanumericAscii() || c == '!' || c == '$' || c == '&' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '+' || c == '-' || c == ',' || c == '.' || c == '/' || c == ':' || c == ';' || c == '=' || c == '?' || c == '@' || c == '_' || c == '~' || c.IsInRange(160 /*0xA0*/, 55295) || c.IsInRange(57344 /*0xE000*/, 64975) || c.IsInRange(65008, 65533) || c.IsInRange(65536 /*0x010000*/, 131069) || c.IsInRange(131072 /*0x020000*/, 196605) || c.IsInRange(196608 /*0x030000*/, 262141) || c.IsInRange(262144 /*0x040000*/, 327677) || c.IsInRange(327680 /*0x050000*/, 393213) || c.IsInRange(393216 /*0x060000*/, 458749) || c.IsInRange(458752 /*0x070000*/, 524285) || c.IsInRange(524288 /*0x080000*/, 589821) || c.IsInRange(589824 /*0x090000*/, 655357) || c.IsInRange(655360 /*0x0A0000*/, 720893) || c.IsInRange(720896 /*0x0B0000*/, 786429) || c.IsInRange(786432 /*0x0C0000*/, 851965) || c.IsInRange(851968 /*0x0D0000*/, 917501) || c.IsInRange(917504 /*0x0E0000*/, 983037) || c.IsInRange(983040 /*0x0F0000*/, 1048573) || c.IsInRange(1048576 /*0x100000*/, 1114109);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsInvalid(this int c)
  {
    if (c == 0 || c > 1114111)
      return true;
    return c > 55296 && c < 57343 /*0xDFFF*/;
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(this char c, char a, char b)
  {
    return (int) a == (int) c || (int) b == (int) c;
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(this char c, char o1, char o2, char o3)
  {
    return (int) c == (int) o1 || (int) c == (int) o2 || (int) c == (int) o3;
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(this char c, char o1, char o2, char o3, char o4)
  {
    return (int) c == (int) o1 || (int) c == (int) o2 || (int) c == (int) o3 || (int) c == (int) o4;
  }
}
