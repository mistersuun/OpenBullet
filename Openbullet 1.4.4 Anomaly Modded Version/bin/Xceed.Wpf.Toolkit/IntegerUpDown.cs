// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.IntegerUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class IntegerUpDown : CommonNumericUpDown<int>
{
  static IntegerUpDown()
  {
    CommonNumericUpDown<int>.UpdateMetadata(typeof (IntegerUpDown), new int?(1), new int?(int.MinValue), new int?(int.MaxValue));
  }

  public IntegerUpDown()
    : base(new CommonNumericUpDown<int>.FromText(int.TryParse), new CommonNumericUpDown<int>.FromDecimal(Decimal.ToInt32), (Func<int, int, bool>) ((v1, v2) => v1 < v2), (Func<int, int, bool>) ((v1, v2) => v1 > v2))
  {
  }

  protected override int IncrementValue(int value, int increment) => value + increment;

  protected override int DecrementValue(int value, int increment) => value - increment;
}
