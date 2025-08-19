// Decompiled with JetBrains decompiler
// Type: Tesseract.AggregateResultRenderer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using Tesseract.Internal;

#nullable disable
namespace Tesseract;

public class AggregateResultRenderer : DisposableBase, IResultRenderer, IDisposable
{
  private List<IResultRenderer> _resultRenderers;
  private int _pageNumber = -1;
  private IDisposable _currentDocumentHandle;

  public AggregateResultRenderer(params IResultRenderer[] resultRenderers)
    : this((IEnumerable<IResultRenderer>) resultRenderers)
  {
  }

  public AggregateResultRenderer(IEnumerable<IResultRenderer> resultRenderers)
  {
    Guard.RequireNotNull(nameof (resultRenderers), (object) resultRenderers);
    this._resultRenderers = new List<IResultRenderer>(resultRenderers);
  }

  public int PageNumber => this._pageNumber;

  public IEnumerable<IResultRenderer> ResultRenderers
  {
    get => (IEnumerable<IResultRenderer>) this._resultRenderers;
  }

  public bool AddPage(Page page)
  {
    Guard.RequireNotNull(nameof (page), (object) page);
    this.VerifyNotDisposed();
    ++this._pageNumber;
    foreach (IResultRenderer resultRenderer in this.ResultRenderers)
    {
      if (!resultRenderer.AddPage(page))
        return false;
    }
    return true;
  }

  public IDisposable BeginDocument(string title)
  {
    Guard.RequireNotNull(nameof (title), (object) title);
    this.VerifyNotDisposed();
    Guard.Verify((this._currentDocumentHandle == null ? 1 : 0) != 0, "Cannot begin document \"{0}\" as another document is currently being processed which must be dispose off first.", (object) title);
    this._pageNumber = -1;
    List<IDisposable> children = new List<IDisposable>();
    try
    {
      foreach (IResultRenderer resultRenderer in this.ResultRenderers)
        children.Add(resultRenderer.BeginDocument(title));
      this._currentDocumentHandle = (IDisposable) new AggregateResultRenderer.EndDocumentOnDispose(this, (IEnumerable<IDisposable>) children);
      return this._currentDocumentHandle;
    }
    catch (Exception ex1)
    {
      foreach (IDisposable disposable in children)
      {
        try
        {
          disposable.Dispose();
        }
        catch (Exception ex2)
        {
          Logger.TraceError("Failed to dispose of child document {0}: {1}", (object) disposable, (object) ex2.Message);
        }
      }
      throw ex1;
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
      foreach (IDisposable resultRenderer in this.ResultRenderers)
        resultRenderer.Dispose();
      this._resultRenderers = (List<IResultRenderer>) null;
    }
  }

  private class EndDocumentOnDispose : DisposableBase
  {
    private readonly AggregateResultRenderer _renderer;
    private List<IDisposable> _children;

    public EndDocumentOnDispose(AggregateResultRenderer renderer, IEnumerable<IDisposable> children)
    {
      this._renderer = renderer;
      this._children = new List<IDisposable>(children);
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Guard.Verify(this._renderer._currentDocumentHandle == this, "Expected the Result Render's active document to be this document.");
      foreach (IDisposable child in this._children)
        child.Dispose();
      this._children = (List<IDisposable>) null;
      this._renderer._currentDocumentHandle = (IDisposable) null;
    }
  }
}
