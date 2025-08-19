// Decompiled with JetBrains decompiler
// Type: System.Text.InternalDecoderBestFitFallback
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

#nullable disable
namespace System.Text;

internal sealed class InternalDecoderBestFitFallback : DecoderFallback
{
  internal BaseCodePageEncoding encoding;
  internal char[] arrayBestFit;
  internal char cReplacement = '?';

  internal InternalDecoderBestFitFallback(BaseCodePageEncoding _encoding)
  {
    this.encoding = _encoding;
  }

  public override DecoderFallbackBuffer CreateFallbackBuffer()
  {
    return (DecoderFallbackBuffer) new InternalDecoderBestFitFallbackBuffer(this);
  }

  public override int MaxCharCount => 1;

  public override bool Equals(object value)
  {
    return value is InternalDecoderBestFitFallback decoderBestFitFallback && this.encoding.CodePage == decoderBestFitFallback.encoding.CodePage;
  }

  public override int GetHashCode() => this.encoding.CodePage;
}
