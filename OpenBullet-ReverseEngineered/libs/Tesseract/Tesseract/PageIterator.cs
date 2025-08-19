// Decompiled with JetBrains decompiler
// Type: Tesseract.PageIterator
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public class PageIterator : DisposableBase
{
  protected readonly Page page;
  protected readonly HandleRef handle;

  internal PageIterator(Page page, IntPtr handle)
  {
    this.page = page;
    this.handle = new HandleRef((object) this, handle);
  }

  public void Begin()
  {
    this.VerifyNotDisposed();
    if (!(this.handle.Handle != IntPtr.Zero))
      return;
    TessApi.Native.PageIteratorBegin(this.handle);
  }

  public bool Next(PageIteratorLevel level)
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.PageIteratorNext(this.handle, level) != 0;
  }

  public bool Next(PageIteratorLevel level, PageIteratorLevel element)
  {
    this.VerifyNotDisposed();
    return !this.IsAtFinalOf(level, element) && this.Next(element);
  }

  public bool IsAtBeginningOf(PageIteratorLevel level)
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.PageIteratorIsAtBeginningOf(this.handle, level) != 0;
  }

  public bool IsAtFinalOf(PageIteratorLevel level, PageIteratorLevel element)
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.PageIteratorIsAtFinalElement(this.handle, level, element) != 0;
  }

  public PolyBlockType BlockType
  {
    get
    {
      this.VerifyNotDisposed();
      return this.handle.Handle == IntPtr.Zero ? PolyBlockType.Unknown : TessApi.Native.PageIteratorBlockType(this.handle);
    }
  }

  public Pix GetBinaryImage(PageIteratorLevel level)
  {
    this.VerifyNotDisposed();
    return this.handle.Handle == IntPtr.Zero ? (Pix) null : Pix.Create(TessApi.Native.PageIteratorGetBinaryImage(this.handle, level));
  }

  public Pix GetImage(PageIteratorLevel level, int padding, out int x, out int y)
  {
    this.VerifyNotDisposed();
    if (!(this.handle.Handle == IntPtr.Zero))
      return Pix.Create(TessApi.Native.PageIteratorGetImage(this.handle, level, padding, this.page.Image.Handle, out x, out y));
    x = 0;
    y = 0;
    return (Pix) null;
  }

  public bool TryGetBoundingBox(PageIteratorLevel level, out Rect bounds)
  {
    this.VerifyNotDisposed();
    int left;
    int top;
    int right;
    int bottom;
    if (this.handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBoundingBox(this.handle, level, out left, out top, out right, out bottom) != 0)
    {
      bounds = Rect.FromCoords(left, top, right, bottom);
      return true;
    }
    bounds = Rect.Empty;
    return false;
  }

  public bool TryGetBaseline(PageIteratorLevel level, out Rect bounds)
  {
    this.VerifyNotDisposed();
    int x1;
    int y1;
    int x2;
    int y2;
    if (this.handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBaseline(this.handle, level, out x1, out y1, out x2, out y2) != 0)
    {
      bounds = Rect.FromCoords(x1, y1, x2, y2);
      return true;
    }
    bounds = Rect.Empty;
    return false;
  }

  public ElementProperties GetProperties()
  {
    this.VerifyNotDisposed();
    if (this.handle.Handle == IntPtr.Zero)
      return new ElementProperties(Orientation.PageUp, TextLineOrder.TopToBottom, WritingDirection.LeftToRight, 0.0f);
    Orientation orientation;
    WritingDirection writing_direction;
    TextLineOrder textLineOrder;
    float deskew_angle;
    TessApi.Native.PageIteratorOrientation(this.handle, out orientation, out writing_direction, out textLineOrder, out deskew_angle);
    return new ElementProperties(orientation, textLineOrder, writing_direction, deskew_angle);
  }

  protected override void Dispose(bool disposing)
  {
    if (!(this.handle.Handle != IntPtr.Zero))
      return;
    TessApi.Native.PageIteratorDelete(this.handle);
  }
}
