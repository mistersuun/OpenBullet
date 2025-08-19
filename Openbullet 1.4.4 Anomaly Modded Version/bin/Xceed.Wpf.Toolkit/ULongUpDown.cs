// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ULongUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class ULongUpDown : CommonNumericUpDown<ulong>
{
  static ULongUpDown()
  {
    CommonNumericUpDown<ulong>.UpdateMetadata(typeof (ULongUpDown), new ulong?(1UL), new ulong?(0UL), new ulong?(ulong.MaxValue));
  }

  public ULongUpDown()
    : base(new CommonNumericUpDown<ulong>.FromText(ulong.TryParse), new CommonNumericUpDown<ulong>.FromDecimal(Decimal.ToUInt64), (Func<ulong, ulong, bool>) ((v1, v2) => v1 < v2), (Func<ulong, ulong, bool>) ((v1, v2) => v1 > v2))
  {
  }

  protected override ulong IncrementValue(ulong value, ulong increment) => value + increment;

  protected override ulong DecrementValue(ulong value, ulong increment) => value - increment;
}
