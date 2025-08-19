// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlKeygenElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlKeygenElement(Document owner, string prefix = null) : 
  HtmlFormControlElementWithState(owner, TagNames.Keygen, prefix, NodeFlags.SelfClosing),
  IHtmlKeygenElement,
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
  public string Challenge
  {
    get => this.GetOwnAttribute(AttributeNames.Challenge);
    set => this.SetOwnAttribute(AttributeNames.Challenge, value);
  }

  public string KeyEncryption
  {
    get => this.GetOwnAttribute(AttributeNames.Keytype);
    set => this.SetOwnAttribute(AttributeNames.Keytype, value);
  }

  public string Type => TagNames.Keygen;

  internal override FormControlState SaveControlState()
  {
    return new FormControlState(this.Name, this.Type, this.Challenge);
  }

  internal override void RestoreFormControlState(FormControlState state)
  {
    if (!state.Type.Is(this.Type) || !state.Name.Is(this.Name))
      return;
    this.Challenge = state.Value;
  }

  protected override bool CanBeValidated() => false;
}
