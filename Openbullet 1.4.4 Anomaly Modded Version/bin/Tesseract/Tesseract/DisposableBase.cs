// Decompiled with JetBrains decompiler
// Type: Tesseract.DisposableBase
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Diagnostics;

#nullable disable
namespace Tesseract;

public abstract class DisposableBase : IDisposable
{
  private static readonly TraceSource trace = new TraceSource("Tesseract");

  protected DisposableBase() => this.IsDisposed = false;

  ~DisposableBase() => this.Dispose(false);

  public void Dispose()
  {
    this.Dispose(true);
    this.IsDisposed = true;
    GC.SuppressFinalize((object) this);
    if (this.Disposed == null)
      return;
    this.Disposed((object) this, EventArgs.Empty);
  }

  public bool IsDisposed { get; private set; }

  public event EventHandler<EventArgs> Disposed;

  protected virtual void VerifyNotDisposed()
  {
    if (this.IsDisposed)
      throw new ObjectDisposedException(this.ToString());
  }

  protected abstract void Dispose(bool disposing);
}
