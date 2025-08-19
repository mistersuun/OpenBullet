// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.LongUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class LongUpDown : CommonNumericUpDown<long>
{
  static LongUpDown()
  {
    CommonNumericUpDown<long>.UpdateMetadata(typeof (LongUpDown), new long?(1L), new long?(long.MinValue), new long?(long.MaxValue));
  }

  public LongUpDown()
    : base(new CommonNumericUpDown<long>.FromText(long.TryParse), new CommonNumericUpDown<long>.FromDecimal(Decimal.ToInt64), (Func<long, long, bool>) ((v1, v2) => v1 < v2), (Func<long, long, bool>) ((v1, v2) => v1 > v2))
  {
  }

  protected override long IncrementValue(long value, long increment) => value + increment;

  protected override long DecrementValue(long value, long increment) => value - increment;
}
