// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Conversions.Conversion
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace RuriLib.Functions.Conversions;

public static class Conversion
{
  public static byte[] ConvertFrom(this string input, Encoding encoding)
  {
    switch (encoding)
    {
      case Encoding.HEX:
        input = new string(((IEnumerable<char>) input.ToCharArray()).Where<char>((Func<char, bool>) (c => !char.IsWhiteSpace(c))).ToArray<char>()).Replace("0x", "");
        return Enumerable.Range(0, input.Length).Where<int>((Func<int, bool>) (x => x % 2 == 0)).Select<int, byte>((Func<int, byte>) (x => Convert.ToByte(input.Substring(x, 2), 16 /*0x10*/))).ToArray<byte>();
      case Encoding.BIN:
        int length = input.Length / 8;
        byte[] numArray = new byte[length];
        for (int index = 0; index < length; ++index)
          numArray[index] = Convert.ToByte(input.Substring(8 * index, 8), 2);
        return numArray;
      case Encoding.BASE64:
        return Convert.FromBase64String(input);
      case Encoding.ASCII:
        return System.Text.Encoding.ASCII.GetBytes(input);
      case Encoding.UTF8:
        return System.Text.Encoding.UTF8.GetBytes(input);
      case Encoding.UNICODE:
        return System.Text.Encoding.Unicode.GetBytes(input);
      default:
        return new byte[0];
    }
  }

  public static string ConvertTo(this byte[] input, Encoding encoding)
  {
    StringBuilder stringBuilder = new StringBuilder();
    switch (encoding)
    {
      case Encoding.HEX:
        foreach (byte num in input)
          stringBuilder.AppendFormat("{0:x2}", (object) num);
        return stringBuilder.ToString().ToUpper();
      case Encoding.BIN:
        return string.Concat(((IEnumerable<byte>) input).Select<byte, string>((Func<byte, string>) (b => Convert.ToString(b, 2).PadLeft(8, '0'))));
      case Encoding.BASE64:
        return Convert.ToBase64String(input);
      case Encoding.ASCII:
        return System.Text.Encoding.ASCII.GetString(input);
      case Encoding.UTF8:
        return System.Text.Encoding.UTF8.GetString(input);
      case Encoding.UNICODE:
        return System.Text.Encoding.Unicode.GetString(input);
      default:
        return "";
    }
  }
}
