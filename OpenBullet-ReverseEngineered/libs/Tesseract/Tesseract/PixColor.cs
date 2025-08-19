// Decompiled with JetBrains decompiler
// Type: Tesseract.PixColor
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#nullable disable
namespace Tesseract;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PixColor(byte red, byte green, byte blue, byte alpha = 255 /*0xFF*/) : 
  IEquatable<PixColor>
{
  private byte red = red;
  private byte blue = blue;
  private byte green = green;
  private byte alpha = alpha;

  public byte Red => this.red;

  public byte Green => this.green;

  public byte Blue => this.blue;

  public byte Alpha => this.alpha;

  public static PixColor FromRgba(uint value)
  {
    return new PixColor((byte) (value >> 24 & (uint) byte.MaxValue), (byte) (value >> 16 /*0x10*/ & (uint) byte.MaxValue), (byte) (value >> 8 & (uint) byte.MaxValue), (byte) (value & (uint) byte.MaxValue));
  }

  public static PixColor FromRgb(uint value)
  {
    return new PixColor((byte) (value >> 24 & (uint) byte.MaxValue), (byte) (value >> 16 /*0x10*/ & (uint) byte.MaxValue), (byte) (value >> 8 & (uint) byte.MaxValue));
  }

  public uint ToRGBA()
  {
    return (uint) ((int) this.red << 24 | (int) this.green << 16 /*0x10*/ | (int) this.blue << 8) | (uint) this.alpha;
  }

  public static explicit operator Color(PixColor color)
  {
    return Color.FromArgb((int) color.alpha, (int) color.red, (int) color.green, (int) color.blue);
  }

  public static explicit operator PixColor(Color color)
  {
    return new PixColor(color.R, color.G, color.B, color.A);
  }

  public override bool Equals(object obj) => obj is PixColor other && this.Equals(other);

  public bool Equals(PixColor other)
  {
    return (int) this.red == (int) other.red && (int) this.blue == (int) other.blue && (int) this.green == (int) other.green && (int) this.alpha == (int) other.alpha;
  }

  public override int GetHashCode()
  {
    return 0 + 1000000007 * this.red.GetHashCode() + 1000000009 * this.blue.GetHashCode() + 1000000021 * this.green.GetHashCode() + 1000000033 * this.alpha.GetHashCode();
  }

  public static bool operator ==(PixColor lhs, PixColor rhs) => lhs.Equals(rhs);

  public static bool operator !=(PixColor lhs, PixColor rhs) => !(lhs == rhs);

  public override string ToString() => $"Color(0x{this.ToRGBA():X})";
}
