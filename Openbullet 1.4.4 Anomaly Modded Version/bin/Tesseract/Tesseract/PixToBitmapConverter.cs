// Decompiled with JetBrains decompiler
// Type: Tesseract.PixToBitmapConverter
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;

#nullable disable
namespace Tesseract;

public class PixToBitmapConverter
{
  public Bitmap Convert(Pix pix, bool includeAlpha = false)
  {
    PixelFormat pixelFormat = this.GetPixelFormat(pix);
    int depth = pix.Depth;
    Bitmap img = new Bitmap(pix.Width, pix.Height, pixelFormat);
    BitmapData bitmapData = (BitmapData) null;
    try
    {
      if ((pixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
        this.TransferPalette(pix, img);
      PixData data = pix.GetData();
      bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, pixelFormat);
      switch (depth)
      {
        case 1:
          this.TransferData1(data, bitmapData);
          break;
        case 8:
          this.TransferData8(data, bitmapData);
          break;
        case 16 /*0x10*/:
          this.TransferData16(data, bitmapData);
          break;
        case 32 /*0x20*/:
          this.TransferData32(data, bitmapData, includeAlpha ? 0 : (int) byte.MaxValue);
          break;
      }
      return img;
    }
    catch (Exception ex)
    {
      img.Dispose();
      throw;
    }
    finally
    {
      if (bitmapData != null)
        img.UnlockBits(bitmapData);
    }
  }

  private unsafe void TransferData32(PixData pixData, BitmapData imgData, int alphaMask)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      byte* numPtr1 = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      uint* numPtr2 = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      for (int index2 = 0; index2 < width; ++index2)
      {
        PixColor pixColor = PixColor.FromRgba(numPtr2[index2]);
        byte* numPtr3 = numPtr1 + (index2 << 2);
        *numPtr3 = pixColor.Blue;
        numPtr3[1] = pixColor.Green;
        numPtr3[2] = pixColor.Red;
        numPtr3[3] = (byte) ((uint) alphaMask | (uint) pixColor.Alpha);
      }
    }
  }

  private unsafe void TransferData16(PixData pixData, BitmapData imgData)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      ushort* numPtr = (ushort*) ((IntPtr) (void*) imgData.Scan0 + (IntPtr) (index1 * imgData.Stride) * 2);
      for (int index2 = 0; index2 < width; ++index2)
      {
        ushort dataTwoByte = (ushort) PixData.GetDataTwoByte(data, index2);
        numPtr[index2] = dataTwoByte;
      }
    }
  }

  private unsafe void TransferData8(PixData pixData, BitmapData imgData)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      byte* numPtr = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      for (int index2 = 0; index2 < width; ++index2)
      {
        byte dataByte = (byte) PixData.GetDataByte(data, index2);
        numPtr[index2] = dataByte;
      }
    }
  }

  private unsafe void TransferData1(PixData pixData, BitmapData imgData)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int num = imgData.Width / 8;
    for (int index1 = 0; index1 < height; ++index1)
    {
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      byte* numPtr = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      for (int index2 = 0; index2 < num; ++index2)
      {
        byte dataByte = (byte) PixData.GetDataByte(data, index2);
        numPtr[index2] = dataByte;
      }
    }
  }

  private void TransferPalette(Pix pix, Bitmap img)
  {
    ColorPalette palette = img.Palette;
    int length = palette.Entries.Length;
    int num1 = length - 1;
    PixColormap colormap = pix.Colormap;
    if (colormap != null && colormap.Count <= length)
    {
      int count = colormap.Count;
      for (int index = 0; index < count; ++index)
        palette.Entries[index] = (Color) colormap[index];
    }
    else
    {
      for (int index = 0; index < length; ++index)
      {
        byte num2 = (byte) (index * (int) byte.MaxValue / num1);
        palette.Entries[index] = Color.FromArgb((int) num2, (int) num2, (int) num2);
      }
    }
    img.Palette = palette;
  }

  private PixelFormat GetPixelFormat(Pix pix)
  {
    switch (pix.Depth)
    {
      case 1:
        return PixelFormat.Format1bppIndexed;
      case 8:
        return PixelFormat.Format8bppIndexed;
      case 16 /*0x10*/:
        return PixelFormat.Format16bppGrayScale;
      case 32 /*0x20*/:
        return PixelFormat.Format32bppArgb;
      default:
        throw new InvalidOperationException($"Pix depth {pix.Depth} is not supported.");
    }
  }
}
