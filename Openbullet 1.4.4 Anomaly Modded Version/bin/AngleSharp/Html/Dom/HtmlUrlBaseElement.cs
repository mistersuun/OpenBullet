// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlUrlBaseElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlUrlBaseElement : HtmlElement, IUrlUtilities
{
  private TokenList _relList;
  private SettableTokenList _ping;

  public HtmlUrlBaseElement(Document owner, string name, string prefix, NodeFlags flags)
    : base(owner, name, prefix, flags)
  {
  }

  public string Download
  {
    get => this.GetOwnAttribute(AttributeNames.Download);
    set => this.SetOwnAttribute(AttributeNames.Download, value);
  }

  public string Href
  {
    get => this.GetUrlAttribute(AttributeNames.Href);
    set => this.SetAttribute(AttributeNames.Href, value);
  }

  public string Hash
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Hash));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Hash = value));
  }

  public string Host
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Host));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Host = value));
  }

  public string HostName
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.HostName));
    set => this.SetLocationPart((Action<ILocation>) (m => m.HostName = value));
  }

  public string PathName
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.PathName));
    set => this.SetLocationPart((Action<ILocation>) (m => m.PathName = value));
  }

  public string Port
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Port));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Port = value));
  }

  public string Protocol
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Protocol));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Protocol = value));
  }

  public string UserName
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.UserName));
    set => this.SetLocationPart((Action<ILocation>) (m => m.UserName = value));
  }

  public string Password
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Password));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Password = value));
  }

  public string Search
  {
    get => this.GetLocationPart((Func<ILocation, string>) (m => m.Search));
    set => this.SetLocationPart((Action<ILocation>) (m => m.Search = value));
  }

  public string Origin => this.GetLocationPart((Func<ILocation, string>) (m => m.Origin));

  public string TargetLanguage
  {
    get => this.GetOwnAttribute(AttributeNames.HrefLang);
    set => this.SetOwnAttribute(AttributeNames.HrefLang, value);
  }

  public string Media
  {
    get => this.GetOwnAttribute(AttributeNames.Media);
    set => this.SetOwnAttribute(AttributeNames.Media, value);
  }

  public string Relation
  {
    get => this.GetOwnAttribute(AttributeNames.Rel);
    set => this.SetOwnAttribute(AttributeNames.Rel, value);
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

  public ISettableTokenList Ping
  {
    get
    {
      if (this._ping == null)
      {
        this._ping = new SettableTokenList(this.GetOwnAttribute(AttributeNames.Ping));
        this._ping.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.Ping, value));
      }
      return (ISettableTokenList) this._ping;
    }
  }

  public string Target
  {
    get => this.GetOwnAttribute(AttributeNames.Target);
    set => this.SetOwnAttribute(AttributeNames.Target, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  internal bool IsVisited { get; set; }

  internal bool IsActive { get; set; }

  public override async void DoClick()
  {
    HtmlUrlBaseElement element = this;
    if (await element.IsClickedCancelled().ConfigureAwait(false))
      return;
    IDocument document = await element.NavigateAsync<HtmlUrlBaseElement>().ConfigureAwait(false);
  }

  internal void UpdateRel(string value) => this._relList?.Update(value);

  internal void UpdatePing(string value) => this._ping?.Update(value);

  private string GetLocationPart(Func<ILocation, string> getter)
  {
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Href);
    Url url = ownAttribute != null ? new Url(this.BaseUrl, ownAttribute) : (Url) null;
    if (url == null || url.IsInvalid)
      return string.Empty;
    Location location = new Location(url);
    return getter((ILocation) location);
  }

  private void SetLocationPart(Action<ILocation> setter)
  {
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Href);
    Url url = ownAttribute != null ? new Url(this.BaseUrl, ownAttribute) : (Url) null;
    if (url == null || url.IsInvalid)
      url = new Url(this.BaseUrl);
    Location location = new Location(url);
    setter((ILocation) location);
    this.SetOwnAttribute(AttributeNames.Href, location.Href);
  }
}
