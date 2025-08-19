// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.EncodingMapEncoding
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System.Text;

#nullable disable
namespace IronPython.Modules;

internal class EncodingMapEncoding : Encoding
{
  private readonly EncodingMap _map;
  private readonly string _errors;

  public EncodingMapEncoding(EncodingMap map, string errors)
  {
    this._map = map;
    this._errors = errors;
  }

  public override int GetByteCount(char[] chars, int index, int count)
  {
    int byteCount = 0;
    for (int index1 = index + count; index < index1; ++index)
    {
      char ch = chars[index];
      if (!this._map.Mapping.TryGetValue((int) ch, out char _))
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(ch, index))
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
      char ch1 = chars[charIndex];
      char ch2;
      if (!this._map.Mapping.TryGetValue((int) ch1, out ch2))
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(ch1, charIndex))
        {
          while (fallbackBuffer.Remaining != 0)
          {
            bytes[byteIndex++] = (byte) this._map.Mapping[(int) fallbackBuffer.GetNextChar()];
            ++bytes1;
          }
        }
      }
      else
      {
        bytes[byteIndex++] = (byte) ch2;
        ++bytes1;
      }
    }
    return bytes1;
  }

  public override int GetCharCount(byte[] bytes, int index, int count)
  {
    int num = index + count;
    int charCount = 0;
    for (; index < num; ++index)
    {
      byte key = bytes[index];
      if (!this._map.Mapping.TryGetValue((int) key, out char _))
      {
        DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(new byte[1]{ key }, 0))
          charCount += fallbackBuffer.Remaining;
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
    int num = byteIndex + byteCount;
    int chars1 = 0;
    for (; byteIndex < num; ++byteIndex)
    {
      byte key = bytes[byteIndex];
      char ch;
      if (!this._map.Mapping.TryGetValue((int) key, out ch))
      {
        DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(new byte[1]{ key }, 0))
        {
          while (fallbackBuffer.Remaining != 0)
          {
            chars[charIndex++] = this._map.Mapping[(int) fallbackBuffer.GetNextChar()];
            ++chars1;
          }
        }
      }
      else
      {
        chars[charIndex++] = ch;
        ++chars1;
      }
    }
    return chars1;
  }

  public override int GetMaxByteCount(int charCount) => charCount * 4;

  public override int GetMaxCharCount(int byteCount) => byteCount;
}
