// Decompiled with JetBrains decompiler
// Type: Tesseract.Pix
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract.Internal;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class Pix : DisposableBase, IEquatable<Pix>
{
  public const float Deg2Rad = 0.0174532924f;
  public const int DefaultBinarySearchReduction = 2;
  public const int DefaultBinaryThreshold = 130;
  private const float VerySmallAngle = 0.001f;
  private static readonly List<int> AllowedDepths = new List<int>()
  {
    1,
    2,
    4,
    8,
    16 /*0x10*/,
    32 /*0x20*/
  };
  private static readonly Dictionary<string, ImageFormat> imageFomatLookup = new Dictionary<string, ImageFormat>()
  {
    {
      ".jpg",
      ImageFormat.JfifJpeg
    },
    {
      ".jpeg",
      ImageFormat.JfifJpeg
    },
    {
      ".gif",
      ImageFormat.Gif
    },
    {
      ".tif",
      ImageFormat.Tiff
    },
    {
      ".tiff",
      ImageFormat.Tiff
    },
    {
      ".png",
      ImageFormat.Png
    },
    {
      ".bmp",
      ImageFormat.Bmp
    }
  };
  private readonly int depth;
  private readonly int height;
  private readonly int width;
  private PixColormap colormap;
  private HandleRef handle;

  private Pix(IntPtr handle)
  {
    this.handle = !(handle == IntPtr.Zero) ? new HandleRef((object) this, handle) : throw new ArgumentNullException(nameof (handle));
    this.width = LeptonicaApi.Native.pixGetWidth(this.handle);
    this.height = LeptonicaApi.Native.pixGetHeight(this.handle);
    this.depth = LeptonicaApi.Native.pixGetDepth(this.handle);
    IntPtr colormap = LeptonicaApi.Native.pixGetColormap(this.handle);
    if (!(colormap != IntPtr.Zero))
      return;
    this.colormap = new PixColormap(colormap);
  }

  public static Pix Create(int width, int height, int depth)
  {
    if (!Pix.AllowedDepths.Contains(depth))
      throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", nameof (depth));
    if (width <= 0)
      throw new ArgumentException("Width must be greater than zero", nameof (width));
    if (height <= 0)
      throw new ArgumentException("Height must be greater than zero", nameof (height));
    IntPtr handle = LeptonicaApi.Native.pixCreate(width, height, depth);
    return !(handle == IntPtr.Zero) ? Pix.Create(handle) : throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");
  }

  public static Pix Create(IntPtr handle)
  {
    return !(handle == IntPtr.Zero) ? new Pix(handle) : throw new ArgumentException("Pix handle must not be zero (null).", nameof (handle));
  }

  public static Pix LoadFromFile(string filename)
  {
    IntPtr handle = LeptonicaApi.Native.pixRead(filename);
    return !(handle == IntPtr.Zero) ? Pix.Create(handle) : throw new IOException($"Failed to load image '{filename}'.");
  }

  public static unsafe Pix LoadTiffFromMemory(byte[] bytes)
  {
    IntPtr handle;
    fixed (byte* data = bytes)
      handle = LeptonicaApi.Native.pixReadMemTiff(data, bytes.Length, 0);
    return !(handle == IntPtr.Zero) ? Pix.Create(handle) : throw new IOException("Failed to load image from memory.");
  }

  public PixColormap Colormap
  {
    get => this.colormap;
    set
    {
      if (value != null)
      {
        if (LeptonicaApi.Native.pixSetColormap(this.handle, value.Handle) != 0)
          return;
        this.colormap = value;
      }
      else
      {
        if (LeptonicaApi.Native.pixDestroyColormap(this.handle) != 0)
          return;
        this.colormap = (PixColormap) null;
      }
    }
  }

  public int Depth => this.depth;

  public int Height => this.height;

  public int Width => this.width;

  public int XRes
  {
    get => LeptonicaApi.Native.pixGetXRes(this.handle);
    set => LeptonicaApi.Native.pixSetXRes(this.handle, value);
  }

  public int YRes
  {
    get => LeptonicaApi.Native.pixGetYRes(this.handle);
    set => LeptonicaApi.Native.pixSetYRes(this.handle, value);
  }

  internal HandleRef Handle => this.handle;

  public PixData GetData() => new PixData(this);

  public override bool Equals(object obj)
  {
    return obj != null && !(this.GetType() != obj.GetType()) && this.Equals((Pix) obj);
  }

  public bool Equals(Pix other)
  {
    if (other == null)
      return false;
    int same;
    if (LeptonicaApi.Native.pixEqual(this.Handle, other.Handle, out same) != 0)
      throw new TesseractException("Failed to compare pix");
    return same != 0;
  }

  public void Save(string filename, ImageFormat? format = null)
  {
    ImageFormat format1;
    if (!format.HasValue)
    {
      string lowerInvariant = Path.GetExtension(filename).ToLowerInvariant();
      if (!Pix.imageFomatLookup.TryGetValue(lowerInvariant, out format1))
        format1 = ImageFormat.Default;
    }
    else
      format1 = format.Value;
    if (LeptonicaApi.Native.pixWrite(filename, this.handle, format1) != 0)
      throw new IOException($"Failed to save image '{filename}'.");
  }

  public Pix Clone() => new Pix(LeptonicaApi.Native.pixClone(this.handle));

  public Pix BinarizeOtsuAdaptiveThreshold(
    int sx,
    int sy,
    int smoothx,
    int smoothy,
    float scorefract)
  {
    Guard.Verify(this.Depth == 8, "Image must have a depth of 8 bits per pixel to be binerized using Otsu.");
    Guard.Require(nameof (sx), sx >= 16 /*0x10*/, "The sx parameter must be greater than or equal to 16");
    Guard.Require(nameof (sy), sy >= 16 /*0x10*/, "The sy parameter must be greater than or equal to 16");
    IntPtr ppixth;
    IntPtr ppixd;
    int num = LeptonicaApi.Native.pixOtsuAdaptiveThreshold(this.handle, sx, sy, smoothx, smoothy, scorefract, out ppixth, out ppixd);
    if (ppixth != IntPtr.Zero)
      LeptonicaApi.Native.pixDestroy(ref ppixth);
    if (num == 1)
      throw new TesseractException("Failed to binarize image.");
    return new Pix(ppixd);
  }

  public Pix BinarizeSauvola(int whsize, float factor, bool addborder)
  {
    Guard.Verify(this.Depth == 8, "Source image must be 8bpp");
    Guard.Verify(this.Colormap == null, "Source image must not be color mapped.");
    Guard.Require(nameof (whsize), whsize >= 2, "The window half-width (whsize) must be greater than 2.");
    int num1 = Math.Min((this.Width - 3) / 2, (this.Height - 3) / 2);
    Guard.Require(nameof (whsize), (whsize < num1 ? 1 : 0) != 0, "The window half-width (whsize) must be less than {0} for this image.", (object) num1);
    Guard.Require(nameof (factor), (double) factor >= 0.0, "Factor must be greater than zero (0).");
    IntPtr ppixm;
    IntPtr ppixsd;
    IntPtr ppixth;
    IntPtr ppixd;
    int num2 = LeptonicaApi.Native.pixSauvolaBinarize(this.handle, whsize, factor, addborder ? 1 : 0, out ppixm, out ppixsd, out ppixth, out ppixd);
    if (ppixm != IntPtr.Zero)
      LeptonicaApi.Native.pixDestroy(ref ppixm);
    if (ppixsd != IntPtr.Zero)
      LeptonicaApi.Native.pixDestroy(ref ppixsd);
    if (ppixth != IntPtr.Zero)
      LeptonicaApi.Native.pixDestroy(ref ppixth);
    if (num2 == 1)
      throw new TesseractException("Failed to binarize image.");
    return new Pix(ppixd);
  }

  public Pix BinarizeSauvolaTiled(int whsize, float factor, int nx, int ny)
  {
    Guard.Verify(this.Depth == 8, "Source image must be 8bpp");
    Guard.Verify(this.Colormap == null, "Source image must not be color mapped.");
    Guard.Require(nameof (whsize), whsize >= 2, "The window half-width (whsize) must be greater than 2.");
    int num1 = Math.Min((this.Width - 3) / 2, (this.Height - 3) / 2);
    Guard.Require(nameof (whsize), (whsize < num1 ? 1 : 0) != 0, "The window half-width (whsize) must be less than {0} for this image.", (object) num1);
    Guard.Require(nameof (factor), (double) factor >= 0.0, "Factor must be greater than zero (0).");
    IntPtr ppixth;
    IntPtr ppixd;
    int num2 = LeptonicaApi.Native.pixSauvolaBinarizeTiled(this.handle, whsize, factor, nx, ny, out ppixth, out ppixd);
    if (ppixth != IntPtr.Zero)
      LeptonicaApi.Native.pixDestroy(ref ppixth);
    if (num2 == 1)
      throw new TesseractException("Failed to binarize image.");
    return new Pix(ppixd);
  }

  public Pix ConvertRGBToGray(float rwt, float gwt, float bwt)
  {
    Guard.Verify(this.Depth == 32 /*0x20*/, "The source image must have a depth of 32 (32 bpp).");
    Guard.Require(nameof (rwt), (double) rwt >= 0.0, "All weights must be greater than or equal to zero; red was not.");
    Guard.Require(nameof (gwt), (double) gwt >= 0.0, "All weights must be greater than or equal to zero; green was not.");
    Guard.Require(nameof (bwt), (double) bwt >= 0.0, "All weights must be greater than or equal to zero; blue was not.");
    IntPtr gray = LeptonicaApi.Native.pixConvertRGBToGray(this.handle, rwt, gwt, bwt);
    return !(gray == IntPtr.Zero) ? new Pix(gray) : throw new TesseractException("Failed to convert to grayscale.");
  }

  public Pix ConvertRGBToGray() => this.ConvertRGBToGray(0.0f, 0.0f, 0.0f);

  public Pix RemoveLines()
  {
    IntPtr zero;
    IntPtr pix1 = zero = IntPtr.Zero;
    IntPtr pix2 = zero;
    IntPtr pix3 = zero;
    IntPtr pix4 = zero;
    IntPtr pix5 = zero;
    IntPtr pix6 = zero;
    IntPtr pix7 = zero;
    IntPtr pix8 = zero;
    try
    {
      pix8 = LeptonicaApi.Native.pixThresholdToBinary(this.handle, 170);
      float pangle;
      LeptonicaApi.Native.pixFindSkew(new HandleRef((object) this, pix8), out pangle, out float _);
      pix7 = LeptonicaApi.Native.pixRotateAMGray(this.handle, (float) Math.PI / 180f * pangle, byte.MaxValue);
      pix6 = LeptonicaApi.Native.pixCloseGray(new HandleRef((object) this, pix7), 51, 1);
      pix5 = LeptonicaApi.Native.pixErodeGray(new HandleRef((object) this, pix6), 1, 5);
      pix4 = LeptonicaApi.Native.pixThresholdToValue(new HandleRef((object) this, IntPtr.Zero), new HandleRef((object) this, pix5), 210, (int) byte.MaxValue);
      pix3 = LeptonicaApi.Native.pixThresholdToValue(new HandleRef((object) this, IntPtr.Zero), new HandleRef((object) this, pix4), 200, 0);
      pix2 = LeptonicaApi.Native.pixThresholdToBinary(new HandleRef((object) this, pix3), 210);
      LeptonicaApi.Native.pixInvert(new HandleRef((object) this, pix3), new HandleRef((object) this, pix3));
      IntPtr handle = LeptonicaApi.Native.pixAddGray(new HandleRef((object) this, IntPtr.Zero), new HandleRef((object) this, pix7), new HandleRef((object) this, pix3));
      pix1 = LeptonicaApi.Native.pixOpenGray(new HandleRef((object) this, handle), 1, 9);
      LeptonicaApi.Native.pixCombineMasked(new HandleRef((object) this, handle), new HandleRef((object) this, pix1), new HandleRef((object) this, pix2));
      return !(handle == IntPtr.Zero) ? new Pix(handle) : throw new TesseractException("Failed to remove lines from image.");
    }
    finally
    {
      if (pix8 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix8);
      if (pix7 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix7);
      if (pix6 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix6);
      if (pix5 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix5);
      if (pix4 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix4);
      if (pix3 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix3);
      if (pix2 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix2);
      if (pix1 != IntPtr.Zero)
        LeptonicaApi.Native.pixDestroy(ref pix1);
    }
  }

  public Pix Deskew() => this.Deskew(2, out Scew _);

  public Pix Deskew(out Scew scew) => this.Deskew(2, out scew);

  public Pix Deskew(int redSearch, out Scew scew)
  {
    return this.Deskew(ScewSweep.Default, redSearch, 130, out scew);
  }

  public Pix Deskew(ScewSweep sweep, int redSearch, int thresh, out Scew scew)
  {
    float pAngle;
    float pConf;
    IntPtr handle = LeptonicaApi.Native.pixDeskewGeneral(this.handle, sweep.Reduction, sweep.Range, sweep.Delta, redSearch, thresh, out pAngle, out pConf);
    if (handle == IntPtr.Zero)
      throw new TesseractException("Failed to deskew image.");
    scew = new Scew(pAngle, pConf);
    return new Pix(handle);
  }

  public Pix Rotate(
    float angleInRadians,
    RotationMethod method = RotationMethod.AreaMap,
    RotationFill fillColor = RotationFill.White,
    int? width = null,
    int? height = null)
  {
    if (!width.HasValue)
      width = new int?(this.Width);
    if (!height.HasValue)
      height = new int?(this.Height);
    if ((double) Math.Abs(angleInRadians) < 1.0 / 1000.0)
      return this.Clone();
    double num = 2.0 * (double) angleInRadians / Math.PI;
    IntPtr handle = Math.Abs(num - Math.Floor(num)) >= 1.0 / 1000.0 ? LeptonicaApi.Native.pixRotate(this.handle, angleInRadians, method, fillColor, width.Value, height.Value) : LeptonicaApi.Native.pixRotateOrth(this.handle, (int) num);
    return !(handle == IntPtr.Zero) ? new Pix(handle) : throw new LeptonicaException("Failed to rotate image around its centre.");
  }

  public Pix Rotate90(int direction)
  {
    IntPtr handle = LeptonicaApi.Native.pixRotate90(this.handle, direction);
    return !(handle == IntPtr.Zero) ? new Pix(handle) : throw new LeptonicaException("Failed to rotate image.");
  }

  public Pix Scale(float scaleX, float scaleY)
  {
    IntPtr handle = LeptonicaApi.Native.pixScale(this.handle, scaleX, scaleY);
    return !(handle == IntPtr.Zero) ? new Pix(handle) : throw new InvalidOperationException("Failed to scale pix.");
  }

  protected override void Dispose(bool disposing)
  {
    IntPtr handle = this.handle.Handle;
    LeptonicaApi.Native.pixDestroy(ref handle);
    this.handle = new HandleRef((object) this, IntPtr.Zero);
  }
}
