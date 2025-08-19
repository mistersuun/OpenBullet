// Decompiled with JetBrains decompiler
// Type: System.Text.SBCSCodePageEncoding
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace System.Text;

internal class SBCSCodePageEncoding(int codePage, int dataCodePage) : BaseCodePageEncoding(codePage, dataCodePage)
{
  private unsafe char* _mapBytesToUnicode = (char*) null;
  private unsafe byte* _mapUnicodeToBytes = (byte*) null;
  private const char UNKNOWN_CHAR = '�';
  private byte _byteUnknown;
  private char _charUnknown;
  private static object s_InternalSyncObject;

  public SBCSCodePageEncoding(int codePage)
    : this(codePage, codePage)
  {
  }

  protected override unsafe void LoadManagedCodePage()
  {
    fixed (byte* numPtr1 = &this.m_codePageHeader[0])
    {
      this._byteUnknown = ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->ByteCount == (short) 1 ? (byte) ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->ByteReplace : throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, (object) this.CodePage));
      this._charUnknown = ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->UnicodeReplace;
      int iSize = 66052 + this.iExtraBytes;
      byte* nativeMemory = this.GetNativeMemory(iSize);
      Unsafe.InitBlockUnaligned((void*) nativeMemory, (byte) 0, (uint) iSize);
      char* chPtr = (char*) nativeMemory;
      byte* numPtr2 = nativeMemory + 512 /*0x0200*/;
      byte[] buffer = new byte[512 /*0x0200*/];
      lock (BaseCodePageEncoding.s_streamLock)
      {
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek((long) this.m_firstDataWordOffset, SeekOrigin.Begin);
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, buffer.Length);
      }
      fixed (byte* numPtr3 = &buffer[0])
      {
        for (int index = 0; index < 256 /*0x0100*/; ++index)
        {
          if (((char*) numPtr3)[index] != char.MinValue || index == 0)
          {
            chPtr[index] = ((char*) numPtr3)[index];
            if (((char*) numPtr3)[index] != '�')
              numPtr2[(int) ((char*) numPtr3)[index]] = (byte) index;
          }
          else
            chPtr[index] = '�';
        }
      }
      this._mapBytesToUnicode = chPtr;
      this._mapUnicodeToBytes = numPtr2;
    }
  }

  private static object InternalSyncObject
  {
    get
    {
      if (SBCSCodePageEncoding.s_InternalSyncObject == null)
      {
        object obj = new object();
        Interlocked.CompareExchange<object>(ref SBCSCodePageEncoding.s_InternalSyncObject, obj, (object) null);
      }
      return SBCSCodePageEncoding.s_InternalSyncObject;
    }
  }

  protected override unsafe void ReadBestFitTable()
  {
    lock (SBCSCodePageEncoding.InternalSyncObject)
    {
      if (this.arrayUnicodeBestFit != null)
        return;
      byte[] buffer = new byte[this.m_dataSize - 512 /*0x0200*/];
      lock (BaseCodePageEncoding.s_streamLock)
      {
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek((long) (this.m_firstDataWordOffset + 512 /*0x0200*/), SeekOrigin.Begin);
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, buffer.Length);
      }
      fixed (byte* numPtr1 = buffer)
      {
        byte* numPtr2 = numPtr1;
        char[] chArray1 = new char[256 /*0x0100*/];
        for (int index = 0; index < 256 /*0x0100*/; ++index)
          chArray1[index] = this._mapBytesToUnicode[index];
        byte* numPtr3;
        ushort index1;
        for (; (index1 = *(ushort*) numPtr2) != (ushort) 0; numPtr2 = numPtr3 + 2)
        {
          numPtr3 = numPtr2 + 2;
          chArray1[(int) index1] = (char) *(ushort*) numPtr3;
        }
        this.arrayBytesBestFit = chArray1;
        byte* numPtr4 = numPtr2 + 2;
        byte* numPtr5 = numPtr4;
        int num1 = 0;
        int num2 = (int) *(ushort*) numPtr4;
        byte* numPtr6 = numPtr4 + 2;
        while (num2 < 65536 /*0x010000*/)
        {
          byte num3 = *numPtr6;
          ++numPtr6;
          if (num3 == (byte) 1)
          {
            num2 = (int) *(ushort*) numPtr6;
            numPtr6 += 2;
          }
          else if (num3 < (byte) 32 /*0x20*/ && num3 > (byte) 0 && num3 != (byte) 30)
          {
            num2 += (int) num3;
          }
          else
          {
            if (num3 > (byte) 0)
              ++num1;
            ++num2;
          }
        }
        char[] chArray2 = new char[num1 * 2];
        byte* numPtr7 = numPtr5;
        int num4 = (int) *(ushort*) numPtr7;
        byte* numPtr8 = numPtr7 + 2;
        int num5 = 0;
        while (num4 < 65536 /*0x010000*/)
        {
          byte index2 = *numPtr8;
          ++numPtr8;
          if (index2 == (byte) 1)
          {
            num4 = (int) *(ushort*) numPtr8;
            numPtr8 += 2;
          }
          else if (index2 < (byte) 32 /*0x20*/ && index2 > (byte) 0 && index2 != (byte) 30)
          {
            num4 += (int) index2;
          }
          else
          {
            if (index2 == (byte) 30)
            {
              index2 = *numPtr8;
              ++numPtr8;
            }
            if (index2 > (byte) 0)
            {
              char[] chArray3 = chArray2;
              int index3 = num5;
              int num6 = index3 + 1;
              int num7 = (int) (ushort) num4;
              chArray3[index3] = (char) num7;
              char[] chArray4 = chArray2;
              int index4 = num6;
              num5 = index4 + 1;
              int num8 = (int) this._mapBytesToUnicode[index2];
              chArray4[index4] = (char) num8;
            }
            ++num4;
          }
        }
        this.arrayUnicodeBestFit = chArray2;
      }
    }
  }

  public override unsafe int GetByteCount(char* chars, int count, EncoderNLS encoder)
  {
    this.CheckMemorySection();
    char ch1 = char.MinValue;
    EncoderReplacementFallback replacementFallback;
    if (encoder != null)
    {
      ch1 = encoder.charLeftOver;
      replacementFallback = encoder.Fallback as EncoderReplacementFallback;
    }
    else
      replacementFallback = this.EncoderFallback as EncoderReplacementFallback;
    if (replacementFallback != null && replacementFallback.MaxCharCount == 1)
    {
      if (ch1 > char.MinValue)
        ++count;
      return count;
    }
    EncoderFallbackBuffer fallbackBuffer = (EncoderFallbackBuffer) null;
    int byteCount = 0;
    char* _charEnd = chars + count;
    EncoderFallbackBufferHelper fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
    if (ch1 > char.MinValue)
    {
      fallbackBuffer = encoder.FallbackBuffer;
      fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
      fallbackBufferHelper.InternalInitialize(chars, _charEnd, encoder, false);
      fallbackBufferHelper.InternalFallback(ch1, ref chars);
    }
    char ch2;
    while ((ch2 = fallbackBuffer == null ? char.MinValue : fallbackBufferHelper.InternalGetNextChar()) != char.MinValue || chars < _charEnd)
    {
      if (ch2 == char.MinValue)
      {
        ch2 = *chars;
        ++chars;
      }
      if (this._mapUnicodeToBytes[(int) ch2] == (byte) 0 && ch2 != char.MinValue)
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = encoder != null ? encoder.FallbackBuffer : this.EncoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(_charEnd - count, _charEnd, encoder, false);
        }
        fallbackBufferHelper.InternalFallback(ch2, ref chars);
      }
      else
        ++byteCount;
    }
    return byteCount;
  }

  public override unsafe int GetBytes(
    char* chars,
    int charCount,
    byte* bytes,
    int byteCount,
    EncoderNLS encoder)
  {
    this.CheckMemorySection();
    char ch1 = char.MinValue;
    EncoderReplacementFallback replacementFallback;
    if (encoder != null)
    {
      ch1 = encoder.charLeftOver;
      replacementFallback = encoder.Fallback as EncoderReplacementFallback;
    }
    else
      replacementFallback = this.EncoderFallback as EncoderReplacementFallback;
    char* _charEnd = chars + charCount;
    byte* numPtr1 = bytes;
    char* chPtr = chars;
    if (replacementFallback != null && replacementFallback.MaxCharCount == 1)
    {
      byte num1 = this._mapUnicodeToBytes[(int) replacementFallback.DefaultString[0]];
      if (num1 != (byte) 0)
      {
        if (ch1 > char.MinValue)
        {
          if (byteCount == 0)
            this.ThrowBytesOverflow(encoder, true);
          *bytes++ = num1;
          --byteCount;
        }
        if (byteCount < charCount)
        {
          this.ThrowBytesOverflow(encoder, byteCount < 1);
          _charEnd = chars + byteCount;
        }
        while (chars < _charEnd)
        {
          char index = *chars;
          ++chars;
          byte num2 = this._mapUnicodeToBytes[(int) index];
          *bytes = num2 != (byte) 0 || index == char.MinValue ? num2 : num1;
          ++bytes;
        }
        if (encoder != null)
        {
          encoder.charLeftOver = char.MinValue;
          encoder.m_charsUsed = (int) (chars - chPtr);
        }
        return (int) (bytes - numPtr1);
      }
    }
    EncoderFallbackBuffer fallbackBuffer = (EncoderFallbackBuffer) null;
    byte* numPtr2 = bytes + byteCount;
    EncoderFallbackBufferHelper fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
    if (ch1 > char.MinValue)
    {
      fallbackBuffer = encoder.FallbackBuffer;
      fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
      fallbackBufferHelper.InternalInitialize(chars, _charEnd, encoder, true);
      fallbackBufferHelper.InternalFallback(ch1, ref chars);
      if ((long) fallbackBuffer.Remaining > numPtr2 - bytes)
        this.ThrowBytesOverflow(encoder, true);
    }
    char ch2;
    while ((ch2 = fallbackBuffer == null ? char.MinValue : fallbackBufferHelper.InternalGetNextChar()) != char.MinValue || chars < _charEnd)
    {
      if (ch2 == char.MinValue)
      {
        ch2 = *chars;
        ++chars;
      }
      byte num = this._mapUnicodeToBytes[(int) ch2];
      if (num == (byte) 0 && ch2 != char.MinValue)
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = encoder != null ? encoder.FallbackBuffer : this.EncoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(_charEnd - charCount, _charEnd, encoder, true);
        }
        fallbackBufferHelper.InternalFallback(ch2, ref chars);
        if ((long) fallbackBuffer.Remaining > numPtr2 - bytes)
        {
          --chars;
          fallbackBufferHelper.InternalReset();
          this.ThrowBytesOverflow(encoder, chars == chPtr);
          break;
        }
      }
      else
      {
        if (bytes >= numPtr2)
        {
          if (fallbackBuffer == null || !fallbackBufferHelper.bFallingBack)
            --chars;
          this.ThrowBytesOverflow(encoder, chars == chPtr);
          break;
        }
        *bytes = num;
        ++bytes;
      }
    }
    if (encoder != null)
    {
      if (fallbackBuffer != null && !fallbackBufferHelper.bUsedEncoder)
        encoder.charLeftOver = char.MinValue;
      encoder.m_charsUsed = (int) (chars - chPtr);
    }
    return (int) (bytes - numPtr1);
  }

  public override unsafe int GetCharCount(byte* bytes, int count, DecoderNLS decoder)
  {
    this.CheckMemorySection();
    DecoderReplacementFallback replacementFallback;
    bool flag;
    if (decoder == null)
    {
      replacementFallback = this.DecoderFallback as DecoderReplacementFallback;
      flag = this.DecoderFallback is InternalDecoderBestFitFallback;
    }
    else
    {
      replacementFallback = decoder.Fallback as DecoderReplacementFallback;
      flag = decoder.Fallback is InternalDecoderBestFitFallback;
    }
    if (flag || replacementFallback != null && replacementFallback.MaxCharCount == 1)
      return count;
    DecoderFallbackBuffer fallbackBuffer = (DecoderFallbackBuffer) null;
    DecoderFallbackBufferHelper fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
    int charCount = count;
    byte[] bytes1 = new byte[1];
    byte* numPtr = bytes + count;
    while (bytes < numPtr)
    {
      char ch = this._mapBytesToUnicode[*bytes];
      ++bytes;
      if (ch == '�')
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = decoder != null ? decoder.FallbackBuffer : this.DecoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(numPtr - count, (char*) null);
        }
        bytes1[0] = *(bytes - 1);
        charCount = charCount - 1 + fallbackBufferHelper.InternalFallback(bytes1, bytes);
      }
    }
    return charCount;
  }

  public override unsafe int GetChars(
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    DecoderNLS decoder)
  {
    this.CheckMemorySection();
    byte* numPtr1 = bytes + byteCount;
    byte* numPtr2 = bytes;
    char* chPtr = chars;
    DecoderReplacementFallback replacementFallback;
    bool flag;
    if (decoder == null)
    {
      replacementFallback = this.DecoderFallback as DecoderReplacementFallback;
      flag = this.DecoderFallback is InternalDecoderBestFitFallback;
    }
    else
    {
      replacementFallback = decoder.Fallback as DecoderReplacementFallback;
      flag = decoder.Fallback is InternalDecoderBestFitFallback;
    }
    if (flag || replacementFallback != null && replacementFallback.MaxCharCount == 1)
    {
      char ch1 = replacementFallback != null ? replacementFallback.DefaultString[0] : '?';
      if (charCount < byteCount)
      {
        this.ThrowCharsOverflow(decoder, charCount < 1);
        numPtr1 = bytes + charCount;
      }
      while (bytes < numPtr1)
      {
        char ch2;
        if (flag)
        {
          if (this.arrayBytesBestFit == null)
            this.ReadBestFitTable();
          ch2 = this.arrayBytesBestFit[(int) *bytes];
        }
        else
          ch2 = this._mapBytesToUnicode[*bytes];
        ++bytes;
        *chars = ch2 != '�' ? ch2 : ch1;
        ++chars;
      }
      if (decoder != null)
        decoder.m_bytesUsed = (int) (bytes - numPtr2);
      return (int) (chars - chPtr);
    }
    DecoderFallbackBuffer fallbackBuffer = (DecoderFallbackBuffer) null;
    byte[] bytes1 = new byte[1];
    char* _charEnd = chars + charCount;
    DecoderFallbackBufferHelper fallbackBufferHelper = new DecoderFallbackBufferHelper(decoder != null ? decoder.FallbackBuffer : this.DecoderFallback.CreateFallbackBuffer());
    while (bytes < numPtr1)
    {
      char ch = this._mapBytesToUnicode[*bytes];
      ++bytes;
      if (ch == '�')
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = decoder != null ? decoder.FallbackBuffer : this.DecoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(numPtr1 - byteCount, _charEnd);
        }
        bytes1[0] = *(bytes - 1);
        if (!fallbackBufferHelper.InternalFallback(bytes1, bytes, ref chars))
        {
          --bytes;
          fallbackBufferHelper.InternalReset();
          this.ThrowCharsOverflow(decoder, bytes == numPtr2);
          break;
        }
      }
      else
      {
        if (chars >= _charEnd)
        {
          --bytes;
          this.ThrowCharsOverflow(decoder, bytes == numPtr2);
          break;
        }
        *chars = ch;
        ++chars;
      }
    }
    if (decoder != null)
      decoder.m_bytesUsed = (int) (bytes - numPtr2);
    return (int) (chars - chPtr);
  }

  public override int GetMaxByteCount(int charCount)
  {
    if (charCount < 0)
      throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    long num = (long) charCount + 1L;
    if (this.EncoderFallback.MaxCharCount > 1)
      num *= (long) this.EncoderFallback.MaxCharCount;
    return num <= (long) int.MaxValue ? (int) num : throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_GetByteCountOverflow);
  }

  public override int GetMaxCharCount(int byteCount)
  {
    long num = byteCount >= 0 ? (long) byteCount : throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (this.DecoderFallback.MaxCharCount > 1)
      num *= (long) this.DecoderFallback.MaxCharCount;
    return num <= (long) int.MaxValue ? (int) num : throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_GetCharCountOverflow);
  }

  public override bool IsSingleByte => true;
}
