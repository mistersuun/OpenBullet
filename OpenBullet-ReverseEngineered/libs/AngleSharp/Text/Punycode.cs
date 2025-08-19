// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.Punycode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Text;

public static class Punycode
{
  private const int PunycodeBase = 36;
  private const int Tmin = 1;
  private const int Tmax = 26;
  private static readonly string acePrefix = "xn--";
  private static readonly char[] possibleDots = new char[4]
  {
    '.',
    '。',
    '．',
    '｡'
  };
  public static IDictionary<char, char> Symbols = (IDictionary<char, char>) new Dictionary<char, char>()
  {
    {
      '。',
      '.'
    },
    {
      '．',
      '.'
    },
    {
      'Ｇ',
      'g'
    },
    {
      'ｏ',
      'o'
    },
    {
      'ｃ',
      'c'
    },
    {
      'Ｘ',
      'x'
    },
    {
      '０',
      '0'
    },
    {
      '１',
      '1'
    },
    {
      '２',
      '2'
    },
    {
      '５',
      '5'
    },
    {
      '\u2070',
      '0'
    },
    {
      '\u00B9',
      '1'
    },
    {
      '\u00B2',
      '2'
    },
    {
      '\u00B3',
      '3'
    },
    {
      '\u2074',
      '4'
    },
    {
      '\u2075',
      '5'
    },
    {
      '\u2076',
      '6'
    },
    {
      '\u2077',
      '7'
    },
    {
      '\u2078',
      '8'
    },
    {
      '\u2079',
      '9'
    },
    {
      '\u2080',
      '0'
    },
    {
      '\u2081',
      '1'
    },
    {
      '\u2082',
      '2'
    },
    {
      '\u2083',
      '3'
    },
    {
      '\u2084',
      '4'
    },
    {
      '\u2085',
      '5'
    },
    {
      '\u2086',
      '6'
    },
    {
      '\u2087',
      '7'
    },
    {
      '\u2088',
      '8'
    },
    {
      '\u2089',
      '9'
    },
    {
      'ᵃ',
      'a'
    },
    {
      'ᵇ',
      'b'
    },
    {
      'ᶜ',
      'c'
    },
    {
      'ᵈ',
      'd'
    },
    {
      'ᵉ',
      'e'
    },
    {
      'ᶠ',
      'f'
    },
    {
      'ᵍ',
      'g'
    },
    {
      'ʰ',
      'h'
    },
    {
      'ⁱ',
      'i'
    },
    {
      'ʲ',
      'j'
    },
    {
      'ᵏ',
      'k'
    },
    {
      'ˡ',
      'l'
    },
    {
      'ᵐ',
      'm'
    },
    {
      'ⁿ',
      'n'
    },
    {
      'ᵒ',
      'o'
    },
    {
      'ᵖ',
      'p'
    },
    {
      'ʳ',
      'r'
    },
    {
      'ˢ',
      's'
    },
    {
      'ᵗ',
      't'
    },
    {
      'ᵘ',
      'u'
    },
    {
      'ᵛ',
      'v'
    },
    {
      'ʷ',
      'w'
    },
    {
      'ˣ',
      'x'
    },
    {
      'ʸ',
      'y'
    },
    {
      'ᶻ',
      'z'
    },
    {
      'ᴬ',
      'A'
    },
    {
      'ᴮ',
      'B'
    },
    {
      'ᴰ',
      'D'
    },
    {
      'ᴱ',
      'E'
    },
    {
      'ᴳ',
      'G'
    },
    {
      'ᴴ',
      'H'
    },
    {
      'ᴵ',
      'I'
    },
    {
      'ᴶ',
      'J'
    },
    {
      'ᴷ',
      'K'
    },
    {
      'ᴸ',
      'L'
    },
    {
      'ᴹ',
      'M'
    },
    {
      'ᴺ',
      'N'
    },
    {
      'ᴼ',
      'O'
    },
    {
      'ᴾ',
      'P'
    },
    {
      'ᴿ',
      'R'
    },
    {
      'ᵀ',
      'T'
    },
    {
      'ᵁ',
      'U'
    },
    {
      'ⱽ',
      'V'
    },
    {
      'ᵂ',
      'W'
    }
  };

  public static string Encode(string text)
  {
    if (text.Length == 0)
      return text;
    StringBuilder stringBuilder = new StringBuilder(text.Length);
    int num1 = 0;
    int startIndex1 = 0;
    int startIndex2 = 0;
    while (num1 < text.Length)
    {
      num1 = text.IndexOfAny(Punycode.possibleDots, startIndex1);
      if (num1 < 0)
        num1 = text.Length;
      if (num1 != startIndex1)
      {
        stringBuilder.Append(Punycode.acePrefix);
        int num2 = 0;
        for (int index = startIndex1; index < num1; ++index)
        {
          if (text[index] < '\u0080')
          {
            stringBuilder.Append(Punycode.EncodeBasic(text[index]));
            ++num2;
          }
          else if (char.IsSurrogatePair(text, index))
            ++index;
        }
        int num3 = num2;
        if (num3 == num1 - startIndex1)
          stringBuilder.Remove(startIndex2, Punycode.acePrefix.Length);
        else if (text.Length - startIndex1 < Punycode.acePrefix.Length || !text.Substring(startIndex1, Punycode.acePrefix.Length).Equals(Punycode.acePrefix, StringComparison.OrdinalIgnoreCase))
        {
          int num4 = 0;
          if (num3 > 0)
            stringBuilder.Append('-');
          int num5 = 128 /*0x80*/;
          int num6 = 0;
          int num7 = 72;
          while (num2 < num1 - startIndex1)
          {
            int test = 134217727 /*0x07FFFFFF*/;
            int utf32_1;
            for (int index = startIndex1; index < num1; index += Punycode.IsSupplementary(utf32_1) ? 2 : 1)
            {
              utf32_1 = char.ConvertToUtf32(text, index);
              if (utf32_1 >= num5 && utf32_1 < test)
                test = utf32_1;
            }
            int delta = num6 + (test - num5) * (num2 - num4 + 1);
            int num8 = test;
            int utf32_2;
            for (int index = startIndex1; index < num1; index += Punycode.IsSupplementary(utf32_2) ? 2 : 1)
            {
              utf32_2 = char.ConvertToUtf32(text, index);
              if (utf32_2 < num8)
                ++delta;
              else if (utf32_2 == num8)
              {
                int digit = delta;
                int num9 = 36;
                while (true)
                {
                  int num10 = num9 <= num7 ? 1 : (num9 >= num7 + 26 ? 26 : num9 - num7);
                  if (digit >= num10)
                  {
                    stringBuilder.Append(Punycode.EncodeDigit(num10 + (digit - num10) % (36 - num10)));
                    digit = (digit - num10) / (36 - num10);
                    num9 += 36;
                  }
                  else
                    break;
                }
                stringBuilder.Append(Punycode.EncodeDigit(digit));
                num7 = Punycode.AdaptChar(delta, num2 - num4 + 1, num2 == num3);
                delta = 0;
                ++num2;
                if (Punycode.IsSupplementary(test))
                {
                  ++num2;
                  ++num4;
                }
              }
            }
            num6 = delta + 1;
            num5 = num8 + 1;
          }
        }
        else
          break;
        if (stringBuilder.Length - startIndex2 > 63 /*0x3F*/)
          throw new ArgumentException();
        if (num1 != text.Length)
          stringBuilder.Append(Punycode.possibleDots[0]);
        startIndex1 = num1 + 1;
        startIndex2 = stringBuilder.Length;
      }
      else
        break;
    }
    int startIndex3 = (int) byte.MaxValue - (Punycode.IsDot(text[text.Length - 1]) ? 0 : 1);
    if (stringBuilder.Length > startIndex3)
      stringBuilder.Remove(startIndex3, stringBuilder.Length - startIndex3);
    return stringBuilder.ToString();
  }

  private static bool IsSupplementary(int test) => test >= 65536 /*0x010000*/;

  private static bool IsDot(char c)
  {
    for (int index = 0; index < Punycode.possibleDots.Length; ++index)
    {
      if ((int) Punycode.possibleDots[index] == (int) c)
        return true;
    }
    return false;
  }

  private static char EncodeDigit(int digit)
  {
    return digit > 25 ? (char) (digit + 22) : (char) (digit + 97);
  }

  private static char EncodeBasic(char character)
  {
    if (char.IsUpper(character))
      character += ' ';
    return character;
  }

  private static int AdaptChar(int delta, int numPoints, bool firstTime)
  {
    delta = firstTime ? delta / 700 : delta / 2;
    delta += delta / numPoints;
    uint num = 0;
    while (delta > 455)
    {
      delta /= 35;
      num += 36U;
    }
    return (int) ((long) num + (long) (36 * delta / (delta + 38)));
  }
}
