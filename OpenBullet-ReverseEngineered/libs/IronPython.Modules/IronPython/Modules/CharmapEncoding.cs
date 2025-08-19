// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.CharmapEncoding
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Modules;

internal class CharmapEncoding : Encoding
{
  private readonly IDictionary<object, object> _map;
  private readonly string _errors;

  public CharmapEncoding(IDictionary<object, object> map, string errors)
  {
    this._map = map;
    this._errors = errors;
    this.FixupMap();
  }

  private void FixupMap()
  {
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) this._map)
    {
      if (keyValuePair.Key is string key && key.Length == 1)
        this._map[(object) (int) key[0]] = keyValuePair.Value;
    }
  }

  public override int GetByteCount(char[] chars, int index, int count)
  {
    int byteCount = 0;
    for (int index1 = index + count; index < index1; ++index)
    {
      char ch = chars[index];
      object obj;
      if (!this._map.TryGetValue((object) (int) ch, out obj) || obj == null && this._errors == "strict")
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(ch, index))
          byteCount += fallbackBuffer.Remaining;
      }
      else
      {
        switch (obj)
        {
          case null:
            throw PythonOps.UnicodeEncodeError("charmap", ch, index, "'charmap' codec can't encode character u'\\x{0:x}' in position {1}: character maps to <undefined>", (object) (int) ch, (object) index);
          case string _:
            byteCount += ((string) obj).Length;
            continue;
          case int _:
            ++byteCount;
            continue;
          default:
            throw PythonOps.TypeError("charmap must be an int, str, or None");
        }
      }
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
    int num1 = charIndex + charCount;
    int bytes1 = 0;
    for (; charIndex < num1; ++charIndex)
    {
      char ch = chars[charIndex];
      object obj;
      if (!this._map.TryGetValue((object) (int) ch, out obj) || obj == null && this._errors == "strict")
      {
        EncoderFallbackBuffer fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
        if (fallbackBuffer.Fallback(ch, charIndex))
        {
          while (fallbackBuffer.Remaining != 0)
          {
            object nextChar = (object) (int) fallbackBuffer.GetNextChar();
            bytes[byteIndex++] = (byte) (int) this._map[nextChar];
            ++bytes1;
          }
        }
      }
      else
      {
        switch (obj)
        {
          case null:
            throw PythonOps.UnicodeEncodeError("charmap", ch, charIndex, "'charmap' codec can't encode character u'\\x{0:x}' in position {1}: character maps to <undefined>", (object) (int) ch, (object) charIndex);
          case string _:
            foreach (byte num2 in obj as string)
            {
              bytes[byteIndex++] = num2;
              ++bytes1;
            }
            continue;
          case int num3:
            bytes[byteIndex++] = (byte) num3;
            ++bytes1;
            continue;
          default:
            throw PythonOps.TypeError("charmap must be an int, str, or None");
        }
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
      object obj;
      if (this._map.TryGetValue(ScriptingRuntimeHelpers.Int32ToObject((int) num2), out obj))
      {
        switch (obj)
        {
          case null:
            break;
          case string _:
            charCount += ((string) obj).Length;
            continue;
          case int _:
            ++charCount;
            continue;
          default:
            throw PythonOps.TypeError("charmap must be an int, str, or None");
        }
      }
      DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
      if (fallbackBuffer.Fallback(new byte[1]{ num2 }, 0))
        charCount += fallbackBuffer.Remaining;
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
      object obj;
      if (this._map.TryGetValue(ScriptingRuntimeHelpers.Int32ToObject((int) num2), out obj))
      {
        switch (obj)
        {
          case null:
            break;
          case string _:
            foreach (char ch in obj as string)
            {
              chars[charIndex++] = ch;
              ++chars1;
            }
            continue;
          case int num3:
            chars[charIndex++] = (char) num3;
            ++chars1;
            continue;
          default:
            throw PythonOps.TypeError("charmap must be an int, str, or None");
        }
      }
      DecoderFallbackBuffer fallbackBuffer = this.DecoderFallback.CreateFallbackBuffer();
      if (fallbackBuffer.Fallback(new byte[1]{ num2 }, 0))
      {
        while (fallbackBuffer.Remaining != 0)
        {
          chars[charIndex++] = fallbackBuffer.GetNextChar();
          ++chars1;
        }
      }
    }
    return chars1;
  }

  public override int GetMaxByteCount(int charCount) => charCount * 4;

  public override int GetMaxCharCount(int byteCount) => byteCount;
}
