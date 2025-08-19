// Decompiled with JetBrains decompiler
// Type: Tesseract.PixData
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public class PixData
{
  public Pix Pix { get; private set; }

  internal PixData(Pix pix)
  {
    this.Pix = pix;
    this.Data = LeptonicaApi.Native.pixGetData(this.Pix.Handle);
    this.WordsPerLine = LeptonicaApi.Native.pixGetWpl(this.Pix.Handle);
  }

  public IntPtr Data { get; private set; }

  public int WordsPerLine { get; private set; }

  public void EndianByteSwap() => LeptonicaApi.Native.pixEndianByteSwap(this.Pix.Handle);

  public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
  {
    return (uint) ((int) red << 24 | (int) green << 16 /*0x10*/ | (int) blue << 8) | (uint) alpha;
  }

  public static unsafe uint GetDataBit(uint* data, int index)
  {
    return data[index >> 5] >> 31 /*0x1F*/ - (index & 31 /*0x1F*/) & 1U;
  }

  public static unsafe void SetDataBit(uint* data, int index, uint value)
  {
    uint* numPtr = data + (index >> 5);
    *numPtr = *numPtr & ~(2147483648U /*0x80000000*/ >> index);
    *numPtr = *numPtr | value << 31 /*0x1F*/ - (index & 31 /*0x1F*/);
  }

  public static unsafe uint GetDataDIBit(uint* data, int index)
  {
    return data[index >> 4] >> 2 * (15 - (index & 15)) & 3U;
  }

  public static unsafe void SetDataDIBit(uint* data, int index, uint value)
  {
    uint* numPtr = data + (index >> 4);
    *numPtr = *numPtr & ~(3221225472U /*0xC0000000*/ >> 2 * (index & 15));
    *numPtr = *numPtr | (uint) (((int) value & 3) << 30 - 2 * (index & 15));
  }

  public static unsafe uint GetDataQBit(uint* data, int index)
  {
    return data[index >> 3] >> 4 * (7 - (index & 7)) & 15U;
  }

  public static unsafe void SetDataQBit(uint* data, int index, uint value)
  {
    uint* numPtr = data + (index >> 3);
    *numPtr = *numPtr & ~(4026531840U /*0xF0000000*/ >> 4 * (index & 7));
    *numPtr = *numPtr | (uint) (((int) value & 15) << 28 - 4 * (index & 7));
  }

  public static unsafe uint GetDataByte(uint* data, int index)
  {
    return IntPtr.Size == 8 ? (uint) *(byte*) ((ulong) ((UIntPtr) data + (UIntPtr) index) ^ 3UL) : (uint) *(byte*) ((uint) ((UIntPtr) data + (UIntPtr) index) ^ 3U);
  }

  public static unsafe void SetDataByte(uint* data, int index, uint value)
  {
    if (IntPtr.Size == 8)
      *(sbyte*) ((ulong) ((UIntPtr) data + (UIntPtr) index) ^ 3UL) = (sbyte) (byte) value;
    else
      *(sbyte*) ((uint) ((UIntPtr) data + (UIntPtr) index) ^ 3U) = (sbyte) (byte) value;
  }

  public static unsafe uint GetDataTwoByte(uint* data, int index)
  {
    return IntPtr.Size == 8 ? (uint) *(ushort*) ((ulong) ((UIntPtr) data + (UIntPtr) index * new UIntPtr(2)) ^ 2UL) : (uint) *(ushort*) ((uint) ((UIntPtr) data + (UIntPtr) index * new UIntPtr(2)) ^ 2U);
  }

  public static unsafe void SetDataTwoByte(uint* data, int index, uint value)
  {
    if (IntPtr.Size == 8)
      *(short*) ((ulong) ((UIntPtr) data + (UIntPtr) index * new UIntPtr(2)) ^ 2UL) = (short) (ushort) value;
    else
      *(short*) ((uint) ((UIntPtr) data + (UIntPtr) index * new UIntPtr(2)) ^ 2U) = (short) (ushort) value;
  }

  public static unsafe uint GetDataFourByte(uint* data, int index) => data[index];

  public static unsafe void SetDataFourByte(uint* data, int index, uint value)
  {
    data[index] = value;
  }
}
