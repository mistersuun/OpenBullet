// Decompiled with JetBrains decompiler
// Type: Tesseract.ScewSweep
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public struct ScewSweep(int reduction = 4, float range = 7f, float delta = 1f)
{
  public static ScewSweep Default = new ScewSweep();
  public const int DefaultReduction = 4;
  public const float DefaultRange = 7f;
  public const float DefaultDelta = 1f;
  private int reduction = reduction;
  private float range = range;
  private float delta = delta;

  public int Reduction => this.reduction;

  public float Range => this.range;

  public float Delta => this.delta;
}
