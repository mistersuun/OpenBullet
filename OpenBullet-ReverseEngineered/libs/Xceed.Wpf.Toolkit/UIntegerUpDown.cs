// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.UIntegerUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class UIntegerUpDown : CommonNumericUpDown<uint>
{
  static UIntegerUpDown()
  {
    CommonNumericUpDown<uint>.UpdateMetadata(typeof (UIntegerUpDown), new uint?(1U), new uint?(0U), new uint?(uint.MaxValue));
  }

  public UIntegerUpDown()
    : base(new CommonNumericUpDown<uint>.FromText(uint.TryParse), new CommonNumericUpDown<uint>.FromDecimal(Decimal.ToUInt32), (Func<uint, uint, bool>) ((v1, v2) => v1 < v2), (Func<uint, uint, bool>) ((v1, v2) => v1 > v2))
  {
  }

  protected override uint IncrementValue(uint value, uint increment) => value + increment;

  protected override uint DecrementValue(uint value, uint increment) => value - increment;
}
