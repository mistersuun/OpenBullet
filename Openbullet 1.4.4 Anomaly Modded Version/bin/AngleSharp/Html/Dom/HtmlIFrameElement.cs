// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlIFrameElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlIFrameElement(Document owner, string prefix = null) : 
  HtmlFrameElementBase(owner, TagNames.Iframe, prefix, NodeFlags.LiteralText),
  IHtmlInlineFrameElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILoadableElement
{
  private SettableTokenList _sandbox;

  public Alignment Align
  {
    get => this.GetOwnAttribute(AttributeNames.Align).ToEnum<Alignment>(Alignment.Bottom);
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public string ContentHtml
  {
    get => this.GetOwnAttribute(AttributeNames.SrcDoc);
    set => this.SetOwnAttribute(AttributeNames.SrcDoc, value);
  }

  public ISettableTokenList Sandbox
  {
    get
    {
      if (this._sandbox == null)
      {
        this._sandbox = new SettableTokenList(this.GetOwnAttribute(AttributeNames.Sandbox));
        this._sandbox.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.Sandbox, value));
      }
      return (ISettableTokenList) this._sandbox;
    }
  }

  public bool IsSeamless
  {
    get => this.GetBoolAttribute(AttributeNames.SrcDoc);
    set => this.SetBoolAttribute(AttributeNames.SrcDoc, value);
  }

  public bool IsFullscreenAllowed
  {
    get => this.GetBoolAttribute(AttributeNames.AllowFullscreen);
    set => this.SetBoolAttribute(AttributeNames.AllowFullscreen, value);
  }

  public bool IsPaymentRequestAllowed
  {
    get => this.GetBoolAttribute(AttributeNames.AllowPaymentRequest);
    set => this.SetBoolAttribute(AttributeNames.AllowPaymentRequest, value);
  }

  public string ReferrerPolicy
  {
    get => this.GetOwnAttribute(AttributeNames.ReferrerPolicy);
    set => this.SetOwnAttribute(AttributeNames.ReferrerPolicy, value);
  }

  public IWindow ContentWindow => this.NestedContext.Current;

  internal override string GetContentHtml() => this.ContentHtml;

  internal override void SetupElement()
  {
    base.SetupElement();
    if (this.GetOwnAttribute(AttributeNames.SrcDoc) == null)
      return;
    this.UpdateSource();
  }

  internal void UpdateSandbox(string value) => this._sandbox?.Update(value);
}
