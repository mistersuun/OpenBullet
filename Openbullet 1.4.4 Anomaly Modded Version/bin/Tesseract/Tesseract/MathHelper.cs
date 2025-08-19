// Decompiled with JetBrains decompiler
// Type: Tesseract.MathHelper
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace Tesseract;

public static class MathHelper
{
  public static float ToRadians(float angleInDegrees)
  {
    return (float) MathHelper.ToRadians((double) angleInDegrees);
  }

  public static double ToRadians(double angleInDegrees) => angleInDegrees * Math.PI / 180.0;

  public static int DivRoundUp(int dividend, int divisor)
  {
    int num = dividend / divisor;
    return dividend % divisor == 0 || divisor > 0 != dividend > 0 ? num : num + 1;
  }
}
