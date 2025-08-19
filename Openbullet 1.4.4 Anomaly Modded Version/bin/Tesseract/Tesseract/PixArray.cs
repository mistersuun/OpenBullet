// Decompiled with JetBrains decompiler
// Type: Tesseract.PixArray
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract.Internal;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public class PixArray : DisposableBase, IEnumerable<Pix>, IEnumerable
{
  private HandleRef _handle;
  private int _count;
  private int version;

  public static PixArray LoadMultiPageTiffFromFile(string filename)
  {
    IntPtr handle = LeptonicaApi.Native.pixaReadMultipageTiff(filename);
    return !(handle == IntPtr.Zero) ? new PixArray(handle) : throw new IOException($"Failed to load image '{filename}'.");
  }

  public static PixArray Create(int n)
  {
    IntPtr handle = LeptonicaApi.Native.pixaCreate(n);
    return !(handle == IntPtr.Zero) ? new PixArray(handle) : throw new IOException("Failed to create PixArray");
  }

  private PixArray(IntPtr handle)
  {
    this._handle = new HandleRef((object) this, handle);
    this.version = 1;
    this._count = LeptonicaApi.Native.pixaGetCount(this._handle);
  }

  public int Count
  {
    get
    {
      this.VerifyNotDisposed();
      return this._count;
    }
  }

  public bool Add(Pix pix, PixArrayAccessType copyflag = PixArrayAccessType.Clone)
  {
    Guard.RequireNotNull(nameof (pix), (object) pix);
    Guard.Require(nameof (copyflag), (copyflag == PixArrayAccessType.Clone ? 1 : (copyflag == PixArrayAccessType.Copy ? 1 : 0)) != 0, "Copy flag must be either copy or clone but was {0}.", (object) copyflag);
    int num = LeptonicaApi.Native.pixaAddPix(this._handle, pix.Handle, copyflag);
    if (num == 0)
      this._count = LeptonicaApi.Native.pixaGetCount(this._handle);
    return num == 0;
  }

  public void Remove(int index)
  {
    Guard.Require(nameof (index), (index < 0 ? 0 : (index < this.Count ? 1 : 0)) != 0, "The index {0} must be between 0 and {1}.", (object) index, (object) this.Count);
    this.VerifyNotDisposed();
    if (LeptonicaApi.Native.pixaRemovePix(this._handle, index) != 0)
      return;
    this._count = LeptonicaApi.Native.pixaGetCount(this._handle);
  }

  public void Clear()
  {
    this.VerifyNotDisposed();
    if (LeptonicaApi.Native.pixaClear(this._handle) != 0)
      return;
    this._count = LeptonicaApi.Native.pixaGetCount(this._handle);
  }

  public Pix GetPix(int index, PixArrayAccessType accessType = PixArrayAccessType.Clone)
  {
    Guard.Require(nameof (accessType), (accessType == PixArrayAccessType.Clone ? 1 : (accessType == PixArrayAccessType.Copy ? 1 : 0)) != 0, "Access type must be either copy or clone but was {0}.", (object) accessType);
    Guard.Require(nameof (index), (index < 0 ? 0 : (index < this.Count ? 1 : 0)) != 0, "The index {0} must be between 0 and {1}.", (object) index, (object) this.Count);
    this.VerifyNotDisposed();
    IntPtr pix = LeptonicaApi.Native.pixaGetPix(this._handle, index, accessType);
    return !(pix == IntPtr.Zero) ? Pix.Create(pix) : throw new InvalidOperationException($"Failed to retrieve pix {pix}.");
  }

  public IEnumerator<Pix> GetEnumerator()
  {
    return (IEnumerator<Pix>) new PixArray.PixArrayEnumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new PixArray.PixArrayEnumerator(this);

  protected override void Dispose(bool disposing)
  {
    IntPtr handle = this._handle.Handle;
    LeptonicaApi.Native.pixaDestroy(ref handle);
    this._handle = new HandleRef((object) this, handle);
  }

  private class PixArrayEnumerator : DisposableBase, IEnumerator<Pix>, IDisposable, IEnumerator
  {
    private readonly PixArray array;
    private readonly Pix[] items;
    private Pix current;
    private int index;
    private readonly int version;

    public PixArrayEnumerator(PixArray array)
    {
      this.array = array;
      this.version = array.version;
      this.items = new Pix[array.Count];
      this.index = 0;
      this.current = (Pix) null;
    }

    public bool MoveNext()
    {
      this.VerifyArrayUnchanged();
      this.VerifyNotDisposed();
      if (this.index < this.items.Length)
      {
        if (this.items[this.index] == null)
          this.items[this.index] = this.array.GetPix(this.index);
        this.current = this.items[this.index];
        ++this.index;
        return true;
      }
      this.index = this.items.Length + 1;
      this.current = (Pix) null;
      return false;
    }

    public Pix Current
    {
      get
      {
        this.VerifyArrayUnchanged();
        this.VerifyNotDisposed();
        return this.current;
      }
    }

    void IEnumerator.Reset()
    {
      this.VerifyArrayUnchanged();
      this.VerifyNotDisposed();
      this.index = 0;
      this.current = (Pix) null;
    }

    object IEnumerator.Current
    {
      get
      {
        if (this.index == 0 || this.index == this.items.Length + 1)
          throw new InvalidOperationException("The enumerator is positioned either before the first item or after the last item .");
        return (object) this.Current;
      }
    }

    private void VerifyArrayUnchanged()
    {
      if (this.version != this.array.version)
        throw new InvalidOperationException("PixArray was modified; enumeration operation may not execute.");
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      for (int index = 0; index < this.items.Length; ++index)
      {
        if (this.items[index] != null)
        {
          this.items[index].Dispose();
          this.items[index] = (Pix) null;
        }
      }
    }
  }
}
