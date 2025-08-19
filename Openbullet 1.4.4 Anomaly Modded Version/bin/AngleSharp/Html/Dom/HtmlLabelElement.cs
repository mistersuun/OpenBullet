// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlLabelElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlLabelElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Label, prefix),
  IHtmlLabelElement,
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
  public IHtmlElement Control
  {
    get
    {
      string htmlFor = this.HtmlFor;
      if (!string.IsNullOrEmpty(htmlFor))
      {
        IHtmlElement elementById = this.Owner.GetElementById(htmlFor) as IHtmlElement;
        if (elementById is ILabelabelElement)
          return elementById;
      }
      return (IHtmlElement) null;
    }
  }

  public string HtmlFor
  {
    get => this.GetOwnAttribute(AttributeNames.For);
    set => this.SetOwnAttribute(AttributeNames.For, value);
  }

  public IHtmlFormElement Form => this.GetAssignedForm();
}
