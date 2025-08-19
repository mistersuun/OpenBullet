// Decompiled with JetBrains decompiler
// Type: System.Text.DecoderFallbackBufferHelper
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Globalization;

#nullable disable
namespace System.Text;

internal struct DecoderFallbackBufferHelper(DecoderFallbackBuffer fallbackBuffer)
{
  internal unsafe byte* byteStart = (byte*) null;
  internal unsafe char* charEnd = (char*) null;
  private DecoderFallbackBuffer _fallbackBuffer = fallbackBuffer;

  internal unsafe void InternalReset()
  {
    this.byteStart = (byte*) null;
    this._fallbackBuffer.Reset();
  }

  internal unsafe void InternalInitialize(byte* _byteStart, char* _charEnd)
  {
    this.byteStart = _byteStart;
    this.charEnd = _charEnd;
  }

  internal unsafe bool InternalFallback(byte[] bytes, byte* pBytes, ref char* chars)
  {
    if (this._fallbackBuffer.Fallback(bytes, (int) (pBytes - this.byteStart - (long) bytes.Length)))
    {
      char* chPtr = chars;
      bool flag = false;
      char nextChar;
      while ((nextChar = this._fallbackBuffer.GetNextChar()) != char.MinValue)
      {
        if (char.IsSurrogate(nextChar))
        {
          if (char.IsHighSurrogate(nextChar))
            flag = !flag ? true : throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
          else
            flag = flag ? false : throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
        }
        if (chPtr >= this.charEnd)
          return false;
        *chPtr++ = nextChar;
      }
      if (flag)
        throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
      chars = chPtr;
    }
    return true;
  }

  internal unsafe int InternalFallback(byte[] bytes, byte* pBytes)
  {
    if (!this._fallbackBuffer.Fallback(bytes, (int) (pBytes - this.byteStart - (long) bytes.Length)))
      return 0;
    int num = 0;
    bool flag = false;
    char nextChar;
    while ((nextChar = this._fallbackBuffer.GetNextChar()) != char.MinValue)
    {
      if (char.IsSurrogate(nextChar))
      {
        if (char.IsHighSurrogate(nextChar))
          flag = !flag ? true : throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
        else
          flag = flag ? false : throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
      }
      ++num;
    }
    if (flag)
      throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
    return num;
  }

  internal void ThrowLastBytesRecursive(byte[] bytesUnknown)
  {
    StringBuilder stringBuilder = new StringBuilder(bytesUnknown.Length * 3);
    int index;
    for (index = 0; index < bytesUnknown.Length && index < 20; ++index)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.Append(" ");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\\x{0:X2}", (object) bytesUnknown[index]);
    }
    if (index == 20)
      stringBuilder.Append(" ...");
    throw new ArgumentException(SR.Format(SR.Argument_RecursiveFallbackBytes, (object) stringBuilder.ToString()), nameof (bytesUnknown));
  }
}
