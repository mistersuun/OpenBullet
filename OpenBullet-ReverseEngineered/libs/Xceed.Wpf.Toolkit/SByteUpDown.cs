// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.SByteUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class SByteUpDown : CommonNumericUpDown<sbyte>
{
  static SByteUpDown()
  {
    CommonNumericUpDown<sbyte>.UpdateMetadata(typeof (SByteUpDown), new sbyte?((sbyte) 1), new sbyte?(sbyte.MinValue), new sbyte?(sbyte.MaxValue));
  }

  public SByteUpDown()
    : base(new CommonNumericUpDown<sbyte>.FromText(sbyte.TryParse), new CommonNumericUpDown<sbyte>.FromDecimal(Decimal.ToSByte), (Func<sbyte, sbyte, bool>) ((v1, v2) => (int) v1 < (int) v2), (Func<sbyte, sbyte, bool>) ((v1, v2) => (int) v1 > (int) v2))
  {
  }

  protected override sbyte IncrementValue(sbyte value, sbyte increment)
  {
    return (sbyte) ((int) value + (int) increment);
  }

  protected override sbyte DecrementValue(sbyte value, sbyte increment)
  {
    return (sbyte) ((int) value - (int) increment);
  }
}
