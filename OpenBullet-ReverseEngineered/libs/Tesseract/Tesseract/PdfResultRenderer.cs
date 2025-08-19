// Decompiled with JetBrains decompiler
// Type: Tesseract.PdfResultRenderer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class PdfResultRenderer : ResultRenderer
{
  private IntPtr _fontDirectoryHandle;

  public PdfResultRenderer(string outputFilename, string fontDirectory)
  {
    IntPtr hglobalAnsi = Marshal.StringToHGlobalAnsi(fontDirectory);
    this.Initialise(TessApi.Native.PDFRendererCreate(outputFilename, hglobalAnsi));
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!(this._fontDirectoryHandle != IntPtr.Zero))
      return;
    Marshal.FreeHGlobal(this._fontDirectoryHandle);
    this._fontDirectoryHandle = IntPtr.Zero;
  }
}
