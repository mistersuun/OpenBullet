// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ByteUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class ByteUpDown : CommonNumericUpDown<byte>
{
  static ByteUpDown()
  {
    CommonNumericUpDown<byte>.UpdateMetadata(typeof (ByteUpDown), new byte?((byte) 1), new byte?((byte) 0), new byte?(byte.MaxValue));
  }

  public ByteUpDown()
    : base(new CommonNumericUpDown<byte>.FromText(byte.TryParse), new CommonNumericUpDown<byte>.FromDecimal(Decimal.ToByte), (Func<byte, byte, bool>) ((v1, v2) => (int) v1 < (int) v2), (Func<byte, byte, bool>) ((v1, v2) => (int) v1 > (int) v2))
  {
  }

  protected override byte IncrementValue(byte value, byte increment)
  {
    return (byte) ((uint) value + (uint) increment);
  }

  protected override byte DecrementValue(byte value, byte increment)
  {
    return (byte) ((uint) value - (uint) increment);
  }
}
