// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonAsciiEncoding
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal sealed class PythonAsciiEncoding : Encoding
{
  internal static readonly Encoding Instance = PythonAsciiEncoding.MakeNonThrowing();
  internal static readonly Encoding SourceEncoding = PythonAsciiEncoding.MakeSourceEncoding();

  internal PythonAsciiEncoding()
  {
  }

  internal static Encoding MakeNonThrowing()
  {
    Encoding encoding = (Encoding) new PythonAsciiEncoding().Clone();
    encoding.DecoderFallback = (DecoderFallback) new NonStrictDecoderFallback();
    encoding.EncoderFallback = (EncoderFallback) new NonStrictEncoderFallback();
    return encoding;
  }

  private static Encoding MakeSourceEncoding()
  {
    Encoding encoding = (Encoding) new PythonAsciiEncoding().Clone();
    encoding.DecoderFallback = (DecoderFallback) new SourceNonStrictDecoderFallback();
    return encoding;
  }

  public override int GetByteCount(char[] chars, int index, int count)
  {
    int byteCount = 0;
    for (int index1 = index + count; index < index1; ++index)
    {
      char charUnknown = chars[index];
      if (charUnknown > '\u007F')
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(charUnknown, index))
          byteCount += fallbackBuffer.Remaining;
      }
      else
        ++byteCount;
    }
    return byteCount;
  }

  public override int GetBytes(
    char[] chars,
    int charIndex,
    int charCount,
    byte[] bytes,
    int byteIndex)
  {
    int num = charIndex + charCount;
    int bytes1 = 0;
    for (; charIndex < num; ++charIndex)
    {
      char charUnknown = chars[charIndex];
      if (charUnknown > '\u007F')
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(charUnknown, charIndex))
        {
          while (fallbackBuffer.Remaining != 0)
          {
            bytes[byteIndex++] = (byte) fallbackBuffer.GetNextChar();
            ++bytes1;
          }
        }
      }
      else
      {
        bytes[byteIndex++] = (byte) charUnknown;
        ++bytes1;
      }
    }
    return bytes1;
  }

  public override int GetCharCount(byte[] bytes, int index, int count)
  {
    int num1 = index + count;
    int charCount = 0;
    for (; index < num1; ++index)
    {
      byte num2 = bytes[index];
      if (num2 > (byte) 127 /*0x7F*/)
      {
        DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
        try
        {
          if (fallbackBuffer.Fallback(new byte[1]{ num2 }, 0))
            charCount += fallbackBuffer.Remaining;
        }
        catch (DecoderFallbackException ex)
        {
          DecoderFallbackException fallbackException = new DecoderFallbackException("ordinal out of range(128)", ex.BytesUnknown, ex.Index);
          fallbackException.Data.Add((object) "encoding", (object) this.EncodingName);
          throw fallbackException;
        }
      }
      else
        ++charCount;
    }
    return charCount;
  }

  public override int GetChars(
    byte[] bytes,
    int byteIndex,
    int byteCount,
    char[] chars,
    int charIndex)
  {
    int num1 = byteIndex + byteCount;
    int chars1 = 0;
    for (; byteIndex < num1; ++byteIndex)
    {
      byte num2 = bytes[byteIndex];
      if (num2 > (byte) 127 /*0x7F*/)
      {
        DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
        try
        {
          if (fallbackBuffer.Fallback(new byte[1]{ num2 }, 0))
          {
            while (fallbackBuffer.Remaining != 0)
            {
              chars[charIndex++] = fallbackBuffer.GetNextChar();
              ++chars1;
            }
          }
        }
        catch (DecoderFallbackException ex)
        {
          DecoderFallbackException fallbackException = new DecoderFallbackException("ordinal out of range(128)", ex.BytesUnknown, ex.Index);
          fallbackException.Data.Add((object) "encoding", (object) this.EncodingName);
          throw fallbackException;
        }
      }
      else
      {
        chars[charIndex++] = (char) num2;
        ++chars1;
      }
    }
    return chars1;
  }

  public override int GetMaxByteCount(int charCount) => charCount * 4;

  public override int GetMaxCharCount(int byteCount) => byteCount;

  public override string WebName => "ascii";

  public override string EncodingName => "ascii";
}
