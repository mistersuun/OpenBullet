// Decompiled with JetBrains decompiler
// Type: System.Text.EncoderFallbackBufferHelper
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal struct EncoderFallbackBufferHelper
{
  internal unsafe char* charStart;
  internal unsafe char* charEnd;
  internal EncoderNLS encoder;
  internal bool setEncoder;
  internal bool bUsedEncoder;
  internal bool bFallingBack;
  internal int iRecursionCount;
  private const int iMaxRecursion = 250;
  private EncoderFallbackBuffer _fallbackBuffer;

  public unsafe EncoderFallbackBufferHelper(EncoderFallbackBuffer fallbackBuffer)
  {
    this._fallbackBuffer = fallbackBuffer;
    this.bFallingBack = this.bUsedEncoder = this.setEncoder = false;
    this.iRecursionCount = 0;
    this.charEnd = this.charStart = (char*) null;
    this.encoder = (EncoderNLS) null;
  }

  internal unsafe void InternalReset()
  {
    this.charStart = (char*) null;
    this.bFallingBack = false;
    this.iRecursionCount = 0;
    this._fallbackBuffer.Reset();
  }

  internal unsafe void InternalInitialize(
    char* _charStart,
    char* _charEnd,
    EncoderNLS _encoder,
    bool _setEncoder)
  {
    this.charStart = _charStart;
    this.charEnd = _charEnd;
    this.encoder = _encoder;
    this.setEncoder = _setEncoder;
    this.bUsedEncoder = false;
    this.bFallingBack = false;
    this.iRecursionCount = 0;
  }

  internal char InternalGetNextChar()
  {
    char nextChar = this._fallbackBuffer.GetNextChar();
    this.bFallingBack = nextChar > char.MinValue;
    if (nextChar == char.MinValue)
      this.iRecursionCount = 0;
    return nextChar;
  }

  internal unsafe bool InternalFallback(char ch, ref char* chars)
  {
    int index = (int) (chars - this.charStart) - 1;
    if (char.IsHighSurrogate(ch))
    {
      if (chars >= this.charEnd)
      {
        if (this.encoder != null && !this.encoder.MustFlush)
        {
          if (this.setEncoder)
          {
            this.bUsedEncoder = true;
            this.encoder.charLeftOver = ch;
          }
          this.bFallingBack = false;
          return false;
        }
      }
      else
      {
        char ch1 = *chars;
        if (char.IsLowSurrogate(ch1))
        {
          if (this.bFallingBack && this.iRecursionCount++ > 250)
            this.ThrowLastCharRecursive(char.ConvertToUtf32(ch, ch1));
          ++chars;
          this.bFallingBack = this._fallbackBuffer.Fallback(ch, ch1, index);
          return this.bFallingBack;
        }
      }
    }
    if (this.bFallingBack && this.iRecursionCount++ > 250)
      this.ThrowLastCharRecursive((int) ch);
    this.bFallingBack = this._fallbackBuffer.Fallback(ch, index);
    return this.bFallingBack;
  }

  internal void ThrowLastCharRecursive(int charRecursive)
  {
    throw new ArgumentException(SR.Format(SR.Argument_RecursiveFallback, (object) charRecursive), "chars");
  }
}
