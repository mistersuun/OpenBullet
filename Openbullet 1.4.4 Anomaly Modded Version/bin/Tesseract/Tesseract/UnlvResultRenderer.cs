// Decompiled with JetBrains decompiler
// Type: Tesseract.UnlvResultRenderer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class UnlvResultRenderer : ResultRenderer
{
  public UnlvResultRenderer(string outputFilename)
  {
    this.Initialise(TessApi.Native.UnlvRendererCreate(outputFilename));
  }
}
