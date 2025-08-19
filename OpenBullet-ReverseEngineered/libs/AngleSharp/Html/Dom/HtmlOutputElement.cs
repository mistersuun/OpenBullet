// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlOutputElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlOutputElement(Document owner, string prefix = null) : 
  HtmlFormControlElement(owner, TagNames.Output, prefix),
  IHtmlOutputElement,
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
  private string _defaultValue;
  private string _value;
  private SettableTokenList _for;

  public string DefaultValue
  {
    get => this._defaultValue ?? this.TextContent;
    set => this._defaultValue = value;
  }

  public override string TextContent
  {
    get => this._value ?? this._defaultValue ?? base.TextContent;
    set => base.TextContent = value;
  }

  public string Value
  {
    get => this.TextContent;
    set => this._value = value;
  }

  public ISettableTokenList HtmlFor
  {
    get
    {
      if (this._for == null)
      {
        this._for = new SettableTokenList(this.GetOwnAttribute(AttributeNames.For));
        this._for.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.For, value));
      }
      return (ISettableTokenList) this._for;
    }
  }

  public string Type => TagNames.Output;

  internal override void Reset() => this._value = (string) null;

  internal void UpdateFor(string value) => this._for?.Update(value);

  protected override bool CanBeValidated() => true;
}
