// Decompiled with JetBrains decompiler
// Type: Tesseract.BitmapToPixConverter
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;

#nullable disable
namespace Tesseract;

public class BitmapToPixConverter
{
  public Pix Convert(Bitmap img)
  {
    int pixDepth = this.GetPixDepth(img.PixelFormat);
    Pix pix = Pix.Create(img.Width, img.Height, pixDepth);
    pix.XRes = (int) Math.Round((double) img.HorizontalResolution);
    pix.YRes = (int) Math.Round((double) img.VerticalResolution);
    BitmapData bitmapData = (BitmapData) null;
    try
    {
      if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
        this.CopyColormap(img, pix);
      bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
      PixData data = pix.GetData();
      if (bitmapData.PixelFormat == PixelFormat.Format32bppArgb)
        this.TransferDataFormat32bppArgb(bitmapData, data);
      else if (bitmapData.PixelFormat == PixelFormat.Format24bppRgb)
        this.TransferDataFormat24bppRgb(bitmapData, data);
      else if (bitmapData.PixelFormat == PixelFormat.Format8bppIndexed)
        this.TransferDataFormat8bppIndexed(bitmapData, data);
      else if (bitmapData.PixelFormat == PixelFormat.Format1bppIndexed)
        this.TransferDataFormat1bppIndexed(bitmapData, data);
      return pix;
    }
    catch (Exception ex)
    {
      pix.Dispose();
      throw;
    }
    finally
    {
      if (bitmapData != null)
        img.UnlockBits(bitmapData);
    }
  }

  private void CopyColormap(Bitmap img, Pix pix)
  {
    Color[] entries = img.Palette.Entries;
    PixColormap pixColormap = PixColormap.Create(pix.Depth);
    try
    {
      for (int index = 0; index < entries.Length; ++index)
      {
        if (!pixColormap.AddColor((PixColor) entries[index]))
          throw new InvalidOperationException($"Failed to add colormap entry {index}.");
      }
      pix.Colormap = pixColormap;
    }
    catch (Exception ex)
    {
      pixColormap.Dispose();
      throw;
    }
  }

  private int GetPixDepth(PixelFormat pixelFormat)
  {
    switch (pixelFormat)
    {
      case PixelFormat.Format24bppRgb:
      case PixelFormat.Format32bppArgb:
        return 32 /*0x20*/;
      case PixelFormat.Format1bppIndexed:
        return 1;
      case PixelFormat.Format8bppIndexed:
        return 8;
      default:
        throw new InvalidOperationException($"Source bitmap's pixel format {pixelFormat} is not supported.");
    }
  }

  private unsafe void TransferDataFormat1bppIndexed(BitmapData imgData, PixData pixData)
  {
    int height = imgData.Height;
    int num = imgData.Width / 8;
    for (int index1 = 0; index1 < height; ++index1)
    {
      byte* data1 = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      uint* data2 = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      for (int index2 = 0; index2 < num; ++index2)
      {
        byte dataByte = BitmapHelper.GetDataByte(data1, index2);
        PixData.SetDataByte(data2, index2, (uint) dataByte);
      }
    }
  }

  private unsafe void TransferDataFormat24bppRgb(BitmapData imgData, PixData pixData)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      byte* numPtr1 = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      for (int index2 = 0; index2 < width; ++index2)
      {
        byte* numPtr2 = numPtr1 + index2 * 3;
        byte blue = *numPtr2;
        byte green = numPtr2[1];
        byte red = numPtr2[2];
        PixData.SetDataFourByte(data, index2, BitmapHelper.EncodeAsRGBA(red, green, blue, byte.MaxValue));
      }
    }
  }

  private unsafe void TransferDataFormat32bppArgb(BitmapData imgData, PixData pixData)
  {
    int pixelFormat = (int) imgData.PixelFormat;
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      byte* numPtr1 = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      for (int index2 = 0; index2 < width; ++index2)
      {
        byte* numPtr2 = numPtr1 + (index2 << 2);
        byte blue = *numPtr2;
        byte green = numPtr2[1];
        byte red = numPtr2[2];
        byte alpha = numPtr2[3];
        PixData.SetDataFourByte(data, index2, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
      }
    }
  }

  private unsafe void TransferDataFormat8bppIndexed(BitmapData imgData, PixData pixData)
  {
    int height = imgData.Height;
    int width = imgData.Width;
    for (int index1 = 0; index1 < height; ++index1)
    {
      byte* numPtr = (byte*) ((IntPtr) (void*) imgData.Scan0 + index1 * imgData.Stride);
      uint* data = (uint*) ((IntPtr) (void*) pixData.Data + (IntPtr) (index1 * pixData.WordsPerLine) * 4);
      for (int index2 = 0; index2 < width; ++index2)
      {
        byte num = numPtr[index2];
        PixData.SetDataByte(data, index2, (uint) num);
      }
    }
  }
}
