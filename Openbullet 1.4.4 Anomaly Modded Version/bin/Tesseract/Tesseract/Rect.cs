// Decompiled with JetBrains decompiler
// Type: Tesseract.Rect
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace Tesseract;

public struct Rect(int x, int y, int width, int height) : IEquatable<Rect>
{
  public static readonly Rect Empty;
  private int x = x;
  private int y = y;
  private int width = width;
  private int height = height;

  public static Rect FromCoords(int x1, int y1, int x2, int y2)
  {
    return new Rect(x1, y1, x2 - x1, y2 - y1);
  }

  public int X1 => this.x;

  public int Y1 => this.y;

  public int X2 => this.x + this.width;

  public int Y2 => this.y + this.height;

  public int Width => this.width;

  public int Height => this.height;

  public override bool Equals(object obj) => obj is Rect other && this.Equals(other);

  public bool Equals(Rect other)
  {
    return this.x == other.x && this.y == other.y && this.width == other.width && this.height == other.height;
  }

  public override int GetHashCode()
  {
    return 0 + 1000000007 * this.x.GetHashCode() + 1000000009 * this.y.GetHashCode() + 1000000021 * this.width.GetHashCode() + 1000000033 * this.height.GetHashCode();
  }

  public static bool operator ==(Rect lhs, Rect rhs) => lhs.Equals(rhs);

  public static bool operator !=(Rect lhs, Rect rhs) => !(lhs == rhs);

  public override string ToString()
  {
    return $"[Rect X={this.x}, Y={this.y}, Width={this.width}, Height={this.height}]";
  }
}
