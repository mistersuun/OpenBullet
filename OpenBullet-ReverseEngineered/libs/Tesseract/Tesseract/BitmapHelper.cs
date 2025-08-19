// Decompiled with JetBrains decompiler
// Type: Tesseract.BitmapHelper
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;

#nullable disable
namespace Tesseract;

public static class BitmapHelper
{
  public static int GetBPP(Bitmap bitmap)
  {
    switch (bitmap.PixelFormat)
    {
      case PixelFormat.Format16bppRgb555:
      case PixelFormat.Format16bppRgb565:
      case PixelFormat.Format16bppArgb1555:
      case PixelFormat.Format16bppGrayScale:
        return 16 /*0x10*/;
      case PixelFormat.Format24bppRgb:
        return 24;
      case PixelFormat.Format32bppRgb:
      case PixelFormat.Format32bppPArgb:
      case PixelFormat.Format32bppArgb:
        return 32 /*0x20*/;
      case PixelFormat.Format1bppIndexed:
        return 1;
      case PixelFormat.Format4bppIndexed:
        return 4;
      case PixelFormat.Format8bppIndexed:
        return 8;
      case PixelFormat.Format48bppRgb:
        return 48 /*0x30*/;
      case PixelFormat.Format64bppPArgb:
      case PixelFormat.Format64bppArgb:
        return 64 /*0x40*/;
      default:
        throw new ArgumentException($"The bitmap's pixel format of {bitmap.PixelFormat} was not recognised.", nameof (bitmap));
    }
  }

  public static unsafe byte GetDataBit(byte* data, int index)
  {
    return (byte) ((int) data[index >> 3] >> (index & 7) & 1);
  }

  public static unsafe void SetDataBit(byte* data, int index, byte value)
  {
    byte* numPtr = data + (index >> 3);
    *numPtr = (byte) ((uint) *numPtr & (uint) (byte) ~(128 /*0x80*/ >> (index & 7)));
    *numPtr = (byte) ((uint) *numPtr | (uint) (byte) (((int) value & 1) << 7 - (index & 7)));
  }

  public static unsafe byte GetDataQBit(byte* data, int index)
  {
    return (byte) ((int) data[index >> 1] >> 4 * (index & 1) & 15);
  }

  public static unsafe void SetDataQBit(byte* data, int index, byte value)
  {
    byte* numPtr = data + (index >> 1);
    *numPtr = (byte) ((uint) *numPtr & (uint) (byte) ~(240 /*0xF0*/ >> 4 * (index & 1)));
    *numPtr = (byte) ((uint) *numPtr | (uint) (byte) (((int) value & 15) << 4 - 4 * (index & 1)));
  }

  public static unsafe byte GetDataByte(byte* data, int index) => data[index];

  public static unsafe void SetDataByte(byte* data, int index, byte value) => data[index] = value;

  public static unsafe ushort GetDataUInt16(ushort* data, int index) => data[index];

  public static unsafe void SetDataUInt16(ushort* data, int index, ushort value)
  {
    data[index] = value;
  }

  public static unsafe uint GetDataUInt32(uint* data, int index) => data[index];

  public static unsafe void SetDataUInt32(uint* data, int index, uint value) => data[index] = value;

  public static uint ConvertRgb555ToRGBA(uint val)
  {
    uint num1 = (val & 31744U) >> 10;
    uint num2 = (val & 992U) >> 5;
    uint num3 = val & 31U /*0x1F*/;
    return (uint) (((int) num1 << 3 | (int) (num1 >> 2)) << 24 | ((int) num2 << 3 | (int) (num2 >> 2)) << 16 /*0x10*/ | ((int) num3 << 3 | (int) (num3 >> 2)) << 8 | (int) byte.MaxValue);
  }

  public static uint ConvertRgb565ToRGBA(uint val)
  {
    uint num1 = (val & 63488U) >> 11;
    uint num2 = (val & 2016U) >> 5;
    uint num3 = val & 31U /*0x1F*/;
    return (uint) (((int) num1 << 3 | (int) (num1 >> 2)) << 24 | ((int) num2 << 2 | (int) (num2 >> 4)) << 16 /*0x10*/ | ((int) num3 << 3 | (int) (num3 >> 2)) << 8 | (int) byte.MaxValue);
  }

  public static uint ConvertArgb1555ToRGBA(uint val)
  {
    uint num1 = (val & 32768U /*0x8000*/) >> 15;
    uint num2 = (val & 31744U) >> 10;
    uint num3 = (val & 992U) >> 5;
    uint num4 = val & 31U /*0x1F*/;
    return (uint) (((int) num2 << 3 | (int) (num2 >> 2)) << 24 | ((int) num3 << 3 | (int) (num3 >> 2)) << 16 /*0x10*/ | ((int) num4 << 3 | (int) (num4 >> 2)) << 8 | ((int) num1 << 8) - (int) num1);
  }

  public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
  {
    return (uint) ((int) red << 24 | (int) green << 16 /*0x10*/ | (int) blue << 8) | (uint) alpha;
  }
}
