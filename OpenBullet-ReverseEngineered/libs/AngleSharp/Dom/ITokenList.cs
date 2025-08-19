// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ITokenList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

[DomName("DOMTokenList")]
public interface ITokenList : IEnumerable<string>, IEnumerable
{
  [DomName("length")]
  int Length { get; }

  [DomName("item")]
  [DomAccessor(Accessors.Getter)]
  string this[int index] { get; }

  [DomName("contains")]
  bool Contains(string token);

  [DomName("add")]
  void Add(params string[] tokens);

  [DomName("remove")]
  void Remove(params string[] tokens);

  [DomName("toggle")]
  bool Toggle(string token, bool force = false);
}
