// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.INodeIterator
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("NodeIterator")]
public interface INodeIterator
{
  [DomName("root")]
  INode Root { get; }

  [DomName("referenceNode")]
  INode Reference { get; }

  [DomName("pointerBeforeReferenceNode")]
  bool IsBeforeReference { get; }

  [DomName("whatToShow")]
  FilterSettings Settings { get; }

  [DomName("filter")]
  NodeFilter Filter { get; }

  [DomName("nextNode")]
  INode Next();

  [DomName("previousNode")]
  INode Previous();
}
