// Decompiled with JetBrains decompiler
// Type: Tesseract.PixConverter
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System.Drawing;

#nullable disable
namespace Tesseract;

public static class PixConverter
{
  private static readonly BitmapToPixConverter bitmapConverter = new BitmapToPixConverter();
  private static readonly PixToBitmapConverter pixConverter = new PixToBitmapConverter();

  public static Bitmap ToBitmap(Pix pix) => PixConverter.pixConverter.Convert(pix);

  public static Pix ToPix(Bitmap img) => PixConverter.bitmapConverter.Convert(img);
}
