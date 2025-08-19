// Decompiled with JetBrains decompiler
// Type: System.UriShim
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System;

internal class UriShim
{
  private const char c_DummyChar = '\uFFFF';
  private static readonly char[] s_hexUpperChars = new char[16 /*0x10*/]
  {
    '0',
    '1',
    '2',
    '3',
    '4',
    '5',
    '6',
    '7',
    '8',
    '9',
    'A',
    'B',
    'C',
    'D',
    'E',
    'F'
  };

  public static string HexEscape(char character)
  {
    if (character > 'ÿ')
      throw new ArgumentOutOfRangeException(nameof (character));
    char[] to = new char[3];
    int pos = 0;
    UriShim.EscapeAsciiChar(character, to, ref pos);
    return new string(to);
  }

  public static char HexUnescape(string pattern, ref int index)
  {
    if (index < 0 || index >= pattern.Length)
      throw new ArgumentOutOfRangeException(nameof (index));
    if (pattern[index] == '%' && pattern.Length - index >= 3)
    {
      char ch = UriShim.EscapedAscii(pattern[index + 1], pattern[index + 2]);
      if (ch != char.MaxValue)
      {
        index += 3;
        return ch;
      }
    }
    return pattern[index++];
  }

  public static bool IsHexEncoding(string pattern, int index)
  {
    return pattern.Length - index >= 3 && pattern[index] == '%' && UriShim.EscapedAscii(pattern[index + 1], pattern[index + 2]) != char.MaxValue;
  }

  internal static void EscapeAsciiChar(char ch, char[] to, ref int pos)
  {
    to[pos++] = '%';
    to[pos++] = UriShim.s_hexUpperChars[((int) ch & 240 /*0xF0*/) >> 4];
    to[pos++] = UriShim.s_hexUpperChars[(int) ch & 15];
  }

  private static char EscapedAscii(char digit, char next)
  {
    if ((digit < '0' || digit > '9') && (digit < 'A' || digit > 'F') && (digit < 'a' || digit > 'f'))
      return char.MaxValue;
    int num = digit <= '9' ? (int) digit - 48 /*0x30*/ : (digit <= 'F' ? (int) digit - 65 : (int) digit - 97) + 10;
    return (next < '0' || next > '9') && (next < 'A' || next > 'F') && (next < 'a' || next > 'f') ? char.MaxValue : (char) ((num << 4) + (next <= '9' ? (int) next - 48 /*0x30*/ : (next <= 'F' ? (int) next - 65 : (int) next - 97) + 10));
  }
}
