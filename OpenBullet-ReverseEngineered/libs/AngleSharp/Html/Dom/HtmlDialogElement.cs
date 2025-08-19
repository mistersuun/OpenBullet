// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlDialogElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlDialogElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Dialog, prefix),
  IHtmlDialogElement,
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
  private string _returnValue;

  public bool Open
  {
    get => this.GetBoolAttribute(AttributeNames.Open);
    set => this.SetBoolAttribute(AttributeNames.Open, value);
  }

  public string ReturnValue
  {
    get => this._returnValue;
    set => this._returnValue = value;
  }

  public void Show(IElement anchor = null) => this.Open = true;

  public void ShowModal(IElement anchor = null) => this.Open = true;

  public void Close(string returnValue = null)
  {
    this.Open = false;
    this.ReturnValue = returnValue;
  }
}
