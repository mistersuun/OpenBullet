// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlSlotElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlSlotElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Slot, prefix),
  IHtmlSlotElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }

  public IEnumerable<INode> GetDistributedNodes()
  {
    IElement host = this.GetAncestor<IShadowRoot>()?.Host;
    if (host == null)
      return Enumerable.Empty<INode>();
    List<INode> distributedNodes = new List<INode>();
    foreach (INode childNode in (IEnumerable<INode>) host.ChildNodes)
    {
      if (HtmlSlotElement.GetAssignedSlot(childNode) == this)
      {
        if (childNode is HtmlSlotElement htmlSlotElement)
          distributedNodes.AddRange(htmlSlotElement.GetDistributedNodes());
        else
          distributedNodes.Add(childNode);
      }
    }
    return (IEnumerable<INode>) distributedNodes;
  }

  private static IElement GetAssignedSlot(INode node)
  {
    switch (node.NodeType)
    {
      case NodeType.Element:
        return ((IElement) node).AssignedSlot;
      case NodeType.Text:
        return ((IText) node).AssignedSlot;
      default:
        return (IElement) null;
    }
  }
}
