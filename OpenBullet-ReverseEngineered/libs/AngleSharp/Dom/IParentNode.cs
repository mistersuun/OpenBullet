// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IParentNode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("ParentNode")]
[DomNoInterfaceObject]
public interface IParentNode
{
  [DomName("children")]
  IHtmlCollection<IElement> Children { get; }

  [DomName("firstElementChild")]
  IElement FirstElementChild { get; }

  [DomName("lastElementChild")]
  IElement LastElementChild { get; }

  [DomName("childElementCount")]
  int ChildElementCount { get; }

  [DomName("append")]
  void Append(params INode[] nodes);

  [DomName("prepend")]
  void Prepend(params INode[] nodes);

  [DomName("querySelector")]
  IElement QuerySelector(string selectors);

  [DomName("querySelectorAll")]
  IHtmlCollection<IElement> QuerySelectorAll(string selectors);
}
