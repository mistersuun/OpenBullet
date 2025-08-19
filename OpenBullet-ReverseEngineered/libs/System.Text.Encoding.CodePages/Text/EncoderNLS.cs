// Decompiled with JetBrains decompiler
// Type: System.Text.EncoderNLS
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Runtime.Serialization;

#nullable disable
namespace System.Text;

internal class EncoderNLS : Encoder, ISerializable
{
  internal char charLeftOver;
  protected EncodingNLS m_encoding;
  protected bool m_mustFlush;
  internal bool m_throwOnOverflow;
  internal int m_charsUsed;
  internal new EncoderFallback m_fallback;
  internal new EncoderFallbackBuffer m_fallbackBuffer;

  internal EncoderNLS(EncodingNLS encoding)
  {
    this.m_encoding = encoding;
    this.m_fallback = this.m_encoding.EncoderFallback;
    this.Reset();
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    throw new PlatformNotSupportedException();
  }

  internal new EncoderFallback Fallback => this.m_fallback;

  internal new bool InternalHasFallbackBuffer => this.m_fallbackBuffer != null;

  public new EncoderFallbackBuffer FallbackBuffer
  {
    get
    {
      if (this.m_fallbackBuffer == null)
        this.m_fallbackBuffer = this.m_fallback == null ? EncoderFallback.ReplacementFallback.CreateFallbackBuffer() : this.m_fallback.CreateFallbackBuffer();
      return this.m_fallbackBuffer;
    }
  }

  internal EncoderNLS()
  {
    this.m_encoding = (EncodingNLS) null;
    this.Reset();
  }

  public override void Reset()
  {
    this.charLeftOver = char.MinValue;
    if (this.m_fallbackBuffer == null)
      return;
    this.m_fallbackBuffer.Reset();
  }

  public override unsafe int GetByteCount(char[] chars, int index, int count, bool flush)
  {
    if (chars == null)
      throw new ArgumentNullException(nameof (chars), SR.ArgumentNull_Array);
    if (index < 0 || count < 0)
      throw new ArgumentOutOfRangeException(index < 0 ? nameof (index) : nameof (count), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (chars.Length - index < count)
      throw new ArgumentOutOfRangeException(nameof (chars), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (chars.Length == 0)
      chars = new char[1];
    int byteCount;
    fixed (char* chPtr = &chars[0])
      byteCount = this.GetByteCount(chPtr + index, count, flush);
    return byteCount;
  }

  public override unsafe int GetByteCount(char* chars, int count, bool flush)
  {
    if ((IntPtr) chars == IntPtr.Zero)
      throw new ArgumentNullException(nameof (chars), SR.ArgumentNull_Array);
    if (count < 0)
      throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = true;
    return this.m_encoding.GetByteCount(chars, count, this);
  }

  public override unsafe int GetBytes(
    char[] chars,
    int charIndex,
    int charCount,
    byte[] bytes,
    int byteIndex,
    bool flush)
  {
    if (chars == null || bytes == null)
      throw new ArgumentNullException(chars == null ? nameof (chars) : nameof (bytes), SR.ArgumentNull_Array);
    if (charIndex < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof (charIndex) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (chars.Length - charIndex < charCount)
      throw new ArgumentOutOfRangeException(nameof (chars), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (byteIndex < 0 || byteIndex > bytes.Length)
      throw new ArgumentOutOfRangeException(nameof (byteIndex), SR.ArgumentOutOfRange_Index);
    if (chars.Length == 0)
      chars = new char[1];
    int byteCount = bytes.Length - byteIndex;
    if (bytes.Length == 0)
      bytes = new byte[1];
    fixed (char* chPtr = &chars[0])
      fixed (byte* numPtr = &bytes[0])
        return this.GetBytes(chPtr + charIndex, charCount, numPtr + byteIndex, byteCount, flush);
  }

  public override unsafe int GetBytes(
    char* chars,
    int charCount,
    byte* bytes,
    int byteCount,
    bool flush)
  {
    if ((IntPtr) chars == IntPtr.Zero || (IntPtr) bytes == IntPtr.Zero)
      throw new ArgumentNullException((IntPtr) chars == IntPtr.Zero ? nameof (chars) : nameof (bytes), SR.ArgumentNull_Array);
    if (byteCount < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof (byteCount) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = true;
    return this.m_encoding.GetBytes(chars, charCount, bytes, byteCount, this);
  }

  public override unsafe void Convert(
    char[] chars,
    int charIndex,
    int charCount,
    byte[] bytes,
    int byteIndex,
    int byteCount,
    bool flush,
    out int charsUsed,
    out int bytesUsed,
    out bool completed)
  {
    if (chars == null || bytes == null)
      throw new ArgumentNullException(chars == null ? nameof (chars) : nameof (bytes), SR.ArgumentNull_Array);
    if (charIndex < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof (charIndex) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (byteIndex < 0 || byteCount < 0)
      throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof (byteIndex) : nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (chars.Length - charIndex < charCount)
      throw new ArgumentOutOfRangeException(nameof (chars), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (bytes.Length - byteIndex < byteCount)
      throw new ArgumentOutOfRangeException(nameof (bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (chars.Length == 0)
      chars = new char[1];
    if (bytes.Length == 0)
      bytes = new byte[1];
    fixed (char* chPtr = &chars[0])
      fixed (byte* numPtr = &bytes[0])
        this.Convert(chPtr + charIndex, charCount, numPtr + byteIndex, byteCount, flush, out charsUsed, out bytesUsed, out completed);
  }

  public override unsafe void Convert(
    char* chars,
    int charCount,
    byte* bytes,
    int byteCount,
    bool flush,
    out int charsUsed,
    out int bytesUsed,
    out bool completed)
  {
    if ((IntPtr) bytes == IntPtr.Zero || (IntPtr) chars == IntPtr.Zero)
      throw new ArgumentNullException((IntPtr) bytes == IntPtr.Zero ? nameof (bytes) : nameof (chars), SR.ArgumentNull_Array);
    if (charCount < 0 || byteCount < 0)
      throw new ArgumentOutOfRangeException(charCount < 0 ? nameof (charCount) : nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = false;
    this.m_charsUsed = 0;
    bytesUsed = this.m_encoding.GetBytes(chars, charCount, bytes, byteCount, this);
    charsUsed = this.m_charsUsed;
    completed = charsUsed == charCount && (!flush || !this.HasState) && (this.m_fallbackBuffer == null || this.m_fallbackBuffer.Remaining == 0);
  }

  public Encoding Encoding => (Encoding) this.m_encoding;

  public bool MustFlush => this.m_mustFlush;

  internal virtual bool HasState => this.charLeftOver > char.MinValue;

  internal void ClearMustFlush() => this.m_mustFlush = false;
}
