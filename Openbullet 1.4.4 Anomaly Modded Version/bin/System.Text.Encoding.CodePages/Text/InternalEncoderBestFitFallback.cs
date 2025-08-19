// Decompiled with JetBrains decompiler
// Type: System.Text.InternalEncoderBestFitFallback
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal class InternalEncoderBestFitFallback : EncoderFallback
{
  internal BaseCodePageEncoding encoding;
  internal char[] arrayBestFit;

  internal InternalEncoderBestFitFallback(BaseCodePageEncoding _encoding)
  {
    this.encoding = _encoding;
  }

  public override EncoderFallbackBuffer CreateFallbackBuffer()
  {
    return (EncoderFallbackBuffer) new InternalEncoderBestFitFallbackBuffer(this);
  }

  public override int MaxCharCount => 1;

  public override bool Equals(object value)
  {
    return value is InternalEncoderBestFitFallback encoderBestFitFallback && this.encoding.CodePage == encoderBestFitFallback.encoding.CodePage;
  }

  public override int GetHashCode() => this.encoding.CodePage;
}
