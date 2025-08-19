// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ShortUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class ShortUpDown : CommonNumericUpDown<short>
{
  static ShortUpDown()
  {
    CommonNumericUpDown<short>.UpdateMetadata(typeof (ShortUpDown), new short?((short) 1), new short?(short.MinValue), new short?(short.MaxValue));
  }

  public ShortUpDown()
    : base(new CommonNumericUpDown<short>.FromText(short.TryParse), new CommonNumericUpDown<short>.FromDecimal(Decimal.ToInt16), (Func<short, short, bool>) ((v1, v2) => (int) v1 < (int) v2), (Func<short, short, bool>) ((v1, v2) => (int) v1 > (int) v2))
  {
  }

  protected override short IncrementValue(short value, short increment)
  {
    return (short) ((int) value + (int) increment);
  }

  protected override short DecrementValue(short value, short increment)
  {
    return (short) ((int) value - (int) increment);
  }
}
