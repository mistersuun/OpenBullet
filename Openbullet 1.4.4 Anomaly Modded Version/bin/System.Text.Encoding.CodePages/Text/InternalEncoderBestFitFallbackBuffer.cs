// Decompiled with JetBrains decompiler
// Type: System.Text.InternalEncoderBestFitFallbackBuffer
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Threading;

#nullable disable
namespace System.Text;

internal sealed class InternalEncoderBestFitFallbackBuffer : EncoderFallbackBuffer
{
  private char _cBestFit;
  private InternalEncoderBestFitFallback _oFallback;
  private int _iCount = -1;
  private int _iSize;
  private static object s_InternalSyncObject;

  private static object InternalSyncObject
  {
    get
    {
      if (InternalEncoderBestFitFallbackBuffer.s_InternalSyncObject == null)
      {
        object obj = new object();
        Interlocked.CompareExchange<object>(ref InternalEncoderBestFitFallbackBuffer.s_InternalSyncObject, obj, (object) null);
      }
      return InternalEncoderBestFitFallbackBuffer.s_InternalSyncObject;
    }
  }

  public InternalEncoderBestFitFallbackBuffer(InternalEncoderBestFitFallback fallback)
  {
    this._oFallback = fallback;
    if (this._oFallback.arrayBestFit != null)
      return;
    lock (InternalEncoderBestFitFallbackBuffer.InternalSyncObject)
    {
      if (this._oFallback.arrayBestFit != null)
        return;
      this._oFallback.arrayBestFit = fallback.encoding.GetBestFitUnicodeToBytesData();
    }
  }

  public override bool Fallback(char charUnknown, int index)
  {
    this._iCount = this._iSize = 1;
    this._cBestFit = this.TryBestFit(charUnknown);
    if (this._cBestFit == char.MinValue)
      this._cBestFit = '?';
    return true;
  }

  public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
  {
    if (!char.IsHighSurrogate(charUnknownHigh))
      throw new ArgumentOutOfRangeException(nameof (charUnknownHigh), SR.Format(SR.ArgumentOutOfRange_Range, (object) 55296, (object) 56319));
    if (!char.IsLowSurrogate(charUnknownLow))
      throw new ArgumentOutOfRangeException(nameof (charUnknownLow), SR.Format(SR.ArgumentOutOfRange_Range, (object) 56320, (object) 57343 /*0xDFFF*/));
    this._cBestFit = '?';
    this._iCount = this._iSize = 2;
    return true;
  }

  public override char GetNextChar()
  {
    --this._iCount;
    if (this._iCount < 0)
      return char.MinValue;
    if (this._iCount != int.MaxValue)
      return this._cBestFit;
    this._iCount = -1;
    return char.MinValue;
  }

  public override bool MovePrevious()
  {
    if (this._iCount >= 0)
      ++this._iCount;
    return this._iCount >= 0 && this._iCount <= this._iSize;
  }

  public override int Remaining => this._iCount <= 0 ? 0 : this._iCount;

  public override void Reset() => this._iCount = -1;

  private char TryBestFit(char cUnknown)
  {
    int num1 = 0;
    int num2 = this._oFallback.arrayBestFit.Length;
    int num3;
    while ((num3 = num2 - num1) > 6)
    {
      int index = num3 / 2 + num1 & 65534;
      char ch = this._oFallback.arrayBestFit[index];
      if ((int) ch == (int) cUnknown)
        return this._oFallback.arrayBestFit[index + 1];
      if ((int) ch < (int) cUnknown)
        num1 = index;
      else
        num2 = index;
    }
    for (int index = num1; index < num2; index += 2)
    {
      if ((int) this._oFallback.arrayBestFit[index] == (int) cUnknown)
        return this._oFallback.arrayBestFit[index + 1];
    }
    return char.MinValue;
  }
}
