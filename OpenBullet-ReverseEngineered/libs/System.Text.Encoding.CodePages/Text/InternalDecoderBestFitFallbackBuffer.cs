// Decompiled with JetBrains decompiler
// Type: System.Text.InternalDecoderBestFitFallbackBuffer
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Threading;

#nullable disable
namespace System.Text;

internal sealed class InternalDecoderBestFitFallbackBuffer : DecoderFallbackBuffer
{
  internal char cBestFit;
  internal int iCount = -1;
  internal int iSize;
  private InternalDecoderBestFitFallback _oFallback;
  private static object s_InternalSyncObject;

  private static object InternalSyncObject
  {
    get
    {
      if (InternalDecoderBestFitFallbackBuffer.s_InternalSyncObject == null)
      {
        object obj = new object();
        Interlocked.CompareExchange<object>(ref InternalDecoderBestFitFallbackBuffer.s_InternalSyncObject, obj, (object) null);
      }
      return InternalDecoderBestFitFallbackBuffer.s_InternalSyncObject;
    }
  }

  public InternalDecoderBestFitFallbackBuffer(InternalDecoderBestFitFallback fallback)
  {
    this._oFallback = fallback;
    if (this._oFallback.arrayBestFit != null)
      return;
    lock (InternalDecoderBestFitFallbackBuffer.InternalSyncObject)
    {
      if (this._oFallback.arrayBestFit != null)
        return;
      this._oFallback.arrayBestFit = fallback.encoding.GetBestFitBytesToUnicodeData();
    }
  }

  public override bool Fallback(byte[] bytesUnknown, int index)
  {
    this.cBestFit = this.TryBestFit(bytesUnknown);
    if (this.cBestFit == char.MinValue)
      this.cBestFit = this._oFallback.cReplacement;
    this.iCount = this.iSize = 1;
    return true;
  }

  public override char GetNextChar()
  {
    --this.iCount;
    if (this.iCount < 0)
      return char.MinValue;
    if (this.iCount != int.MaxValue)
      return this.cBestFit;
    this.iCount = -1;
    return char.MinValue;
  }

  public override bool MovePrevious()
  {
    if (this.iCount >= 0)
      ++this.iCount;
    return this.iCount >= 0 && this.iCount <= this.iSize;
  }

  public override int Remaining => this.iCount <= 0 ? 0 : this.iCount;

  public override void Reset() => this.iCount = -1;

  internal new unsafe int InternalFallback(byte[] bytes, byte* pBytes) => 1;

  private char TryBestFit(byte[] bytesCheck)
  {
    int num1 = 0;
    int num2 = this._oFallback.arrayBestFit.Length;
    if (num2 == 0 || bytesCheck.Length == 0 || bytesCheck.Length > 2)
      return char.MinValue;
    char ch1 = bytesCheck.Length != 1 ? (char) (((uint) bytesCheck[0] << 8) + (uint) bytesCheck[1]) : (char) bytesCheck[0];
    if ((int) ch1 < (int) this._oFallback.arrayBestFit[0] || (int) ch1 > (int) this._oFallback.arrayBestFit[num2 - 2])
      return char.MinValue;
    int num3;
    while ((num3 = num2 - num1) > 6)
    {
      int index = num3 / 2 + num1 & 65534;
      char ch2 = this._oFallback.arrayBestFit[index];
      if ((int) ch2 == (int) ch1)
        return this._oFallback.arrayBestFit[index + 1];
      if ((int) ch2 < (int) ch1)
        num1 = index;
      else
        num2 = index;
    }
    for (int index = num1; index < num2; index += 2)
    {
      if ((int) this._oFallback.arrayBestFit[index] == (int) ch1)
        return this._oFallback.arrayBestFit[index + 1];
    }
    return char.MinValue;
  }
}
