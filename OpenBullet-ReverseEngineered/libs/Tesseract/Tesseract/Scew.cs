// Decompiled with JetBrains decompiler
// Type: Tesseract.Scew
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public struct Scew(float angle, float confidence)
{
  private float angle = angle;
  private float confidence = confidence;

  public float Angle => this.angle;

  public float Confidence => this.confidence;

  public override string ToString() => $"Scew: {this.Angle} [conf: {this.Confidence}]";

  public override bool Equals(object obj) => obj is Scew other && this.Equals(other);

  public bool Equals(Scew other)
  {
    return (double) this.confidence == (double) other.confidence && (double) this.angle == (double) other.angle;
  }

  public override int GetHashCode()
  {
    return 0 + 1000000007 * this.angle.GetHashCode() + 1000000009 * this.confidence.GetHashCode();
  }

  public static bool operator ==(Scew lhs, Scew rhs) => lhs.Equals(rhs);

  public static bool operator !=(Scew lhs, Scew rhs) => !(lhs == rhs);
}
