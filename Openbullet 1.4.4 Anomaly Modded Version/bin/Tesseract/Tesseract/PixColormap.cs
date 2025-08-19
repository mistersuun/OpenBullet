// Decompiled with JetBrains decompiler
// Type: Tesseract.PixColormap
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class PixColormap : IDisposable
{
  private HandleRef handle;

  internal PixColormap(IntPtr handle) => this.handle = new HandleRef((object) this, handle);

  public static PixColormap Create(int depth)
  {
    IntPtr handle = depth == 1 || depth == 2 || depth == 4 || depth == 8 ? LeptonicaApi.Native.pixcmapCreate(depth) : throw new ArgumentOutOfRangeException(nameof (depth), "Depth must be 1, 2, 4, or 8 bpp.");
    return !(handle == IntPtr.Zero) ? new PixColormap(handle) : throw new InvalidOperationException("Failed to create colormap.");
  }

  public static PixColormap CreateLinear(int depth, int levels)
  {
    if (depth != 1 && depth != 2 && depth != 4 && depth != 8)
      throw new ArgumentOutOfRangeException(nameof (depth), "Depth must be 1, 2, 4, or 8 bpp.");
    if (levels < 2 || levels > 2 << depth)
      throw new ArgumentOutOfRangeException(nameof (levels), "Depth must be 2 and 2^depth (inclusive).");
    IntPtr linear = LeptonicaApi.Native.pixcmapCreateLinear(depth, levels);
    return !(linear == IntPtr.Zero) ? new PixColormap(linear) : throw new InvalidOperationException("Failed to create colormap.");
  }

  public static PixColormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite)
  {
    if (depth != 1 && depth != 2 && depth != 4 && depth != 8)
      throw new ArgumentOutOfRangeException(nameof (depth), "Depth must be 1, 2, 4, or 8 bpp.");
    IntPtr random = LeptonicaApi.Native.pixcmapCreateRandom(depth, firstIsBlack ? 1 : 0, lastIsWhite ? 1 : 0);
    return !(random == IntPtr.Zero) ? new PixColormap(random) : throw new InvalidOperationException("Failed to create colormap.");
  }

  internal HandleRef Handle => this.handle;

  public int Depth => LeptonicaApi.Native.pixcmapGetDepth(this.handle);

  public int Count => LeptonicaApi.Native.pixcmapGetCount(this.handle);

  public int FreeCount => LeptonicaApi.Native.pixcmapGetFreeCount(this.handle);

  public bool AddColor(PixColor color)
  {
    return LeptonicaApi.Native.pixcmapAddColor(this.handle, (int) color.Red, (int) color.Green, (int) color.Blue) == 0;
  }

  public bool AddNewColor(PixColor color, out int index)
  {
    return LeptonicaApi.Native.pixcmapAddNewColor(this.handle, (int) color.Red, (int) color.Green, (int) color.Blue, out index) == 0;
  }

  public bool AddNearestColor(PixColor color, out int index)
  {
    return LeptonicaApi.Native.pixcmapAddNearestColor(this.handle, (int) color.Red, (int) color.Green, (int) color.Blue, out index) == 0;
  }

  public bool AddBlackOrWhite(int color, out int index)
  {
    return LeptonicaApi.Native.pixcmapAddBlackOrWhite(this.handle, color, out index) == 0;
  }

  public bool SetBlackOrWhite(bool setBlack, bool setWhite)
  {
    return LeptonicaApi.Native.pixcmapSetBlackAndWhite(this.handle, setBlack ? 1 : 0, setWhite ? 1 : 0) == 0;
  }

  public bool IsUsableColor(PixColor color)
  {
    int usable;
    if (LeptonicaApi.Native.pixcmapUsableColor(this.handle, (int) color.Red, (int) color.Green, (int) color.Blue, out usable) == 0)
      return usable == 1;
    throw new InvalidOperationException("Failed to detect if color was usable or not.");
  }

  public void Clear()
  {
    if (LeptonicaApi.Native.pixcmapClear(this.handle) != 0)
      throw new InvalidOperationException("Failed to clear color map.");
  }

  public PixColor this[int index]
  {
    get
    {
      int color;
      if (LeptonicaApi.Native.pixcmapGetColor32(this.handle, index, out color) == 0)
        return PixColor.FromRgb((uint) color);
      throw new InvalidOperationException("Failed to retrieve color.");
    }
    set
    {
      if (LeptonicaApi.Native.pixcmapResetColor(this.handle, index, (int) value.Red, (int) value.Green, (int) value.Blue) != 0)
        throw new InvalidOperationException("Failed to reset color.");
    }
  }

  public void Dispose()
  {
    IntPtr handle = this.Handle.Handle;
    LeptonicaApi.Native.pixcmapDestroy(ref handle);
    this.handle = new HandleRef((object) this, IntPtr.Zero);
  }
}
