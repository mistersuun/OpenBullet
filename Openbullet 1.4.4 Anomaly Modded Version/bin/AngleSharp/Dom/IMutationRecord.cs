// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IMutationRecord
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("MutationRecord")]
public interface IMutationRecord
{
  [DomName("type")]
  string Type { get; }

  [DomName("target")]
  INode Target { get; }

  [DomName("addedNodes")]
  INodeList Added { get; }

  [DomName("removedNodes")]
  INodeList Removed { get; }

  [DomName("previousSibling")]
  INode PreviousSibling { get; }

  [DomName("nextSibling")]
  INode NextSibling { get; }

  [DomName("attributeName")]
  string AttributeName { get; }

  [DomName("attributeNamespace")]
  string AttributeNamespace { get; }

  [DomName("oldValue")]
  string PreviousValue { get; }
}
