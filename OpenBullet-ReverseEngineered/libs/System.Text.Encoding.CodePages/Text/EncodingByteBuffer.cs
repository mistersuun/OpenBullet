// Decompiled with JetBrains decompiler
// Type: System.Text.EncodingByteBuffer
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal class EncodingByteBuffer
{
  private unsafe byte* _bytes;
  private unsafe byte* _byteStart;
  private unsafe byte* _byteEnd;
  private unsafe char* _chars;
  private unsafe char* _charStart;
  private unsafe char* _charEnd;
  private int _byteCountResult;
  private EncodingNLS _enc;
  private EncoderNLS _encoder;
  internal EncoderFallbackBuffer fallbackBuffer;
  internal EncoderFallbackBufferHelper fallbackBufferHelper;

  internal unsafe EncodingByteBuffer(
    EncodingNLS inEncoding,
    EncoderNLS inEncoder,
    byte* inByteStart,
    int inByteCount,
    char* inCharStart,
    int inCharCount)
  {
    this._enc = inEncoding;
    this._encoder = inEncoder;
    this._charStart = inCharStart;
    this._chars = inCharStart;
    this._charEnd = inCharStart + inCharCount;
    this._bytes = inByteStart;
    this._byteStart = inByteStart;
    this._byteEnd = inByteStart + inByteCount;
    if (this._encoder == null)
    {
      this.fallbackBuffer = this._enc.EncoderFallback.CreateFallbackBuffer();
    }
    else
    {
      this.fallbackBuffer = this._encoder.FallbackBuffer;
      if (this._encoder.m_throwOnOverflow && this._encoder.InternalHasFallbackBuffer && this.fallbackBuffer.Remaining > 0)
        throw new ArgumentException(SR.Format(SR.Argument_EncoderFallbackNotEmpty, (object) this._encoder.Encoding.EncodingName, (object) this._encoder.Fallback.GetType()));
    }
    this.fallbackBufferHelper = new EncoderFallbackBufferHelper(this.fallbackBuffer);
    this.fallbackBufferHelper.InternalInitialize(this._chars, this._charEnd, this._encoder, (IntPtr) this._bytes != IntPtr.Zero);
  }

  internal unsafe bool AddByte(byte b, int moreBytesExpected)
  {
    if ((IntPtr) this._bytes != IntPtr.Zero)
    {
      if (this._bytes >= this._byteEnd - moreBytesExpected)
      {
        this.MovePrevious(true);
        return false;
      }
      *this._bytes++ = b;
    }
    ++this._byteCountResult;
    return true;
  }

  internal bool AddByte(byte b1) => this.AddByte(b1, 0);

  internal bool AddByte(byte b1, byte b2) => this.AddByte(b1, b2, 0);

  internal bool AddByte(byte b1, byte b2, int moreBytesExpected)
  {
    return this.AddByte(b1, 1 + moreBytesExpected) && this.AddByte(b2, moreBytesExpected);
  }

  internal bool AddByte(byte b1, byte b2, byte b3) => this.AddByte(b1, b2, b3, 0);

  internal bool AddByte(byte b1, byte b2, byte b3, int moreBytesExpected)
  {
    return this.AddByte(b1, 2 + moreBytesExpected) && this.AddByte(b2, 1 + moreBytesExpected) && this.AddByte(b3, moreBytesExpected);
  }

  internal bool AddByte(byte b1, byte b2, byte b3, byte b4)
  {
    return this.AddByte(b1, 3) && this.AddByte(b2, 2) && this.AddByte(b3, 1) && this.AddByte(b4, 0);
  }

  internal unsafe void MovePrevious(bool bThrow)
  {
    if (this.fallbackBufferHelper.bFallingBack)
      this.fallbackBuffer.MovePrevious();
    else if (this._chars > this._charStart)
      --this._chars;
    if (!bThrow)
      return;
    this._enc.ThrowBytesOverflow(this._encoder, this._bytes == this._byteStart);
  }

  internal unsafe bool Fallback(char charFallback)
  {
    return this.fallbackBufferHelper.InternalFallback(charFallback, ref this._chars);
  }

  internal unsafe bool MoreData => this.fallbackBuffer.Remaining > 0 || this._chars < this._charEnd;

  internal unsafe char GetNextChar()
  {
    char nextChar = this.fallbackBufferHelper.InternalGetNextChar();
    if (nextChar == char.MinValue && this._chars < this._charEnd)
      nextChar = *this._chars++;
    return nextChar;
  }

  internal unsafe int CharsUsed => (int) (this._chars - this._charStart);

  internal int Count => this._byteCountResult;
}
