// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ITreeWalker
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("TreeWalker")]
public interface ITreeWalker
{
  [DomName("root")]
  INode Root { get; }

  [DomName("currentNode")]
  INode Current { get; set; }

  [DomName("whatToShow")]
  FilterSettings Settings { get; }

  [DomName("filter")]
  NodeFilter Filter { get; }

  [DomName("nextNode")]
  INode ToNext();

  [DomName("previousNode")]
  INode ToPrevious();

  [DomName("parentNode")]
  INode ToParent();

  [DomName("firstChild")]
  INode ToFirst();

  [DomName("lastChild")]
  INode ToLast();

  [DomName("previousSibling")]
  INode ToPreviousSibling();

  [DomName("nextSibling")]
  INode ToNextSibling();
}
