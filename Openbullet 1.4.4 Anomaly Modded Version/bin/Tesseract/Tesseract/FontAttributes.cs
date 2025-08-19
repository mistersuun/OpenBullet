// Decompiled with JetBrains decompiler
// Type: Tesseract.FontAttributes
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public class FontAttributes
{
  public FontInfo FontInfo { get; private set; }

  public bool IsUnderlined { get; private set; }

  public bool IsSmallCaps { get; private set; }

  public int PointSize { get; private set; }

  public FontAttributes(FontInfo fontInfo, bool isUnderlined, bool isSmallCaps, int pointSize)
  {
    this.FontInfo = fontInfo;
    this.IsUnderlined = isUnderlined;
    this.IsSmallCaps = isSmallCaps;
    this.PointSize = pointSize;
  }
}
