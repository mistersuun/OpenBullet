// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.AllowedSpecialValues
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[Flags]
public enum AllowedSpecialValues
{
  None = 0,
  NaN = 1,
  PositiveInfinity = 2,
  NegativeInfinity = 4,
  AnyInfinity = NegativeInfinity | PositiveInfinity, // 0x00000006
  Any = AnyInfinity | NaN, // 0x00000007
}
