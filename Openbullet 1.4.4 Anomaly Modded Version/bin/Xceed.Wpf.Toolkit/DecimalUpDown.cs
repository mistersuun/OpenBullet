// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DecimalUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class DecimalUpDown : CommonNumericUpDown<Decimal>
{
  static DecimalUpDown()
  {
    CommonNumericUpDown<Decimal>.UpdateMetadata(typeof (DecimalUpDown), new Decimal?(1M), new Decimal?(Decimal.MinValue), new Decimal?(Decimal.MaxValue));
  }

  public DecimalUpDown()
    : base(new CommonNumericUpDown<Decimal>.FromText(Decimal.TryParse), (CommonNumericUpDown<Decimal>.FromDecimal) (d => d), (Func<Decimal, Decimal, bool>) ((v1, v2) => v1 < v2), (Func<Decimal, Decimal, bool>) ((v1, v2) => v1 > v2))
  {
  }

  protected override Decimal IncrementValue(Decimal value, Decimal increment) => value + increment;

  protected override Decimal DecrementValue(Decimal value, Decimal increment) => value - increment;
}
