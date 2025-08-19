// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Location
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class Location : ILocation, IUrlUtilities
{
  private readonly Url _url;

  public event EventHandler<Location.ChangedEventArgs> Changed;

  internal Location(string url)
    : this(new Url(url))
  {
  }

  internal Location(Url url) => this._url = url ?? new Url(string.Empty);

  public Url Original => this._url;

  public string Origin => this._url.Origin;

  public bool IsRelative => this._url.IsRelative;

  public string UserName
  {
    get => this._url.UserName;
    set => this._url.UserName = value;
  }

  public string Password
  {
    get => this._url.Password;
    set => this._url.Password = value;
  }

  public string Hash
  {
    get => Location.NonEmptyPrefix(this._url.Fragment, "#");
    set
    {
      string href = this._url.Href;
      if (value != null)
      {
        if (value.Has('#'))
          value = value.Substring(1);
        else if (value.Length == 0)
          value = (string) null;
      }
      if (value.Is(this._url.Fragment))
        return;
      this._url.Fragment = value;
      this.RaiseHashChanged(href);
    }
  }

  public string Host
  {
    get => this._url.Host;
    set
    {
      string href = this._url.Href;
      if (value.Isi(this._url.Host))
        return;
      this._url.Host = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string HostName
  {
    get => this._url.HostName;
    set
    {
      string href = this._url.Href;
      if (value.Isi(this._url.HostName))
        return;
      this._url.HostName = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string Href
  {
    get => this._url.Href;
    set
    {
      string href = this._url.Href;
      if (value.Is(this._url.Href))
        return;
      this._url.Href = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string PathName
  {
    get
    {
      string data = this._url.Data;
      return !string.IsNullOrEmpty(data) ? data : "/" + this._url.Path;
    }
    set
    {
      string href = this._url.Href;
      if (value.Is(this._url.Path))
        return;
      this._url.Path = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string Port
  {
    get => this._url.Port;
    set
    {
      string href = this._url.Href;
      if (value.Isi(this._url.Port))
        return;
      this._url.Port = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string Protocol
  {
    get => Location.NonEmptyPostfix(this._url.Scheme, ":");
    set
    {
      string href = this._url.Href;
      if (value.Isi(this._url.Scheme))
        return;
      this._url.Scheme = value;
      this.RaiseLocationChanged(href);
    }
  }

  public string Search
  {
    get => Location.NonEmptyPrefix(this._url.Query, "?");
    set
    {
      string href = this._url.Href;
      if (value.Is(this._url.Query))
        return;
      this._url.Query = value;
      this.RaiseLocationChanged(href);
    }
  }

  public void Assign(string url)
  {
    string href = this._url.Href;
    if (href.Is(url))
      return;
    this._url.Href = url;
    this.RaiseLocationChanged(href);
  }

  public void Replace(string url)
  {
    string href = this._url.Href;
    if (href.Is(url))
      return;
    this._url.Href = url;
    this.RaiseLocationChanged(href);
  }

  public void Reload()
  {
    EventHandler<Location.ChangedEventArgs> changed = this.Changed;
    if (changed == null)
      return;
    changed((object) this, new Location.ChangedEventArgs(false, this._url.Href, this._url.Href));
  }

  public override string ToString() => this._url.Href;

  private void RaiseHashChanged(string oldAddress)
  {
    EventHandler<Location.ChangedEventArgs> changed = this.Changed;
    if (changed == null)
      return;
    changed((object) this, new Location.ChangedEventArgs(true, oldAddress, this._url.Href));
  }

  private void RaiseLocationChanged(string oldAddress)
  {
    EventHandler<Location.ChangedEventArgs> changed = this.Changed;
    if (changed == null)
      return;
    changed((object) this, new Location.ChangedEventArgs(false, oldAddress, this._url.Href));
  }

  private static string NonEmptyPrefix(string check, string prefix)
  {
    return !string.IsNullOrEmpty(check) ? prefix + check : string.Empty;
  }

  private static string NonEmptyPostfix(string check, string postfix)
  {
    return !string.IsNullOrEmpty(check) ? check + postfix : string.Empty;
  }

  public sealed class ChangedEventArgs : EventArgs
  {
    public ChangedEventArgs(bool hashChanged, string previousLocation, string currentLocation)
    {
      this.IsHashChanged = hashChanged;
      this.PreviousLocation = previousLocation;
      this.CurrentLocation = currentLocation;
    }

    public bool IsReloaded => this.PreviousLocation.Is(this.CurrentLocation);

    public bool IsHashChanged { get; private set; }

    public string PreviousLocation { get; private set; }

    public string CurrentLocation { get; private set; }
  }
}
