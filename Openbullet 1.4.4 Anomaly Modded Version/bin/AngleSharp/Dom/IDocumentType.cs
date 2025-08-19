// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IDocumentType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("DocumentType")]
public interface IDocumentType : INode, IEventTarget, IMarkupFormattable, IChildNode
{
  [DomName("name")]
  string Name { get; }

  [DomName("publicId")]
  string PublicIdentifier { get; }

  [DomName("systemId")]
  string SystemIdentifier { get; }
}
