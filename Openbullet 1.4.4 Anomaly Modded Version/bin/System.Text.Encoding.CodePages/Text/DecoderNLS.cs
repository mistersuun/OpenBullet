// Decompiled with JetBrains decompiler
// Type: System.Text.DecoderNLS
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Runtime.Serialization;

#nullable disable
namespace System.Text;

internal class DecoderNLS : Decoder, ISerializable
{
  protected EncodingNLS m_encoding;
  protected bool m_mustFlush;
  internal bool m_throwOnOverflow;
  internal int m_bytesUsed;
  internal new DecoderFallback m_fallback;
  internal new DecoderFallbackBuffer m_fallbackBuffer;

  internal DecoderNLS(EncodingNLS encoding)
  {
    this.m_encoding = encoding;
    this.m_fallback = this.m_encoding.DecoderFallback;
    this.Reset();
  }

  internal DecoderNLS()
  {
    this.m_encoding = (EncodingNLS) null;
    this.Reset();
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    throw new PlatformNotSupportedException();
  }

  internal new DecoderFallback Fallback => this.m_fallback;

  internal new bool InternalHasFallbackBuffer => this.m_fallbackBuffer != null;

  public new DecoderFallbackBuffer FallbackBuffer
  {
    get
    {
      if (this.m_fallbackBuffer == null)
        this.m_fallbackBuffer = this.m_fallback == null ? DecoderFallback.ReplacementFallback.CreateFallbackBuffer() : this.m_fallback.CreateFallbackBuffer();
      return this.m_fallbackBuffer;
    }
  }

  public override void Reset()
  {
    if (this.m_fallbackBuffer == null)
      return;
    this.m_fallbackBuffer.Reset();
  }

  public override int GetCharCount(byte[] bytes, int index, int count)
  {
    return this.GetCharCount(bytes, index, count, false);
  }

  public override unsafe int GetCharCount(byte[] bytes, int index, int count, bool flush)
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes), SR.ArgumentNull_Array);
    if (index < 0 || count < 0)
      throw new ArgumentOutOfRangeException(index < 0 ? nameof (index) : nameof (count), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (bytes.Length - index < count)
      throw new ArgumentOutOfRangeException(nameof (bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (bytes.Length == 0)
      bytes = new byte[1];
    fixed (byte* numPtr = &bytes[0])
      return this.GetCharCount(numPtr + index, count, flush);
  }

  public override unsafe int GetCharCount(byte* bytes, int count, bool flush)
  {
    if ((IntPtr) bytes == IntPtr.Zero)
      throw new ArgumentNullException(nameof (bytes), SR.ArgumentNull_Array);
    if (count < 0)
      throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = true;
    return this.m_encoding.GetCharCount(bytes, count, this);
  }

  public override int GetChars(
    byte[] bytes,
    int byteIndex,
    int byteCount,
    char[] chars,
    int charIndex)
  {
    return this.GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
  }

  public override unsafe int GetChars(
    byte[] bytes,
    int byteIndex,
    int byteCount,
    char[] chars,
    int charIndex,
    bool flush)
  {
    if (bytes == null || chars == null)
      throw new ArgumentNullException(bytes == null ? nameof (bytes) : nameof (chars), SR.ArgumentNull_Array);
    if (byteIndex < 0 || byteCount < 0)
      throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof (byteIndex) : nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (bytes.Length - byteIndex < byteCount)
      throw new ArgumentOutOfRangeException(nameof (bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (charIndex < 0 || charIndex > chars.Length)
      throw new ArgumentOutOfRangeException(nameof (charIndex), SR.ArgumentOutOfRange_Index);
    if (bytes.Length == 0)
      bytes = new byte[1];
    int charCount = chars.Length - charIndex;
    if (chars.Length == 0)
      chars = new char[1];
    fixed (byte* numPtr = &bytes[0])
      fixed (char* chPtr = &chars[0])
        return this.GetChars(numPtr + byteIndex, byteCount, chPtr + charIndex, charCount, flush);
  }

  public override unsafe int GetChars(
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    bool flush)
  {
    if ((IntPtr) chars == IntPtr.Zero || (IntPtr) bytes == IntPtr.Zero)
      throw new ArgumentNullException((IntPtr) chars == IntPtr.Zero ? nameof (chars) : nameof (bytes), SR.ArgumentNull_Array);
    if (byteCount < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof (byteCount) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = true;
    return this.m_encoding.GetChars(bytes, byteCount, chars, charCount, this);
  }

  public override unsafe void Convert(
    byte[] bytes,
    int byteIndex,
    int byteCount,
    char[] chars,
    int charIndex,
    int charCount,
    bool flush,
    out int bytesUsed,
    out int charsUsed,
    out bool completed)
  {
    if (bytes == null || chars == null)
      throw new ArgumentNullException(bytes == null ? nameof (bytes) : nameof (chars), SR.ArgumentNull_Array);
    if (byteIndex < 0 || byteCount < 0)
      throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof (byteIndex) : nameof (byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (charIndex < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof (charIndex) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (bytes.Length - byteIndex < byteCount)
      throw new ArgumentOutOfRangeException(nameof (bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (chars.Length - charIndex < charCount)
      throw new ArgumentOutOfRangeException(nameof (chars), SR.ArgumentOutOfRange_IndexCountBuffer);
    if (bytes.Length == 0)
      bytes = new byte[1];
    if (chars.Length == 0)
      chars = new char[1];
    fixed (byte* numPtr = &bytes[0])
      fixed (char* chPtr = &chars[0])
        this.Convert(numPtr + byteIndex, byteCount, chPtr + charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
  }

  public override unsafe void Convert(
    byte* bytes,
    int byteCount,
    char* chars,
    int charCount,
    bool flush,
    out int bytesUsed,
    out int charsUsed,
    out bool completed)
  {
    if ((IntPtr) chars == IntPtr.Zero || (IntPtr) bytes == IntPtr.Zero)
      throw new ArgumentNullException((IntPtr) chars == IntPtr.Zero ? nameof (chars) : nameof (bytes), SR.ArgumentNull_Array);
    if (byteCount < 0 || charCount < 0)
      throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof (byteCount) : nameof (charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
    this.m_mustFlush = flush;
    this.m_throwOnOverflow = false;
    this.m_bytesUsed = 0;
    charsUsed = this.m_encoding.GetChars(bytes, byteCount, chars, charCount, this);
    bytesUsed = this.m_bytesUsed;
    completed = bytesUsed == byteCount && (!flush || !this.HasState) && (this.m_fallbackBuffer == null || this.m_fallbackBuffer.Remaining == 0);
  }

  public bool MustFlush => this.m_mustFlush;

  internal virtual bool HasState => false;

  internal void ClearMustFlush() => this.m_mustFlush = false;
}
