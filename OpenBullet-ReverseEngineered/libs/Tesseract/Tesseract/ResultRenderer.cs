// Decompiled with JetBrains decompiler
// Type: Tesseract.ResultRenderer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tesseract.Internal;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public abstract class ResultRenderer : DisposableBase, IResultRenderer, IDisposable
{
  private HandleRef _handle;
  private IDisposable _currentDocumentHandle;

  public static IResultRenderer CreateRenderers(
    string outputbase,
    string dataPath,
    List<RenderedFormat> outputFormats)
  {
    IResultRenderer renderers = (IResultRenderer) null;
    foreach (int outputFormat in outputFormats)
    {
      switch (outputFormat)
      {
        case 0:
          if (renderers == null)
          {
            renderers = ResultRenderer.CreateTextRenderer(outputbase);
            continue;
          }
          TessApi.Native.ResultRendererInsert(((ResultRenderer) renderers).Handle, new TextResultRenderer(outputbase).Handle);
          continue;
        case 1:
          if (renderers == null)
          {
            renderers = ResultRenderer.CreateHOcrRenderer(outputbase);
            continue;
          }
          TessApi.Native.ResultRendererInsert(((ResultRenderer) renderers).Handle, new HOcrResultRenderer(outputbase).Handle);
          continue;
        case 2:
          if (renderers == null)
          {
            renderers = ResultRenderer.CreatePdfRenderer(outputbase, dataPath);
            continue;
          }
          TessApi.Native.ResultRendererInsert(((ResultRenderer) renderers).Handle, new PdfResultRenderer(outputbase, dataPath).Handle);
          continue;
        case 3:
          if (renderers == null)
          {
            renderers = ResultRenderer.CreateUnlvRenderer(outputbase);
            continue;
          }
          TessApi.Native.ResultRendererInsert(((ResultRenderer) renderers).Handle, new UnlvResultRenderer(outputbase).Handle);
          continue;
        case 4:
          if (renderers == null)
          {
            renderers = ResultRenderer.CreateBoxRenderer(outputbase);
            continue;
          }
          TessApi.Native.ResultRendererInsert(((ResultRenderer) renderers).Handle, new BoxResultRenderer(outputbase).Handle);
          continue;
        default:
          continue;
      }
    }
    return renderers;
  }

  public static IResultRenderer CreatePdfRenderer(string outputFilename, string fontDirectory)
  {
    return (IResultRenderer) new PdfResultRenderer(outputFilename, fontDirectory);
  }

  public static IResultRenderer CreateTextRenderer(string outputFilename)
  {
    return (IResultRenderer) new TextResultRenderer(outputFilename);
  }

  public static IResultRenderer CreateHOcrRenderer(string outputFilename, bool fontInfo = false)
  {
    return (IResultRenderer) new HOcrResultRenderer(outputFilename, fontInfo);
  }

  public static IResultRenderer CreateUnlvRenderer(string outputFilename)
  {
    return (IResultRenderer) new UnlvResultRenderer(outputFilename);
  }

  public static IResultRenderer CreateBoxRenderer(string outputFilename)
  {
    return (IResultRenderer) new BoxResultRenderer(outputFilename);
  }

  protected ResultRenderer() => this._handle = new HandleRef((object) this, IntPtr.Zero);

  protected void Initialise(IntPtr handle)
  {
    Guard.Require(nameof (handle), handle != IntPtr.Zero, "handle must be initialised.");
    Guard.Verify(this._handle.Handle == IntPtr.Zero, "Rensult renderer has already been initialised.");
    this._handle = new HandleRef((object) this, handle);
  }

  public bool AddPage(Page page)
  {
    Guard.RequireNotNull(nameof (page), (object) page);
    this.VerifyNotDisposed();
    page.Recognize();
    return TessApi.Native.ResultRendererAddImage(this.Handle, page.Engine.Handle) != 0;
  }

  public IDisposable BeginDocument(string title)
  {
    Guard.RequireNotNull(nameof (title), (object) title);
    this.VerifyNotDisposed();
    Guard.Verify((this._currentDocumentHandle == null ? 1 : 0) != 0, "Cannot begin document \"{0}\" as another document is currently being processed which must be dispose off first.", (object) title);
    IntPtr hglobalAnsi = Marshal.StringToHGlobalAnsi(title);
    if (TessApi.Native.ResultRendererBeginDocument(this.Handle, hglobalAnsi) == 0)
    {
      Marshal.FreeHGlobal(hglobalAnsi);
      throw new InvalidOperationException($"Failed to begin document \"{title}\".");
    }
    this._currentDocumentHandle = (IDisposable) new ResultRenderer.EndDocumentOnDispose(this, hglobalAnsi);
    return this._currentDocumentHandle;
  }

  protected HandleRef Handle => this._handle;

  public int PageNumber
  {
    get
    {
      this.VerifyNotDisposed();
      return TessApi.Native.ResultRendererImageNum(this.Handle);
    }
  }

  protected override void Dispose(bool disposing)
  {
    try
    {
      if (!disposing || this._currentDocumentHandle == null)
        return;
      this._currentDocumentHandle.Dispose();
      this._currentDocumentHandle = (IDisposable) null;
    }
    finally
    {
      if (this._handle.Handle != IntPtr.Zero)
      {
        TessApi.Native.DeleteResultRenderer(this._handle);
        this._handle = new HandleRef((object) this, IntPtr.Zero);
      }
    }
  }

  private class EndDocumentOnDispose : DisposableBase
  {
    private readonly ResultRenderer _renderer;
    private IntPtr _titlePtr;

    public EndDocumentOnDispose(ResultRenderer renderer, IntPtr titlePtr)
    {
      this._renderer = renderer;
      this._titlePtr = titlePtr;
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!disposing)
          return;
        Guard.Verify(this._renderer._currentDocumentHandle == this, "Expected the Result Render's active document to be this document.");
        TessApi.Native.ResultRendererEndDocument(this._renderer._handle);
        this._renderer._currentDocumentHandle = (IDisposable) null;
      }
      finally
      {
        if (this._titlePtr != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(this._titlePtr);
          this._titlePtr = IntPtr.Zero;
        }
      }
    }
  }
}
