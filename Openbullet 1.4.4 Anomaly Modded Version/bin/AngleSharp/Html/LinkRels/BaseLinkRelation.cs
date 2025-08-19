// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.LinkRels.BaseLinkRelation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Io.Processors;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.LinkRels;

public abstract class BaseLinkRelation
{
  private readonly IHtmlLinkElement _link;
  private readonly IRequestProcessor _processor;

  public BaseLinkRelation(IHtmlLinkElement link, IRequestProcessor processor)
  {
    this._link = link;
    this._processor = processor;
  }

  public IRequestProcessor Processor => this._processor;

  public IHtmlLinkElement Link => this._link;

  public Url Url => !string.IsNullOrEmpty(this._link.Href) ? new Url(this._link.Href) : (Url) null;

  public abstract Task LoadAsync();
}
