// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ICssMedium
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Dom;

public interface ICssMedium : IStyleFormattable
{
  string Type { get; }

  bool IsExclusive { get; }

  bool IsInverse { get; }

  string Constraints { get; }

  IEnumerable<IMediaFeature> Features { get; }
}
