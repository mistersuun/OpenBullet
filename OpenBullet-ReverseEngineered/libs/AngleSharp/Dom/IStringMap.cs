// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IStringMap
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

[DomName("DOMStringMap")]
public interface IStringMap : IEnumerable<KeyValuePair<string, string>>, IEnumerable
{
  [DomAccessor(Accessors.Getter | Accessors.Setter)]
  string this[string name] { get; set; }

  [DomAccessor(Accessors.Deleter)]
  void Remove(string name);
}
