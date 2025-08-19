// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.IMediaList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Dom;

[DomName("MediaList")]
public interface IMediaList : IEnumerable<ICssMedium>, IEnumerable, IStyleFormattable
{
  [DomName("mediaText")]
  string MediaText { get; set; }

  [DomName("length")]
  int Length { get; }

  [DomAccessor(Accessors.Getter)]
  [DomName("item")]
  string this[int index] { get; }

  [DomName("appendMedium")]
  void Add(string medium);

  [DomName("removeMedium")]
  void Remove(string medium);
}
