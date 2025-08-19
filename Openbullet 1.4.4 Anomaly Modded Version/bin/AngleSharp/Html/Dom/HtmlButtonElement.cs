// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlButtonElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlButtonElement(Document owner, string prefix = null) : 
  HtmlFormControlElement(owner, TagNames.Button, prefix),
  IHtmlButtonElement,
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
  public string Type
  {
    get => (this.GetOwnAttribute(AttributeNames.Type) ?? InputTypeNames.Submit).ToLowerInvariant();
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string FormAction
  {
    get
    {
      string ownAttribute = this.GetOwnAttribute(AttributeNames.FormAction);
      if (ownAttribute != null)
        return ownAttribute;
      return this.Owner?.DocumentUri;
    }
    set => this.SetOwnAttribute(AttributeNames.FormAction, value);
  }

  public string FormEncType
  {
    get => this.GetOwnAttribute(AttributeNames.FormEncType).ToEncodingType() ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormEncType, value);
  }

  public string FormMethod
  {
    get => this.GetOwnAttribute(AttributeNames.FormMethod).ToFormMethod() ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormMethod, value);
  }

  public bool FormNoValidate
  {
    get => this.GetBoolAttribute(AttributeNames.FormNoValidate);
    set => this.SetBoolAttribute(AttributeNames.FormNoValidate, value);
  }

  public string FormTarget
  {
    get => this.GetOwnAttribute(AttributeNames.FormTarget) ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormTarget, value);
  }

  public string Value
  {
    get => this.GetOwnAttribute(AttributeNames.Value) ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.Value, value);
  }

  internal bool IsVisited { get; set; }

  internal bool IsActive { get; set; }

  public override async void DoClick()
  {
    HtmlButtonElement sourceElement = this;
    int num = await sourceElement.IsClickedCancelled().ConfigureAwait(false) ? 1 : 0;
    IHtmlFormElement form = sourceElement.Form;
    if (num != 0 || form == null)
      return;
    string type = sourceElement.Type;
    if (type.Is(InputTypeNames.Submit))
    {
      IDocument document = await form.SubmitAsync((IHtmlElement) sourceElement).ConfigureAwait(false);
    }
    else
    {
      if (!type.Is(InputTypeNames.Reset))
        return;
      form.Reset();
    }
  }

  protected override bool CanBeValidated()
  {
    return this.Type.Is(InputTypeNames.Submit) && !this.HasDataListAncestor();
  }

  internal override void ConstructDataSet(FormDataSet dataSet, IHtmlElement submitter)
  {
    string type = this.Type;
    if (this != submitter || !type.IsOneOf(InputTypeNames.Submit, InputTypeNames.Reset))
      return;
    dataSet.Append(this.Name, this.Value, type);
  }
}
