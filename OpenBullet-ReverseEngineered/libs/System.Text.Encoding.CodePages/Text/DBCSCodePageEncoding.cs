// Decompiled with JetBrains decompiler
// Type: System.Text.DBCSCodePageEncoding
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace System.Text;

internal class DBCSCodePageEncoding : BaseCodePageEncoding
{
  protected unsafe char* mapBytesToUnicode = (char*) null;
  protected unsafe ushort* mapUnicodeToBytes = (ushort*) null;
  protected const char UNKNOWN_CHAR_FLAG = '\0';
  protected const char UNICODE_REPLACEMENT_CHAR = '�';
  protected const char LEAD_BYTE_CHAR = '\uFFFE';
  private ushort _bytesUnknown;
  private int _byteCountUnknown;
  protected char charUnknown;
  private static object s_InternalSyncObject;

  public DBCSCodePageEncoding(int codePage)
    : this(codePage, codePage)
  {
  }

  internal DBCSCodePageEncoding(int codePage, int dataCodePage)
    : base(codePage, dataCodePage)
  {
  }

  internal DBCSCodePageEncoding(
    int codePage,
    int dataCodePage,
    EncoderFallback enc,
    DecoderFallback dec)
    : base(codePage, dataCodePage, enc, dec)
  {
  }

  protected override unsafe void LoadManagedCodePage()
  {
    fixed (byte* numPtr1 = &this.m_codePageHeader[0])
    {
      this._bytesUnknown = ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->ByteCount == (short) 2 ? ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->ByteReplace : throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, (object) this.CodePage));
      this.charUnknown = ((BaseCodePageEncoding.CodePageHeader*) numPtr1)->UnicodeReplace;
      if (this.DecoderFallback is InternalDecoderBestFitFallback)
        ((InternalDecoderBestFitFallback) this.DecoderFallback).cReplacement = this.charUnknown;
      this._byteCountUnknown = 1;
      if (this._bytesUnknown > (ushort) byte.MaxValue)
        ++this._byteCountUnknown;
      int iSize = 262148 /*0x040004*/ + this.iExtraBytes;
      byte* nativeMemory = this.GetNativeMemory(iSize);
      Unsafe.InitBlockUnaligned((void*) nativeMemory, (byte) 0, (uint) iSize);
      this.mapBytesToUnicode = (char*) nativeMemory;
      this.mapUnicodeToBytes = (ushort*) (nativeMemory + 131072 /*0x020000*/);
      byte[] buffer = new byte[this.m_dataSize];
      lock (BaseCodePageEncoding.s_streamLock)
      {
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek((long) this.m_firstDataWordOffset, SeekOrigin.Begin);
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, this.m_dataSize);
      }
      fixed (byte* numPtr2 = buffer)
      {
        char* chPtr = (char*) numPtr2;
        int num = 0;
        while (num < 65536 /*0x010000*/)
        {
          char index = *chPtr;
          ++chPtr;
          if (index == '\u0001')
          {
            num = (int) *chPtr;
            ++chPtr;
          }
          else if (index < ' ' && index > char.MinValue)
          {
            num += (int) index;
          }
          else
          {
            int bytes;
            switch (index)
            {
              case '�':
                ++num;
                continue;
              case '\uFFFE':
                bytes = num;
                break;
              case char.MaxValue:
                bytes = num;
                index = (char) num;
                break;
              default:
                bytes = num;
                break;
            }
            if (this.CleanUpBytes(ref bytes))
            {
              if (index != '\uFFFE')
                this.mapUnicodeToBytes[index] = (ushort) bytes;
              this.mapBytesToUnicode[bytes] = index;
            }
            ++num;
          }
        }
      }
      this.CleanUpEndBytes(this.mapBytesToUnicode);
    }
  }

  protected virtual bool CleanUpBytes(ref int bytes) => true;

  protected virtual unsafe void CleanUpEndBytes(char* chars)
  {
  }

  private static object InternalSyncObject
  {
    get
    {
      if (DBCSCodePageEncoding.s_InternalSyncObject == null)
      {
        object obj = new object();
        Interlocked.CompareExchange<object>(ref DBCSCodePageEncoding.s_InternalSyncObject, obj, (object) null);
      }
      return DBCSCodePageEncoding.s_InternalSyncObject;
    }
  }

  protected override unsafe void ReadBestFitTable()
  {
    lock (DBCSCodePageEncoding.InternalSyncObject)
    {
      if (this.arrayUnicodeBestFit != null)
        return;
      byte[] buffer = new byte[this.m_dataSize];
      lock (BaseCodePageEncoding.s_streamLock)
      {
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Seek((long) this.m_firstDataWordOffset, SeekOrigin.Begin);
        BaseCodePageEncoding.s_codePagesEncodingDataStream.Read(buffer, 0, this.m_dataSize);
      }
      fixed (byte* numPtr = buffer)
      {
        char* chPtr1 = (char*) numPtr;
        int num1 = 0;
        while (num1 < 65536 /*0x010000*/)
        {
          char ch = *chPtr1;
          ++chPtr1;
          if (ch == '\u0001')
          {
            num1 = (int) *chPtr1;
            ++chPtr1;
          }
          else if (ch < ' ' && ch > char.MinValue)
            num1 += (int) ch;
          else
            ++num1;
        }
        char* chPtr2 = chPtr1;
        int num2 = 0;
        int num3 = (int) *chPtr1;
        char* chPtr3 = chPtr1 + 1;
        while (num3 < 65536 /*0x010000*/)
        {
          char ch = *chPtr3;
          ++chPtr3;
          if (ch == '\u0001')
          {
            num3 = (int) *chPtr3;
            ++chPtr3;
          }
          else if (ch < ' ' && ch > char.MinValue)
          {
            num3 += (int) ch;
          }
          else
          {
            if (ch != '�')
            {
              int bytes = num3;
              if (this.CleanUpBytes(ref bytes) && (int) this.mapBytesToUnicode[bytes] != (int) ch)
                ++num2;
            }
            ++num3;
          }
        }
        char[] chArray1 = new char[num2 * 2];
        int num4 = 0;
        char* chPtr4 = chPtr2;
        int num5 = (int) *chPtr4;
        char* chPtr5 = chPtr4 + 1;
        bool flag = false;
        while (num5 < 65536 /*0x010000*/)
        {
          char ch = *chPtr5;
          ++chPtr5;
          if (ch == '\u0001')
          {
            num5 = (int) *chPtr5;
            ++chPtr5;
          }
          else if (ch < ' ' && ch > char.MinValue)
          {
            num5 += (int) ch;
          }
          else
          {
            if (ch != '�')
            {
              int bytes = num5;
              if (this.CleanUpBytes(ref bytes) && (int) this.mapBytesToUnicode[bytes] != (int) ch)
              {
                if (bytes != num5)
                  flag = true;
                char[] chArray2 = chArray1;
                int index1 = num4;
                int num6 = index1 + 1;
                int num7 = (int) (ushort) bytes;
                chArray2[index1] = (char) num7;
                char[] chArray3 = chArray1;
                int index2 = num6;
                num4 = index2 + 1;
                int num8 = (int) ch;
                chArray3[index2] = (char) num8;
              }
            }
            ++num5;
          }
        }
        if (flag)
        {
          for (int index3 = 0; index3 < chArray1.Length - 2; index3 += 2)
          {
            int index4 = index3;
            char ch1 = chArray1[index3];
            for (int index5 = index3 + 2; index5 < chArray1.Length; index5 += 2)
            {
              if ((int) ch1 > (int) chArray1[index5])
              {
                ch1 = chArray1[index5];
                index4 = index5;
              }
            }
            if (index4 != index3)
            {
              char ch2 = chArray1[index4];
              chArray1[index4] = chArray1[index3];
              chArray1[index3] = ch2;
              char ch3 = chArray1[index4 + 1];
              chArray1[index4 + 1] = chArray1[index3 + 1];
              chArray1[index3 + 1] = ch3;
            }
          }
        }
        this.arrayBytesBestFit = chArray1;
        char* chPtr6 = chPtr5;
        char* chPtr7 = chPtr5;
        char* chPtr8 = (char*) ((IntPtr) chPtr7 + 2);
        int num9 = (int) *chPtr7;
        int num10 = 0;
        while (num9 < 65536 /*0x010000*/)
        {
          char ch = *chPtr8;
          ++chPtr8;
          if (ch == '\u0001')
          {
            num9 = (int) *chPtr8;
            ++chPtr8;
          }
          else if (ch < ' ' && ch > char.MinValue)
          {
            num9 += (int) ch;
          }
          else
          {
            if (ch > char.MinValue)
              ++num10;
            ++num9;
          }
        }
        char[] chArray4 = new char[num10 * 2];
        char* chPtr9 = chPtr6;
        char* chPtr10 = (char*) ((IntPtr) chPtr9 + 2);
        int num11 = (int) *chPtr9;
        int num12 = 0;
        while (num11 < 65536 /*0x010000*/)
        {
          char ch = *chPtr10;
          ++chPtr10;
          if (ch == '\u0001')
          {
            num11 = (int) *chPtr10;
            ++chPtr10;
          }
          else if (ch < ' ' && ch > char.MinValue)
          {
            num11 += (int) ch;
          }
          else
          {
            if (ch > char.MinValue)
            {
              int bytes = (int) ch;
              if (this.CleanUpBytes(ref bytes))
              {
                char[] chArray5 = chArray4;
                int index6 = num12;
                int num13 = index6 + 1;
                int num14 = (int) (ushort) num11;
                chArray5[index6] = (char) num14;
                char[] chArray6 = chArray4;
                int index7 = num13;
                num12 = index7 + 1;
                int num15 = (int) this.mapBytesToUnicode[bytes];
                chArray6[index7] = (char) num15;
              }
            }
            ++num11;
          }
        }
        this.arrayUnicodeBestFit = chArray4;
      }
    }
  }

  public override unsafe int GetByteCount(char* chars, int count, EncoderNLS encoder)
  {
    this.CheckMemorySection();
    char ch1 = char.MinValue;
    if (encoder != null)
    {
      ch1 = encoder.charLeftOver;
      if (encoder.InternalHasFallbackBuffer && encoder.FallbackBuffer.Remaining > 0)
        throw new ArgumentException(SR.Format(SR.Argument_EncoderFallbackNotEmpty, (object) this.EncodingName, (object) encoder.Fallback.GetType()));
    }
    int byteCount = 0;
    char* _charEnd = chars + count;
    EncoderFallbackBuffer fallbackBuffer = (EncoderFallbackBuffer) null;
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
      ushort num = this.mapUnicodeToBytes[ch2];
      if (num == (ushort) 0 && ch2 != char.MinValue)
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
      {
        ++byteCount;
        if (num >= (ushort) 256 /*0x0100*/)
          ++byteCount;
      }
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
    EncoderFallbackBuffer fallbackBuffer = (EncoderFallbackBuffer) null;
    char* _charEnd = chars + charCount;
    char* chPtr = chars;
    byte* numPtr1 = bytes;
    byte* numPtr2 = bytes + byteCount;
    EncoderFallbackBufferHelper fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
    if (encoder != null)
    {
      char charLeftOver = encoder.charLeftOver;
      fallbackBuffer = encoder.FallbackBuffer;
      fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
      fallbackBufferHelper.InternalInitialize(chars, _charEnd, encoder, true);
      if (encoder.m_throwOnOverflow && fallbackBuffer.Remaining > 0)
        throw new ArgumentException(SR.Format(SR.Argument_EncoderFallbackNotEmpty, (object) this.EncodingName, (object) encoder.Fallback.GetType()));
      if (charLeftOver > char.MinValue)
        fallbackBufferHelper.InternalFallback(charLeftOver, ref chars);
    }
    char ch;
    while ((ch = fallbackBuffer == null ? char.MinValue : fallbackBufferHelper.InternalGetNextChar()) != char.MinValue || chars < _charEnd)
    {
      if (ch == char.MinValue)
      {
        ch = *chars;
        ++chars;
      }
      ushort num = this.mapUnicodeToBytes[ch];
      if (num == (ushort) 0 && ch != char.MinValue)
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = this.EncoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new EncoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(_charEnd - charCount, _charEnd, encoder, true);
        }
        fallbackBufferHelper.InternalFallback(ch, ref chars);
      }
      else
      {
        if (num >= (ushort) 256 /*0x0100*/)
        {
          if (bytes + 1 >= numPtr2)
          {
            if (fallbackBuffer == null || !fallbackBufferHelper.bFallingBack)
              --chars;
            else
              fallbackBuffer.MovePrevious();
            this.ThrowBytesOverflow(encoder, chars == chPtr);
            break;
          }
          *bytes = (byte) ((uint) num >> 8);
          ++bytes;
        }
        else if (bytes >= numPtr2)
        {
          if (fallbackBuffer == null || !fallbackBufferHelper.bFallingBack)
            --chars;
          else
            fallbackBuffer.MovePrevious();
          this.ThrowBytesOverflow(encoder, chars == chPtr);
          break;
        }
        *bytes = (byte) ((uint) num & (uint) byte.MaxValue);
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

  public override unsafe int GetCharCount(byte* bytes, int count, DecoderNLS baseDecoder)
  {
    this.CheckMemorySection();
    DBCSCodePageEncoding.DBCSDecoder dbcsDecoder = (DBCSCodePageEncoding.DBCSDecoder) baseDecoder;
    DecoderFallbackBuffer fallbackBuffer = (DecoderFallbackBuffer) null;
    byte* numPtr = bytes + count;
    int charCount = count;
    DecoderFallbackBufferHelper fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
    if (dbcsDecoder != null && dbcsDecoder.bLeftOver > (byte) 0)
    {
      if (count == 0)
      {
        if (!dbcsDecoder.MustFlush)
          return 0;
        fallbackBufferHelper = new DecoderFallbackBufferHelper(dbcsDecoder.FallbackBuffer);
        fallbackBufferHelper.InternalInitialize(bytes, (char*) null);
        byte[] bytes1 = new byte[1]{ dbcsDecoder.bLeftOver };
        return fallbackBufferHelper.InternalFallback(bytes1, bytes);
      }
      int index = (int) dbcsDecoder.bLeftOver << 8 | (int) *bytes;
      ++bytes;
      if (this.mapBytesToUnicode[index] == char.MinValue && index != 0)
      {
        int num = charCount - 1;
        fallbackBuffer = dbcsDecoder.FallbackBuffer;
        fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
        fallbackBufferHelper.InternalInitialize(numPtr - count, (char*) null);
        byte[] bytes2 = new byte[2]
        {
          (byte) (index >> 8),
          (byte) index
        };
        charCount = num + fallbackBufferHelper.InternalFallback(bytes2, bytes);
      }
    }
    while (bytes < numPtr)
    {
      int index = (int) *bytes;
      ++bytes;
      char ch = this.mapBytesToUnicode[index];
      if (ch == '\uFFFE')
      {
        --charCount;
        if (bytes < numPtr)
        {
          index = index << 8 | (int) *bytes;
          ++bytes;
          ch = this.mapBytesToUnicode[index];
        }
        else if (dbcsDecoder == null || dbcsDecoder.MustFlush)
        {
          ++charCount;
          ch = char.MinValue;
        }
        else
          break;
      }
      if (ch == char.MinValue && index != 0)
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = dbcsDecoder != null ? dbcsDecoder.FallbackBuffer : this.DecoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(numPtr - count, (char*) null);
        }
        int num = charCount - 1;
        byte[] bytes3;
        if (index < 256 /*0x0100*/)
          bytes3 = new byte[1]{ (byte) index };
        else
          bytes3 = new byte[2]
          {
            (byte) (index >> 8),
            (byte) index
          };
        charCount = num + fallbackBufferHelper.InternalFallback(bytes3, bytes);
      }
    }
    return charCount;
  }

  public override unsafe int GetChars(
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    DecoderNLS baseDecoder)
  {
    this.CheckMemorySection();
    DBCSCodePageEncoding.DBCSDecoder decoder = (DBCSCodePageEncoding.DBCSDecoder) baseDecoder;
    byte* numPtr1 = bytes;
    byte* numPtr2 = bytes + byteCount;
    char* chPtr = chars;
    char* _charEnd = chars + charCount;
    bool flag = false;
    DecoderFallbackBuffer fallbackBuffer = (DecoderFallbackBuffer) null;
    DecoderFallbackBufferHelper fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
    if (decoder != null && decoder.bLeftOver > (byte) 0)
    {
      if (byteCount == 0)
      {
        if (!decoder.MustFlush)
          return 0;
        fallbackBufferHelper = new DecoderFallbackBufferHelper(decoder.FallbackBuffer);
        fallbackBufferHelper.InternalInitialize(bytes, _charEnd);
        byte[] bytes1 = new byte[1]{ decoder.bLeftOver };
        if (!fallbackBufferHelper.InternalFallback(bytes1, bytes, ref chars))
          this.ThrowCharsOverflow((DecoderNLS) decoder, true);
        decoder.bLeftOver = (byte) 0;
        return (int) (chars - chPtr);
      }
      int index = (int) decoder.bLeftOver << 8 | (int) *bytes;
      ++bytes;
      char ch = this.mapBytesToUnicode[index];
      if (ch == char.MinValue && index != 0)
      {
        fallbackBuffer = decoder.FallbackBuffer;
        fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
        fallbackBufferHelper.InternalInitialize(numPtr2 - byteCount, _charEnd);
        byte[] bytes2 = new byte[2]
        {
          (byte) (index >> 8),
          (byte) index
        };
        if (!fallbackBufferHelper.InternalFallback(bytes2, bytes, ref chars))
          this.ThrowCharsOverflow((DecoderNLS) decoder, true);
      }
      else
      {
        if (chars >= _charEnd)
          this.ThrowCharsOverflow((DecoderNLS) decoder, true);
        *chars++ = ch;
      }
    }
    while (bytes < numPtr2)
    {
      int index = (int) *bytes;
      ++bytes;
      char ch = this.mapBytesToUnicode[index];
      if (ch == '\uFFFE')
      {
        if (bytes < numPtr2)
        {
          index = index << 8 | (int) *bytes;
          ++bytes;
          ch = this.mapBytesToUnicode[index];
        }
        else if (decoder == null || decoder.MustFlush)
        {
          ch = char.MinValue;
        }
        else
        {
          flag = true;
          decoder.bLeftOver = (byte) index;
          break;
        }
      }
      if (ch == char.MinValue && index != 0)
      {
        if (fallbackBuffer == null)
        {
          fallbackBuffer = decoder != null ? decoder.FallbackBuffer : this.DecoderFallback.CreateFallbackBuffer();
          fallbackBufferHelper = new DecoderFallbackBufferHelper(fallbackBuffer);
          fallbackBufferHelper.InternalInitialize(numPtr2 - byteCount, _charEnd);
        }
        byte[] bytes3;
        if (index < 256 /*0x0100*/)
          bytes3 = new byte[1]{ (byte) index };
        else
          bytes3 = new byte[2]
          {
            (byte) (index >> 8),
            (byte) index
          };
        if (!fallbackBufferHelper.InternalFallback(bytes3, bytes, ref chars))
        {
          bytes -= bytes3.Length;
          fallbackBufferHelper.InternalReset();
          this.ThrowCharsOverflow((DecoderNLS) decoder, bytes == numPtr1);
          break;
        }
      }
      else
      {
        if (chars >= _charEnd)
        {
          --bytes;
          if (index >= 256 /*0x0100*/)
            --bytes;
          this.ThrowCharsOverflow((DecoderNLS) decoder, bytes == numPtr1);
          break;
        }
        *chars++ = ch;
      }
    }
    if (decoder != null)
    {
      if (!flag)
        decoder.bLeftOver = (byte) 0;
      decoder.m_bytesUsed = (int) (bytes - numPtr1);
    }
    return (int) (chars - chPtr);
  }

  public override int GetMaxByteCount(int charCount)
  {
    if (charCount < 0)
      throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    long num1 = (long) charCount + 1L;
    if (this.EncoderFallback.MaxCharCount > 1)
      num1 *= (long) this.EncoderFallback.MaxCharCount;
    long num2 = num1 * 2L;
    return num2 <= (long) int.MaxValue ? (int) num2 : throw new ArgumentOutOfRangeException(nameof (charCount), SR.ArgumentOutOfRange_GetByteCountOverflow);
  }

  public override int GetMaxCharCount(int byteCount)
  {
    if (byteCount < 0)
      throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    long num = (long) byteCount + 1L;
    if (this.DecoderFallback.MaxCharCount > 1)
      num *= (long) this.DecoderFallback.MaxCharCount;
    return num <= (long) int.MaxValue ? (int) num : throw new ArgumentOutOfRangeException(nameof (byteCount), SR.ArgumentOutOfRange_GetCharCountOverflow);
  }

  public override Decoder GetDecoder() => (Decoder) new DBCSCodePageEncoding.DBCSDecoder(this);

  internal class DBCSDecoder(DBCSCodePageEncoding encoding) : DecoderNLS((EncodingNLS) encoding)
  {
    internal byte bLeftOver;

    public override void Reset()
    {
      this.bLeftOver = (byte) 0;
      if (this.m_fallbackBuffer == null)
        return;
      this.m_fallbackBuffer.Reset();
    }

    internal override bool HasState => this.bLeftOver > (byte) 0;
  }
}
