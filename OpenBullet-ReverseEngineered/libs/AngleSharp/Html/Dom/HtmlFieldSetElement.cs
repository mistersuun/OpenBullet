// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFieldSetElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlFieldSetElement(Document owner, string prefix = null) : 
  HtmlFormControlElement(owner, TagNames.Fieldset, prefix),
  IHtmlFieldSetElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IValidation
{
  private HtmlFormControlsCollection _elements;

  public string Type => TagNames.Fieldset;

  public IHtmlFormControlsCollection Elements
  {
    get
    {
      return (IHtmlFormControlsCollection) this._elements ?? (IHtmlFormControlsCollection) (this._elements = new HtmlFormControlsCollection((IElement) this.Form, (IElement) this));
    }
  }

  protected override bool IsFieldsetDisabled() => false;

  protected override bool CanBeValidated() => true;
}
