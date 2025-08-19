// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlObjectElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlObjectElement : 
  HtmlFormControlElement,
  IHtmlObjectElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IValidation,
  ILoadableElement
{
  private readonly ObjectRequestProcessor _request;

  public HtmlObjectElement(Document owner, string prefix = null)
    : base(owner, TagNames.Object, prefix, NodeFlags.Scoped)
  {
    this._request = new ObjectRequestProcessor(owner.Context);
  }

  public IDownload CurrentDownload => this._request?.Download;

  public string Source
  {
    get => this.GetUrlAttribute(AttributeNames.Data);
    set => this.SetOwnAttribute(AttributeNames.Data, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public bool TypeMustMatch
  {
    get => this.GetBoolAttribute(AttributeNames.TypeMustMatch);
    set => this.SetBoolAttribute(AttributeNames.TypeMustMatch, value);
  }

  public string UseMap
  {
    get => this.GetOwnAttribute(AttributeNames.UseMap);
    set => this.SetOwnAttribute(AttributeNames.UseMap, value);
  }

  public int DisplayWidth
  {
    get => this.GetOwnAttribute(AttributeNames.Width).ToInteger(this.OriginalWidth);
    set => this.SetOwnAttribute(AttributeNames.Width, value.ToString());
  }

  public int DisplayHeight
  {
    get => this.GetOwnAttribute(AttributeNames.Height).ToInteger(this.OriginalHeight);
    set => this.SetOwnAttribute(AttributeNames.Height, value.ToString());
  }

  public int OriginalWidth
  {
    get
    {
      ObjectRequestProcessor request = this._request;
      return request == null ? 0 : request.Width;
    }
  }

  public int OriginalHeight
  {
    get
    {
      ObjectRequestProcessor request = this._request;
      return request == null ? 0 : request.Height;
    }
  }

  public IDocument ContentDocument => (IDocument) null;

  public IWindow ContentWindow => (IWindow) null;

  protected override bool CanBeValidated() => false;

  internal override void SetupElement()
  {
    base.SetupElement();
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Data);
    if (ownAttribute == null)
      return;
    this.UpdateSource(ownAttribute);
  }

  internal void UpdateSource(string value)
  {
    this.Process((IRequestProcessor) this._request, new Url(this.Source));
  }
}
