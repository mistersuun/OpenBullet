// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IAttr
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Attr")]
public interface IAttr : IEquatable<IAttr>
{
  [DomName("localName")]
  string LocalName { get; }

  [DomName("name")]
  string Name { get; }

  [DomName("value")]
  string Value { get; set; }

  [DomName("namespaceURI")]
  string NamespaceUri { get; }

  [DomName("prefix")]
  string Prefix { get; }
}
