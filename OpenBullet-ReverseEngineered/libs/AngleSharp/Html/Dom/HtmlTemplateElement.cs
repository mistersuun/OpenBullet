// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTemplateElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTemplateElement : 
  HtmlElement,
  IHtmlTemplateElement,
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
  private readonly DocumentFragment _content;

  public HtmlTemplateElement(Document owner, string prefix = null)
    : base(owner, TagNames.Template, prefix, NodeFlags.Special | NodeFlags.Scoped | NodeFlags.HtmlTableSectionScoped | NodeFlags.HtmlTableScoped)
  {
    this._content = new DocumentFragment(owner);
  }

  public IDocumentFragment Content => (IDocumentFragment) this._content;

  public override Node Clone(Document owner, bool deep)
  {
    HtmlTemplateElement htmlTemplateElement = new HtmlTemplateElement(owner);
    this.CloneElement((Element) htmlTemplateElement, owner, deep);
    DocumentFragment content = htmlTemplateElement._content;
    foreach (INode childNode in this._content.ChildNodes)
    {
      if (childNode is Node node1)
      {
        Node node = node1.Clone(owner, deep);
        content.AddNode(node);
      }
    }
    return (Node) htmlTemplateElement;
  }

  public void PopulateFragment()
  {
    while (this.HasChildNodes)
    {
      Node childNode = this.ChildNodes[0];
      this.RemoveNode(0, childNode);
      this._content.AddNode(childNode);
    }
  }

  protected override void NodeIsAdopted(Document oldDocument) => this._content.Owner = oldDocument;
}
