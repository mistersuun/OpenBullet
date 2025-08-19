// Decompiled with JetBrains decompiler
// Type: Tesseract.ElementProperties
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public struct ElementProperties(
  Orientation orientation,
  TextLineOrder textLineOrder,
  WritingDirection writingDirection,
  float deskewAngle)
{
  private Orientation orientation = orientation;
  private TextLineOrder textLineOrder = textLineOrder;
  private WritingDirection writingDirection = writingDirection;
  private float deskewAngle = deskewAngle;

  public Orientation Orientation => this.orientation;

  public TextLineOrder TextLineOrder => this.textLineOrder;

  public WritingDirection WritingDirection => this.writingDirection;

  public float DeskewAngle => this.deskewAngle;
}
