// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTrackElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Media.Dom;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTrackElement : 
  HtmlElement,
  IHtmlTrackElement,
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
  private TrackReadyState _ready;

  public HtmlTrackElement(Document owner, string prefix = null)
    : base(owner, TagNames.Track, prefix, NodeFlags.SelfClosing | NodeFlags.Special)
  {
    this._ready = TrackReadyState.None;
  }

  public string Kind
  {
    get => this.GetOwnAttribute(AttributeNames.Kind);
    set => this.SetOwnAttribute(AttributeNames.Kind, value);
  }

  public string Source
  {
    get => this.GetUrlAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string SourceLanguage
  {
    get => this.GetOwnAttribute(AttributeNames.SrcLang);
    set => this.SetOwnAttribute(AttributeNames.SrcLang, value);
  }

  public string Label
  {
    get => this.GetOwnAttribute(AttributeNames.Label);
    set => this.SetOwnAttribute(AttributeNames.Label, value);
  }

  public bool IsDefault
  {
    get => this.GetBoolAttribute(AttributeNames.Default);
    set => this.SetBoolAttribute(AttributeNames.Default, value);
  }

  public TrackReadyState ReadyState => this._ready;

  public ITextTrack Track => (ITextTrack) null;
}
