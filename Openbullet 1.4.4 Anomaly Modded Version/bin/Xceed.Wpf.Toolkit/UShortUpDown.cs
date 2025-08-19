// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.UShortUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class UShortUpDown : CommonNumericUpDown<ushort>
{
  static UShortUpDown()
  {
    CommonNumericUpDown<ushort>.UpdateMetadata(typeof (UShortUpDown), new ushort?((ushort) 1), new ushort?((ushort) 0), new ushort?(ushort.MaxValue));
  }

  public UShortUpDown()
    : base(new CommonNumericUpDown<ushort>.FromText(ushort.TryParse), new CommonNumericUpDown<ushort>.FromDecimal(Decimal.ToUInt16), (Func<ushort, ushort, bool>) ((v1, v2) => (int) v1 < (int) v2), (Func<ushort, ushort, bool>) ((v1, v2) => (int) v1 > (int) v2))
  {
  }

  protected override ushort IncrementValue(ushort value, ushort increment)
  {
    return (ushort) ((uint) value + (uint) increment);
  }

  protected override ushort DecrementValue(ushort value, ushort increment)
  {
    return (ushort) ((uint) value - (uint) increment);
  }
}
