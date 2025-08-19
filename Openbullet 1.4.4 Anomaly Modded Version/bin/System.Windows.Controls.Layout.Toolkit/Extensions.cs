// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Extensions
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls;

internal static class Extensions
{
  public static bool Invert(this Matrix m, out Matrix outputMatrix)
  {
    double num = m.M11 * m.M22 - m.M12 * m.M21;
    if (num == 0.0)
    {
      outputMatrix = m;
      return false;
    }
    Matrix matrix = m;
    m.M11 = matrix.M22 / num;
    m.M12 = -1.0 * matrix.M12 / num;
    m.M21 = -1.0 * matrix.M21 / num;
    m.M22 = matrix.M11 / num;
    m.OffsetX = (matrix.OffsetY * matrix.M21 - matrix.OffsetX * matrix.M22) / num;
    m.OffsetY = (matrix.OffsetX * matrix.M12 - matrix.OffsetY * matrix.M11) / num;
    outputMatrix = m;
    return true;
  }

  public static bool Contains(this string s, string value, StringComparison comparison)
  {
    return s.IndexOf(value, comparison) >= 0;
  }
}
