// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.RefreshMetaHandler
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Browser;

public class RefreshMetaHandler : IMetaHandler
{
  private readonly Predicate<Url> _shouldRefresh;

  public RefreshMetaHandler(Predicate<Url> shouldRefresh = null)
  {
    this._shouldRefresh = shouldRefresh ?? new Predicate<Url>(RefreshMetaHandler.AlwaysRefresh);
  }

  void IMetaHandler.HandleContent(IHtmlMetaElement element)
  {
    if (!element.HttpEquivalent.Isi("refresh"))
      return;
    IDocument document = element.Owner;
    string content = element.Content;
    Url baseAddress = new Url(document.DocumentUri);
    Url redirectUrl = baseAddress;
    string s = content;
    int length = content.IndexOf(';');
    if (length >= 0)
    {
      s = content.Substring(0, length);
      string str = content.Substring(length + 1).Trim();
      if (str.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
      {
        string relativeAddress = str.Substring(4);
        if (relativeAddress.Length > 0)
          redirectUrl = new Url(baseAddress, relativeAddress);
      }
    }
    int result;
    if (!int.TryParse(s, out result))
      return;
    Task.Delay(TimeSpan.FromSeconds((double) result)).ContinueWith((Action<Task>) (task => document.Location.Assign(redirectUrl.Href)));
  }

  private static bool AlwaysRefresh(Url url) => true;
}
