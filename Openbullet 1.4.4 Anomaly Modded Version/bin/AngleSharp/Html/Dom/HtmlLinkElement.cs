// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlLinkElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.LinkRels;
using AngleSharp.Io;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlLinkElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Link, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlLinkElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILinkStyle,
  ILinkImport,
  ILoadableElement
{
  private BaseLinkRelation _relation;
  private TokenList _relList;
  private SettableTokenList _sizes;

  internal bool IsVisited { get; set; }

  internal bool IsActive { get; set; }

  public IDownload CurrentDownload => this._relation?.Processor?.Download;

  public string Href
  {
    get => this.GetUrlAttribute(AttributeNames.Href);
    set => this.SetOwnAttribute(AttributeNames.Href, value);
  }

  public string TargetLanguage
  {
    get => this.GetOwnAttribute(AttributeNames.HrefLang);
    set => this.SetOwnAttribute(AttributeNames.HrefLang, value);
  }

  public string Charset
  {
    get => this.GetOwnAttribute(AttributeNames.Charset);
    set => this.SetOwnAttribute(AttributeNames.Charset, value);
  }

  public string Relation
  {
    get => this.GetOwnAttribute(AttributeNames.Rel);
    set => this.SetOwnAttribute(AttributeNames.Rel, value);
  }

  public string ReverseRelation
  {
    get => this.GetOwnAttribute(AttributeNames.Rev);
    set => this.SetOwnAttribute(AttributeNames.Rev, value);
  }

  public string NumberUsedOnce
  {
    get => this.GetOwnAttribute(AttributeNames.Nonce);
    set => this.SetOwnAttribute(AttributeNames.Nonce, value);
  }

  public ITokenList RelationList
  {
    get
    {
      if (this._relList == null)
      {
        this._relList = new TokenList(this.GetOwnAttribute(AttributeNames.Rel));
        this._relList.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.Rel, value));
      }
      return (ITokenList) this._relList;
    }
  }

  public ISettableTokenList Sizes
  {
    get
    {
      if (this._sizes == null)
      {
        this._sizes = new SettableTokenList(this.GetOwnAttribute(AttributeNames.Sizes));
        this._sizes.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.Sizes, value));
      }
      return (ISettableTokenList) this._sizes;
    }
  }

  public string Rev
  {
    get => this.GetOwnAttribute(AttributeNames.Rev);
    set => this.SetOwnAttribute(AttributeNames.Rev, value);
  }

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled);
    set => this.SetBoolAttribute(AttributeNames.Disabled, value);
  }

  public string Target
  {
    get => this.GetOwnAttribute(AttributeNames.Target);
    set => this.SetOwnAttribute(AttributeNames.Target, value);
  }

  public string Media
  {
    get => this.GetOwnAttribute(AttributeNames.Media);
    set => this.SetOwnAttribute(AttributeNames.Media, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string Integrity
  {
    get => this.GetOwnAttribute(AttributeNames.Integrity);
    set => this.SetOwnAttribute(AttributeNames.Integrity, value);
  }

  public IStyleSheet Sheet
  {
    get
    {
      return !(this._relation is StyleSheetLinkRelation relation) ? (IStyleSheet) null : relation.Sheet;
    }
  }

  public IDocument Import
  {
    get => !(this._relation is ImportLinkRelation relation) ? (IDocument) null : relation.Import;
  }

  public string CrossOrigin
  {
    get => this.GetOwnAttribute(AttributeNames.CrossOrigin);
    set => this.SetOwnAttribute(AttributeNames.CrossOrigin, value);
  }

  internal override void SetupElement()
  {
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Rel);
    if (ownAttribute != null)
    {
      this._relList?.Update(ownAttribute);
      this._relation = this.CreateFirstLegalRelation();
    }
    base.SetupElement();
  }

  internal void UpdateSizes(string value) => this._sizes?.Update(value);

  internal void UpdateMedia(string value)
  {
    IStyleSheet sheet = this.Sheet;
    if (sheet == null)
      return;
    sheet.Media.MediaText = value;
  }

  internal void UpdateDisabled(string value)
  {
    IStyleSheet sheet = this.Sheet;
    if (sheet == null)
      return;
    sheet.IsDisabled = value != null;
  }

  internal void UpdateSource(string value) => this.Owner?.DelayLoad(this._relation?.LoadAsync());

  private BaseLinkRelation CreateFirstLegalRelation()
  {
    ITokenList relationList = this.RelationList;
    IBrowsingContext context = this.Context;
    ILinkRelationFactory factory = context != null ? context.GetFactory<ILinkRelationFactory>() : (ILinkRelationFactory) null;
    foreach (string relation in (IEnumerable<string>) relationList)
    {
      BaseLinkRelation firstLegalRelation = factory?.Create((IHtmlLinkElement) this, relation);
      if (firstLegalRelation != null)
        return firstLegalRelation;
    }
    return (BaseLinkRelation) null;
  }
}
