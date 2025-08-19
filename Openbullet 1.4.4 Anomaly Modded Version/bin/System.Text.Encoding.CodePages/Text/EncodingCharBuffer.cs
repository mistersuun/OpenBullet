// Decompiled with JetBrains decompiler
// Type: System.Text.EncodingCharBuffer
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal class EncodingCharBuffer
{
  private unsafe char* _chars;
  private unsafe char* _charStart;
  private unsafe char* _charEnd;
  private int _charCountResult;
  private EncodingNLS _enc;
  private DecoderNLS _decoder;
  private unsafe byte* _byteStart;
  private unsafe byte* _byteEnd;
  private unsafe byte* _bytes;
  private DecoderFallbackBuffer _fallbackBuffer;
  private DecoderFallbackBufferHelper _fallbackBufferHelper;

  internal unsafe EncodingCharBuffer(
    EncodingNLS enc,
    DecoderNLS decoder,
    char* charStart,
    int charCount,
    byte* byteStart,
    int byteCount)
  {
    this._enc = enc;
    this._decoder = decoder;
    this._chars = charStart;
    this._charStart = charStart;
    this._charEnd = charStart + charCount;
    this._byteStart = byteStart;
    this._bytes = byteStart;
    this._byteEnd = byteStart + byteCount;
    this._fallbackBuffer = this._decoder != null ? this._decoder.FallbackBuffer : enc.DecoderFallback.CreateFallbackBuffer();
    this._fallbackBufferHelper = new DecoderFallbackBufferHelper(this._fallbackBuffer);
    this._fallbackBufferHelper.InternalInitialize(this._bytes, this._charEnd);
  }

  internal unsafe bool AddChar(char ch, int numBytes)
  {
    if ((IntPtr) this._chars != IntPtr.Zero)
    {
      if (this._chars >= this._charEnd)
      {
        this._bytes -= numBytes;
        this._enc.ThrowCharsOverflow(this._decoder, this._bytes <= this._byteStart);
        return false;
      }
      *this._chars++ = ch;
    }
    ++this._charCountResult;
    return true;
  }

  internal bool AddChar(char ch) => this.AddChar(ch, 1);

  internal unsafe bool AddChar(char ch1, char ch2, int numBytes)
  {
    if (this._chars >= this._charEnd - 1)
    {
      this._bytes -= numBytes;
      this._enc.ThrowCharsOverflow(this._decoder, this._bytes <= this._byteStart);
      return false;
    }
    return this.AddChar(ch1, numBytes) && this.AddChar(ch2, numBytes);
  }

  internal unsafe void AdjustBytes(int count) => this._bytes += count;

  internal unsafe bool MoreData => this._bytes < this._byteEnd;

  internal unsafe bool EvenMoreData(int count) => this._bytes <= this._byteEnd - count;

  internal unsafe byte GetNextByte() => this._bytes >= this._byteEnd ? (byte) 0 : *this._bytes++;

  internal unsafe int BytesUsed => (int) (this._bytes - this._byteStart);

  internal bool Fallback(byte fallbackByte)
  {
    return this.Fallback(new byte[1]{ fallbackByte });
  }

  internal bool Fallback(byte byte1, byte byte2)
  {
    return this.Fallback(new byte[2]{ byte1, byte2 });
  }

  internal bool Fallback(byte byte1, byte byte2, byte byte3, byte byte4)
  {
    return this.Fallback(new byte[4]
    {
      byte1,
      byte2,
      byte3,
      byte4
    });
  }

  internal unsafe bool Fallback(byte[] byteBuffer)
  {
    if ((IntPtr) this._chars != IntPtr.Zero)
    {
      char* chars = this._chars;
      if (!this._fallbackBufferHelper.InternalFallback(byteBuffer, this._bytes, ref this._chars))
      {
        this._bytes -= byteBuffer.Length;
        this._fallbackBufferHelper.InternalReset();
        this._enc.ThrowCharsOverflow(this._decoder, this._chars == this._charStart);
        return false;
      }
      this._charCountResult += (int) (this._chars - chars);
    }
    else
      this._charCountResult += this._fallbackBufferHelper.InternalFallback(byteBuffer, this._bytes);
    return true;
  }

  internal int Count => this._charCountResult;
}
