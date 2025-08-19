// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlLinkElementExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

public static class HtmlLinkElementExtensions
{
  public static bool IsPersistent(this IHtmlLinkElement link)
  {
    return link.Relation.Isi(LinkRelNames.StyleSheet) && link.Title == null;
  }

  public static bool IsPreferred(this IHtmlLinkElement link)
  {
    return link.Relation.Isi(LinkRelNames.StyleSheet) && link.Title != null;
  }

  public static bool IsAlternate(this IHtmlLinkElement link)
  {
    ITokenList relationList = link.RelationList;
    return relationList.Contains(LinkRelNames.StyleSheet) && relationList.Contains(LinkRelNames.Alternate) && link.Title != null;
  }
}
