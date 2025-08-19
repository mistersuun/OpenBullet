// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlStyleElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlStyleElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Style, prefix, NodeFlags.Special | NodeFlags.LiteralText),
  IHtmlStyleElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILinkStyle
{
  private IStyleSheet _sheet;

  public bool IsScoped
  {
    get => this.GetBoolAttribute(AttributeNames.Scoped);
    set => this.SetBoolAttribute(AttributeNames.Scoped, value);
  }

  public IStyleSheet Sheet => this._sheet;

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled);
    set
    {
      this.SetBoolAttribute(AttributeNames.Disabled, value);
      if (this._sheet == null)
        return;
      this._sheet.IsDisabled = value;
    }
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

  internal override void SetupElement()
  {
    base.SetupElement();
    this.UpdateSheet();
  }

  internal void UpdateMedia(string value)
  {
    if (this._sheet == null)
      return;
    this._sheet.Media.MediaText = value;
  }

  protected override void NodeIsInserted(Node newNode)
  {
    base.NodeIsInserted(newNode);
    this.UpdateSheet();
  }

  protected override void NodeIsRemoved(Node removedNode, Node oldPreviousSibling)
  {
    base.NodeIsRemoved(removedNode, oldPreviousSibling);
    this.UpdateSheet();
  }

  private void UpdateSheet()
  {
    Document owner = this.Owner;
    if (owner == null)
      return;
    IStylingService styling = this.Context.GetStyling(this.Type ?? MimeTypeNames.Css);
    if (styling == null)
      return;
    Task sheetAsync = this.CreateSheetAsync(styling, (IDocument) owner);
    owner.DelayLoad(sheetAsync);
  }

  private async Task CreateSheetAsync(IStylingService engine, IDocument document)
  {
    HtmlStyleElement htmlStyleElement = this;
    CancellationToken none = CancellationToken.None;
    // ISSUE: reference to a compiler-generated method
    IResponse response = VirtualResponse.Create(new Action<VirtualResponse>(htmlStyleElement.\u003CCreateSheetAsync\u003Eb__21_0));
    StyleOptions options = new StyleOptions(document)
    {
      Element = (IElement) htmlStyleElement,
      IsDisabled = htmlStyleElement.IsDisabled,
      IsAlternate = false
    };
    IStyleSheet styleSheet = await engine.ParseStylesheetAsync(response, options, none).ConfigureAwait(false);
    htmlStyleElement._sheet = styleSheet;
  }
}
